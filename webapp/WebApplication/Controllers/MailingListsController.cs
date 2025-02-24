using K9.Base.Globalisation;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.Options;
using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Exceptions;
using K9.WebApplication.Packages;
using System;
using System.Linq;
using System.Web.Mvc;
using K9.WebApplication.Models;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    [RoutePrefix("mailinglists")]
    public class MailingListsController : BaseNineStarKiController<MailingList>
    {
        private readonly IRepository<MailingListUser> _mailingListUsersRepository;

        public MailingListsController(IControllerPackage<MailingList> controllerPackage, INineStarKiPackage nineStarKiPackage, IRepository<MailingListUser> mailingListUsersRepository)
            : base(controllerPackage, nineStarKiPackage)
        {
            _mailingListUsersRepository = mailingListUsersRepository;
        }

        [Route("select-users")]
        public ActionResult SelectUsers(int id)
        {
            try
            {
                var mailingList = GetMailingList(id);
                return View(new MailingListViewModel
                {
                    MailingList = mailingList
                });
            }
            catch (NotFoundException exception)
            {
                return HttpNotFound();
            }
        }

        [ChildActionOnly]
        public ActionResult SelectUsersTable(int id)
        {
            try
            {
                var mailingList = GetMailingList(id);
                return PartialView("_SelectUsersTable", mailingList);
            }
            catch (NotFoundException exception)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public JsonResult SelectUsersTableJson(int id, string sqlQuery)
        {
            if (!string.IsNullOrEmpty(sqlQuery))
            {
                if (!SqlValidator.IsSafeSqlQuery(sqlQuery))
                    return Json(new
                    {
                        success = false,
                        error = "Invalid SQL query. Only SELECT statements are allowed."
                    });
            }

            try
            {
                var mailingList = GetMailingList(id, sqlQuery);
                return Json(PartialView("_SelectUsersTable", mailingList).ToString(), JsonRequestBehavior.AllowGet);
            }
            catch (NotFoundException)
            {
                return Json(new { success = false, error = "Mailing list not found" });
            }
        }

        [Route("select-users")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SelectUsers(MailingListViewModel model)
        {
            var mailingList = Repository.Find(model.MailingList.Id);
            var existingUsers = _mailingListUsersRepository.Find(e => e.MailingListId == model.MailingList.Id).ToList();
            _mailingListUsersRepository.DeleteBatch(existingUsers);

            var selectedUsers = model.MailingList.Users.Where(e => e.IsSelected).Select(e => new MailingListUser
            {
                MailingListId = model.MailingList.Id,
                UserId = e.Id
            }).ToList();
            _mailingListUsersRepository.CreateBatch(selectedUsers);

            ViewBag.IsPopupAlert = true;
            ViewBag.AlertOptions = new AlertOptions
            {
                AlertType = EAlertType.Success,
                Message = Dictionary.Success,
                OtherMessage = Globalisation.Dictionary.MailingListUsersUpdated
            };

            try
            {
                mailingList = GetMailingList(model.MailingList.Id);
                return View(mailingList);
            }
            catch (NotFoundException exception)
            {
                return HttpNotFound();
            }
        }

        private MailingList GetMailingList(int id, string sqlQuery = "")
        {
            var mailingList = Repository.Find(id);
            if (mailingList == null)
            {
                throw new NotFoundException();
            }

            var mailingListUserIds = _mailingListUsersRepository.Find(e => e.MailingListId == id).Select(e => e.UserId).ToList();
            var usersToDisplay = string.IsNullOrEmpty(sqlQuery)
                ? My.UsersRepository.List().OrderBy(e => e.FirstName).ThenBy(e => e.LastName).ToList()
                : My.UsersRepository.GetQuery(sqlQuery);

            usersToDisplay.ForEach(e => e.IsSelected = mailingListUserIds.Contains(e.Id));
            mailingList.Users = usersToDisplay;

            return mailingList;
        }
    }
}