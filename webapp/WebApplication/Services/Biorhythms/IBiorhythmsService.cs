using System;
using K9.WebApplication.Models;

namespace K9.WebApplication.Services
{
    public interface IBiorhythmsService
    {
        BioRhythmsModel Calculate(PersonModel personModel, DateTime date);
    }
}