using K9.WebApplication.Models;
using System;
using System.Web.Mvc;
using K9.Base.DataAccessLayer.Enums;
using K9.WebApplication.ViewModels;

namespace K9.WebApplication.Controllers
{
    public partial class NineStarKiController
    {

        [Route("api/calculate")]
        public JsonResult CalculateNineStarKiAjax(DateTime dateOfBirth, EGender gender)
        {
            var model = new NineStarKiModel(new PersonModel
            {
                DateOfBirth = dateOfBirth,
                Gender = gender
            })
            {
                SelectedDate = DateTime.Today
            };

            var selectedDate = model.SelectedDate;
            
            model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false, selectedDate);
            model.SelectedDate = selectedDate;
            
            var result = new NineStarKiAjaxModel
            {
                MainEnergy = model.MainEnergy.Energy,
                CharacterEnergy = model.CharacterEnergy.Energy,
                SurfaceEnergy = model.SurfaceEnergy.Energy,
                MonthlyCycleEnergy = model.MonthlyCycleEnergy.Energy,
                YearlyCycleEnergy = model.YearlyCycleEnergy.Energy
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}

