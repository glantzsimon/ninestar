using K9.WebApplication.Models;
using System;

namespace K9.WebApplication.Services
{
    public interface IAstrologyService : IBaseService
    {
        MoonPhase GetMoonPhase(DateTime selectedDateTime, string userTimeZoneId);
    }
}