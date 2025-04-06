using System;
using System.Collections.Generic;
using System.Linq;
using K9.Globalisation;

namespace K9.WebApplication.Models
{
    public class MoonPhase
    {
        private static List<(double Illum, string File)> waxingImages = new List<(double Illum, string File)>()
        {
            (0, "0.png"),
            (0.4, "wax-0.4.png"),
            (3.3, "wax-3.3.png"),
            (9, "wax-9.png"),
            (16.8, "wax-16.8.png"),
            (26.1, "wax-26.1.png"),
            (36.3, "wax-36.3.png"),
            (46.7, "wax-46.7.png"),
            (57, "wax-57.png"),
            (66.7, "wax-66.7.png"),
            (75.5, "wax-75.5.png"),
            (83.2, "wax-83.2.png"),
            (89.7, "wax-89.7.png"),
            (95, "wax-95.png"),
            (98, "wax-98.png"),
            (99.7, "wax-99.7.png"),
            (100, "100.png")
        };

        private static List<(double Illum, string File)> waningImages = new List<(double Illum, string File)>()
        {
            (0, "0.png"),
            (2.2, "wane-2.2.png"),
            (7.2, "wane-7.2.png"),
            (14.5, "wane-14.5.png"),
            (23.6, "wane-23.6.png"),
            (33.7, "wane-33.7.png"),
            (44.3, "wane-44.3.png"),
            (54.7, "wane-54.7.png"),
            (64.7, "wane-64.7.png"),
            (73.8, "wane-73.8.png"),
            (81.9, "wane-81.9.png"),
            (88.6, "wane-88.6.png"),
            (97.6, "wane-97.6.png"),
            (99.6, "wane-99.6.png"),
            (99.8, "wane-99.8.png"),
            (100, "100.png")
        };

        public MoonPhase(double illuminationPercentage, bool isWaxing)
        {
            IlluminationPercentage = illuminationPercentage;
            IsWaxing = isWaxing;
        }

        public double IlluminationPercentage { get; }

        public bool IsWaxing { get; }

        public string IlluminationDisplay => $"{IlluminationPercentage:F1}%";

        public string GetMoonPhaseName()
        {
            double illum = IlluminationPercentage;

            if (illum == 0)
                return Dictionary.NewMoon;
            if (illum == 100)
                return Dictionary.FullMoon;

            if (IsWaxing)
            {
                if (illum < 50)
                    return Dictionary.WaxingCrescent;
                else if (illum == 50)
                    return Dictionary.FirstQuarter;
                else
                    return Dictionary.WaxingGibbous;
            }
            else
            {
                if (illum > 50)
                    return Dictionary.WaningGibbous;
                else if (illum == 50)
                    return Dictionary.LastQuarter;
                else
                    return Dictionary.WaningCrescent;
            }
        }

        public string GetMoonPhaseImageFile()
        {
            // Select the correct list based on IsWaxing.
            var candidateImages = IsWaxing ? waxingImages : waningImages;

            // Ensure the list is sorted in ascending order by Illum.
            candidateImages = candidateImages.OrderBy(x => x.Illum).ToList();

            // If the illumination is lower than or equal to the minimum, return the minimum image.
            if (IlluminationPercentage <= candidateImages.First().Illum)
            {
                return candidateImages.First().File;
            }

            // If the illumination is greater than or equal to the maximum, return the maximum image.
            if (IlluminationPercentage >= candidateImages.Last().Illum)
            {
                return candidateImages.Last().File;
            }

            // Otherwise, pick the image with the closest Illum value.
            var bestMatch = candidateImages
                .OrderBy(x => Math.Abs(x.Illum - IlluminationPercentage))
                .First();

            return bestMatch.File;
        }

        public int LunarDay { get; set; }

        public string LunarDayDescription { get; set; }

    }
}