using K9.DataAccessLayer.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using System;

namespace K9.WebApplication.Services
{
    public interface INineStarKiService
    {
        NineStarKiModel CalculateNineStarKi(DateTime dateOfBirth, EGender gender = EGender.Male);
        NineStarKiModel CalculateNineStarKi(PersonModel personModel);
        NineStarKiSummaryViewModel GetNineStarKiSummaryViewModel();
    }
}