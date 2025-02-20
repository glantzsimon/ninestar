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
    public class PromoCodesController : BaseNineStarKiController<PromoCode>
    {
        private readonly IRepository<MembershipOption> _membershipOptionsRepository;

        public PromoCodesController(IControllerPackage<PromoCode> controllerPackage, INineStarKiControllerPackage nineStarKiControllerPackage, IRepository<MembershipOption> membershipOptionsRepository)
            : base(controllerPackage, nineStarKiControllerPackage)
        {
            _membershipOptionsRepository = membershipOptionsRepository;
        }

        public override ActionResult Create()
        {
            return View(new PromoCode
            {
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult Create(PromoCode promoCode)
        {
            Validate(promoCode);

            if (ModelState.IsValid)
            {
                promoCode.Name = promoCode.Code;

                try
                {
                    Repository.Create(promoCode);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.GetFullErrorMessage());
                    return View(promoCode);
                }

                return RedirectToAction("Index");
            }

            return View(promoCode);
        }

        public ActionResult CreateMultiple()
        {
            return View(new PromoCode
            {
                NumberToCreate = 11
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMultiple(PromoCode promoCode)
        {
            var codes = new List<PromoCode>();

            Validate(promoCode);

            if (ModelState.IsValid)
            {
                for (int i = 0; i < promoCode.NumberToCreate; i++)
                {
                    var newPromoCode = new PromoCode
                    {
                        MembershipOptionId = promoCode.MembershipOptionId,
                        Discount = promoCode.Discount,
                        TotalPrice = promoCode.TotalPrice
                    };
                    newPromoCode.Name = newPromoCode.Code;

                    codes.Add(newPromoCode);
                }

                Repository.CreateBatch(codes);

                return RedirectToAction("Index");
            }

            return View(promoCode);
        }

        private void Validate(PromoCode promoCode)
        {
            var membershipOption = _membershipOptionsRepository.Find(promoCode.MembershipOptionId);
            if (membershipOption.SubscriptionType == MembershipOption.ESubscriptionType.Free)
            {
                ModelState.AddModelError(nameof(PromoCode.MembershipOptionId), "Cannot create promocode for free membership");
            }

            if (membershipOption.Price == promoCode.TotalPrice)
            {
                ModelState.AddModelError(nameof(PromoCode.TotalPrice), "Total price must be discounted.");
            }
        }
    }
}