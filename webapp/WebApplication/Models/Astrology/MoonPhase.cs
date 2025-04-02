using System;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Models
{
    public class MoonPhase
    {
        private static List<(double Illum, string File)> waxingImages = new List<(double Illum, string File)>()
        {
            (0.4, "wax-0.4.jpg"),
            (16.8, "wax-16.8.jpg"),
            (23.6, "wax-23.6.jpg"),
            (36.7, "wax-36.7.jpg"),
            (43.1, "wax-43.1.jpg"),
            (46.7, "wax-46.7.jpg"),
            (57.9, "wax-57.9.jpg"),
            (63.2, "wax-63.2.jpg"),
            (75.3, "wax-75.3.jpg"),
            (85.7, "wax-85.7.jpg"),
            (89.5, "wax-89.5.jpg"),
            (95.3, "wax-95.3.jpg"),
            (99.7, "wax-99.7.jpg")
        };

        private static List<(double Illum, string File)> waningImages = new List<(double Illum, string File)>()
        {
            (2.2, "wane-2.2.jpg"),
            (14.5, "wane-14.5.jpg"),
            (23.6, "wane-23.6.jpg"),
            (44.3, "wane-44.3.jpg"),
            (54.7, "wane-54.7.jpg"),
            (62.1, "wane-62.1.jpg"),
            (73.8, "wane-73.8.jpg"),
            (79.5, "wane-79.5.jpg"),
            (87.9, "wane-87.9.jpg"),
            (93.7, "wane-93.7.jpg"),
            (98.3, "wane-98.3.jpg"),
            (99.7, "wane-99.7.jpg")
        };

        public double IlluminationPercentage { get; }
        public bool IsWaxing { get; }

        public MoonPhase(double illuminationPercentage, bool isWaxing)
        {
            IlluminationPercentage = illuminationPercentage;
            IsWaxing = isWaxing;
        }

        public string GetMoonPhaseImageFile()
        {
            // Select the correct list based on IsWaxing.
            var candidateImages = IsWaxing ? waxingImages : waningImages;


            var bestMatch = candidateImages
                .OrderBy(x => Math.Abs(x.Illum - IlluminationPercentage))
                .FirstOrDefault();

            return bestMatch.File;
        }
    }
}