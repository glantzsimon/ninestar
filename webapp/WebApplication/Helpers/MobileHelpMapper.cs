using K9.SharedLibrary.Helpers;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using System.Collections.Generic;
using System.Globalization;
using Dictionary = K9.Globalisation.Dictionary;

namespace K9.WebApplication.Helpers
{
    // Resource-authored explanations only. Never render Razor panels or append
    // personal readings here. Callers retain their existing access checks.
    public static class MobileHelpMapper
    {
        public static MobileHelpText Text(string title, string htmlBody) => new MobileHelpText
        {
            Title = title,
            HtmlBody = htmlBody,
            Culture = (Dictionary.Culture ?? CultureInfo.CurrentUICulture).Name
        };

        public static Dictionary<string, MobileHelpText> Chart(bool includeRelationships, bool includeMemberHelp)
        {
            var help = new Dictionary<string, MobileHelpText>
            {
                { "overview", Text(Dictionary.AboutPersonalChart, Dictionary.personal_chart) },
                { "core", Text(Dictionary.AboutConstitution, Dictionary.MainEnergyDescription) },
                { "character", Text(Dictionary.AboutCharacterNature, Dictionary.CharacterEnergyDescription) },
                { "socialExpression", Text(Dictionary.AboutSocialExpression, Dictionary.SurfaceEnergyDescription) },
                { "dayStar", Text(Dictionary.AboutDayStar, Dictionary.DayStarDescription) },
                { "generation", Text(Dictionary.AboutGenerationalKi, Dictionary.GenerationalEnergyDescription) },
                { "epoch", Text(Dictionary.AboutEpochKi, Dictionary.EpochEnergyDescription) }
            };
            if (includeRelationships)
                help.Add("relationships", Text(Dictionary.AboutRelationshipsAndIntimacy, Dictionary.relationships_description));
            // This explanation is inside the character tab's paywall on the web.
            if (includeMemberHelp)
                help.Add("adultChildRelationship", Text(Dictionary.AboutAdultChildRelationship, Dictionary.AdultChildRelationship));
            return help;
        }

        public static Dictionary<string, MobileHelpText> PredictionIntroductions() => new Dictionary<string, MobileHelpText>
        {
            { "overview", Text(Dictionary.PredictionsOverview, Dictionary.PredictionsIntroduction) },
            { "globalKi", Text(Dictionary.AboutGlobalKi, Dictionary.GlobalKiDescription) }
        };

        public static Dictionary<string, MobileHelpText> Predictions() => new Dictionary<string, MobileHelpText>(PredictionIntroductions())
        {
            { "day", Text(Dictionary.AboutDailyPredictions, Dictionary.day_description) },
            { "month", Text(Dictionary.AboutMonthlyPredictions, Dictionary.month_description) },
            { "year", Text(Dictionary.AboutYearlyPredictions, Dictionary.year_description) },
            { "generation", Text(Dictionary.AboutNineYearPredictions, Dictionary.generation_description) },
            { "epoch", Text(Dictionary.AboutEightyOneYearPredictions, Dictionary.epoch_description) },
            { "globalKiWarning", Text(Dictionary.AboutGlobalKi, Dictionary.globalki_display_warning) },
            { "moreOnCycles", Text(Dictionary.MoreOnCycles, Dictionary.holograhpic_spirals) }
        };

        public static Dictionary<string, MobileHelpText> Calendar() => new Dictionary<string, MobileHelpText>
        {
            { "overview", Text(Dictionary.AboutPersonalCalendar, Dictionary.PersonalCalendarHelp) },
            { "subscription", Text(Dictionary.AboutPersonalCalendar, Dictionary.CalendarSubscriptionHelp) }
        };

        public static Dictionary<string, MobileHelpText> Compatibility() => new Dictionary<string, MobileHelpText>
        {
            { "overview", Text(Dictionary.AboutCompatibilityCalculator, Dictionary.compatibility_subtitle) }
        };

        public static Dictionary<string, MobileHelpText> Numerology() => new Dictionary<string, MobileHelpText>
        {
            { "overview", Text(Dictionary.WhatIsKarmicNumerology, Dictionary.karmic_numerology) }
        };

        public static Dictionary<string, MobileHelpText> Biorhythms() => new Dictionary<string, MobileHelpText>
        {
            { "overview", Text(Dictionary.WhatAreBiorhythms, Dictionary.biorhythms) }
        };

        public static Dictionary<string, MobileHelpText> IChing() => new Dictionary<string, MobileHelpText>
        {
            { "overview", Text(Dictionary.AboutTheIChing, Dictionary.i_ching_info) }
        };

        public static Dictionary<string, MobileHelpText> KnowledgeBase(bool includeMemberHelp, string knowledgeBaseUrl)
        {
            var help = new Dictionary<string, MobileHelpText>
            {
                { "origins", Text(Dictionary.OriginsTitle, Dictionary.origins) },
                { "lightAndDark", Text(Dictionary.CyclesOfLightAndDark, Dictionary.cycles) },
                { "yinYang", Text(Dictionary.YinAndYang, Dictionary.yinyang) },
                { "modalities", Text(Dictionary.ThreeModalitiesTitle, Dictionary.modalities_description) },
                { "mainEnergies", Text(Dictionary.NineMainEnergiesTitle, Dictionary.main_energies_short_description) },
                { "characterEnergies", Text(Dictionary.NineCharacterEnergiesTitle, Dictionary.character_energies_short_description) },
                { "year", Text(Dictionary.YearlyCycles, Dictionary.year_description) },
                { "month", Text(Dictionary.MonthlyCycles, Dictionary.month_description) },
                { "personalChart", Text(Dictionary.PersonalChart, Dictionary.personal_chart) },
                { "relationships", Text(Dictionary.RelationshipsTitle, Dictionary.relationships_description) }
            };
            // Match the website's Free-membership PayWallContent calls. Existing
            // reference arrays are left unchanged; these new articles are gated.
            if (includeMemberHelp)
            {
                var images = MediaService.BaseImagesPath;
                help.Add("elements", Text(Dictionary.FiveElementsTitle,
                    TemplateParser.Parse(Dictionary.five_elements, new { BaseImagesPath = images })));
                help.Add("fire", Text(Dictionary.FireElement, Dictionary.fire_element));
                help.Add("earth", Text(Dictionary.EarthElement, Dictionary.earth_element));
                help.Add("metal", Text(Dictionary.MetalElement, Dictionary.metal_element));
                help.Add("water", Text(Dictionary.WaterElement, Dictionary.water_element));
                help.Add("tree", Text(Dictionary.TreeElement, Dictionary.tree_element));
                help.Add("transformations", Text(Dictionary.FiveTransformationsTitle,
                    TemplateParser.Parse(Dictionary.five_transformations, new
                    {
                        BaseImagesPath = images,
                        WaterTree = Dictionary.water_tree, TreeFire = Dictionary.tree_fire,
                        FireEarth = Dictionary.fire_earth, EarthMetal = Dictionary.earth_metal,
                        MetalWater = Dictionary.metal_water, WaterFire = Dictionary.water_fire,
                        TreeEarth = Dictionary.tree_earth, FireMetal = Dictionary.fire_metal,
                        EarthWater = Dictionary.earth_water, MetalTree = Dictionary.metal_tree
                    })));
                help.Add("sexuality", Text(Dictionary.SexualityTitle,
                    TemplateParser.Parse(Dictionary.sexuality, new
                    {
                        AboutTitle = Dictionary.AboutNineStarKi,
                        AboutLink = knowledgeBaseUrl + "#yinyang",
                        ChartLink = knowledgeBaseUrl + "#chart"
                    })));
            }
            return help;
        }
    }
}
