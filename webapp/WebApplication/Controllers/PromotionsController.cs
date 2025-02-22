using K9.Base.WebApplication.UnitsOfWork;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Packages;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    public class PromotionsController : BaseNineStarKiController<Promotion>
    {
        private readonly IRepository<MembershipOption> _membershipOptionsRepository;

        public PromotionsController(IControllerPackage<Promotion> controllerPackage, INineStarKiPackage nineStarKiPackage, IRepository<MembershipOption> membershipOptionsRepository)
            : base(controllerPackage, nineStarKiPackage)
        {
            _membershipOptionsRepository = membershipOptionsRepository;
        }

        public override ActionResult Create()
        {
            return View(new Promotion
            {
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult Create(Promotion promotion)
        {
            Validate(promotion);

            if (ModelState.IsValid)
            {
                promotion.Name = promotion.Code;

                try
                {
                    Repository.Create(promotion);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.GetFullErrorMessage());
                    return View(promotion);
                }

                return RedirectToAction("Index");
            }

            return View(promotion);
        }

        public JsonResult Get(int id)
        {
            try
            {
                var promotion = Repository.Find(id);
                promotion.MembershipOption = _membershipOptionsRepository.Find(promotion.MembershipOptionId);

                return Json(new
                {
                    success = true,
                    promotion
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                Logger.Error(e.GetFullErrorMessage);
                return Json(new
                {
                    success = false,
                    error = e.GetFullErrorMessage()
                });
            }
        }

        private void Validate(Promotion promotion)
        {
            var membershipOption = _membershipOptionsRepository.Find(promotion.MembershipOptionId);
            if (membershipOption.SubscriptionType == MembershipOption.ESubscriptionType.Free)
            {
                ModelState.AddModelError(nameof(Promotion.MembershipOptionId), "Cannot create promocode for free membership");
            }

            if (membershipOption.Price == promotion.SpecialPrice)
            {
                ModelState.AddModelError(nameof(Promotion.SpecialPrice), "Total price must be discounted.");
            }
        }
    }
}