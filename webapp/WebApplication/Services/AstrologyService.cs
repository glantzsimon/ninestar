using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using System;

namespace K9.WebApplication.Services
{
    public class AstrologyService : BaseService, IAstrologyService
    {
        private readonly ISwissEphemerisService _swissEphemerisService;

        public AstrologyService(INineStarKiBasePackage my, ISwissEphemerisService swissEphemerisService) : base(my)
        {
            _swissEphemerisService = swissEphemerisService;
        }

        public MoonPhase GetMoonPhase(DateTime selectedDateTime, string userTimeZoneId)
        {
            var moonPhase = _swissEphemerisService.GetMoonPhase(selectedDateTime, userTimeZoneId);

            return moonPhase;
        }
    }

}