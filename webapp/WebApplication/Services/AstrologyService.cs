﻿using K9.Globalisation;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using System;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public class AstrologyService : BaseService, IAstrologyService
    {
        private readonly IAstronomyService _astronomyService;

        public AstrologyService(INineStarKiBasePackage my, IAstronomyService astronomyService) : base(my)
        {
            _astronomyService = astronomyService;
        }

        public MoonPhase GetMoonPhase(DateTime selectedDateTime, string userTimeZoneId, bool includeLunarDay = false, NineStarKiEnergy energy = null)
        {
            var moonPhase = _astronomyService.GetMoonPhase(selectedDateTime, userTimeZoneId, includeLunarDay);

            moonPhase.LunarDayTitle = GetLunarDayTitle(moonPhase.LunarDay);
            moonPhase.LunarDayDescription = TemplateParser.Parse(GetLunarDayDescription(moonPhase.LunarDay), new { Illumination = $"({moonPhase.IlluminationDisplay})" });

            if (energy != null)
            {
                moonPhase.YinYangComfortDescription = GetYinYangComfortDescription(moonPhase, energy);
            }

            return moonPhase;
        }

        private static readonly Dictionary<int, string> _lunarDayTitles =
            new Dictionary<int, string>
            {
                {1, Dictionary.lunar_day_1_title},
                {2, Dictionary.lunar_day_2_title},
                {3, Dictionary.lunar_day_3_title},
                {4, Dictionary.lunar_day_4_title},
                {5, Dictionary.lunar_day_5_title},
                {6, Dictionary.lunar_day_6_title},
                {7, Dictionary.lunar_day_7_title},
                {8, Dictionary.lunar_day_8_title},
                {9, Dictionary.lunar_day_9_title},
                {10, Dictionary.lunar_day_10_title},
                {11, Dictionary.lunar_day_11_title},
                {12, Dictionary.lunar_day_12_title},
                {13, Dictionary.lunar_day_13_title},
                {14, Dictionary.lunar_day_14_title},
                {15, Dictionary.lunar_day_15_title},
                {16, Dictionary.lunar_day_16_title},
                {17, Dictionary.lunar_day_17_title},
                {18, Dictionary.lunar_day_18_title},
                {19, Dictionary.lunar_day_19_title},
                {20, Dictionary.lunar_day_20_title},
                {21, Dictionary.lunar_day_21_title},
                {22, Dictionary.lunar_day_22_title},
                {23, Dictionary.lunar_day_23_title},
                {24, Dictionary.lunar_day_24_title},
                {25, Dictionary.lunar_day_25_title},
                {26, Dictionary.lunar_day_26_title},
                {27, Dictionary.lunar_day_27_title},
                {28, Dictionary.lunar_day_28_title},
                {29, Dictionary.lunar_day_29_title},
                {30, Dictionary.lunar_day_30_title}

            };

        private static readonly Dictionary<int, string> _lunarDayDescriptions =
            new Dictionary<int, string>
            {
                {1, Dictionary.lunar_day_1_description},
                {2, Dictionary.lunar_day_2_description},
                {3, Dictionary.lunar_day_3_description},
                {4, Dictionary.lunar_day_4_description},
                {5, Dictionary.lunar_day_5_description},
                {6, Dictionary.lunar_day_6_description},
                {7, Dictionary.lunar_day_7_description},
                {8, Dictionary.lunar_day_8_description},
                {9, Dictionary.lunar_day_9_description},
                {10, Dictionary.lunar_day_10_description},
                {11, Dictionary.lunar_day_11_description},
                {12, Dictionary.lunar_day_12_description},
                {13, Dictionary.lunar_day_13_description},
                {14, Dictionary.lunar_day_14_description},
                {15, Dictionary.lunar_day_15_description},
                {16, Dictionary.lunar_day_16_description},
                {17, Dictionary.lunar_day_17_description},
                {18, Dictionary.lunar_day_18_description},
                {19, Dictionary.lunar_day_19_description},
                {20, Dictionary.lunar_day_20_description},
                {21, Dictionary.lunar_day_21_description},
                {22, Dictionary.lunar_day_22_description},
                {23, Dictionary.lunar_day_23_description},
                {24, Dictionary.lunar_day_24_description},
                {25, Dictionary.lunar_day_25_description},
                {26, Dictionary.lunar_day_26_description},
                {27, Dictionary.lunar_day_27_description},
                {28, Dictionary.lunar_day_28_description},
                {29, Dictionary.lunar_day_29_description},
                {30, Dictionary.lunar_day_30_description}
            };

        private static readonly Dictionary<ENineStarKiYinYangExpansion, string> _lunarFullMoonExpansionDescriptions =
            new Dictionary<ENineStarKiYinYangExpansion, string>
            {
                { ENineStarKiYinYangExpansion.YinExpanding, Dictionary.full_moon_yin_expanding },
                { ENineStarKiYinYangExpansion.YangContracting, Dictionary.full_moon_yang_conracting},
                { ENineStarKiYinYangExpansion.Balanced, Dictionary.full_moon_balanced }
            };

        private static readonly Dictionary<ENineStarKiYinYangExpansion, string> _lunarNewMoonExpansionDescriptions =
            new Dictionary<ENineStarKiYinYangExpansion, string>
            {
                { ENineStarKiYinYangExpansion.YinExpanding, Dictionary.new_moon_yin_expanding },
                { ENineStarKiYinYangExpansion.YangContracting, Dictionary.new_moon_yang_conracting},
                { ENineStarKiYinYangExpansion.Balanced, Dictionary.new_moon_balanced }
            };

        private string GetLunarDayTitle(int day) => _lunarDayTitles.TryGetValue(day, out var desc) ? desc : string.Empty;

        private string GetLunarDayDescription(int day) => _lunarDayDescriptions.TryGetValue(day, out var desc) ? desc : string.Empty;

        private string GetFullMoonExpansionDescription(ENineStarKiYinYangExpansion expansion) => _lunarFullMoonExpansionDescriptions.TryGetValue(expansion, out var desc) ? desc : string.Empty;

        private string GetNewMoonExpansionDescription(ENineStarKiYinYangExpansion expansion) => _lunarNewMoonExpansionDescriptions.TryGetValue(expansion, out var desc) ? desc : string.Empty;

        private string GetYinYangComfortDescription(MoonPhase moonPhase, NineStarKiEnergy energy)
        {
            if (moonPhase.IlluminationPercentage >= 95) // Full moon
            {
                return GetFullMoonExpansionDescription(energy.YinYangExpansion);
            }
            else if (moonPhase.IlluminationPercentage <= 5) // New moon
            {
                return GetNewMoonExpansionDescription(energy.YinYangExpansion);
            }

            return string.Empty;
        }


    }

}