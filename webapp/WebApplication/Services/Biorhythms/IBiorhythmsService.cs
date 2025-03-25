using System;
using K9.WebApplication.Models;

namespace K9.WebApplication.Services
{
    public interface IBiorhythmsService
    {
        BioRhythmsModel Calculate(NineStarKiModel nineStarKiModel, DateTime date);
    }
}