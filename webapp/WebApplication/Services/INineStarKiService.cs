using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using System;
using K9.WebApplication.ViewModels;

namespace K9.WebApplication.Services
{
    public interface INineStarKiService
    {
        NineStarKiModel CalculateNineStarKi(DateTime dateOfBirth, EGender gender = EGender.Male);
        NineStarKiModel CalculateNineStarKi(PersonModel personModel);
        NineStarKiSummaryViewModel GetNineStarKiSummaryViewModel();
    }
}