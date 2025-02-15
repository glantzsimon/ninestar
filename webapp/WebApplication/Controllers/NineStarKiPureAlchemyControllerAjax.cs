using K9.Base.DataAccessLayer.Enums;
using K9.Globalisation;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using System;
using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public partial class NineStarKiController : BaseNineStarKiController
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
                YearlyCycleEnergy = model.YearlyCycleEnergy.Energy,
                HealthAdvice = GetHealthAdvice(model.MainEnergy.Energy)
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private string GetHealthAdvice(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_health;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_health;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_health;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_health;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_health;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_health;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_health;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_health;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_health;
            }

            return string.Empty;
        }
    }
}