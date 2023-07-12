using K9.WebApplication.Enums;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Models
{
    public static class NineStarKiBiorhythmsExtension
    {
        public static void AddFactor(this List<double> list, double factor, int weight)
        {
            for (int i = 0; i < weight; i++)
            {
                list.Add(factor);
            }
        }
    }

    public class NineStarKiBiorhythmsModel
    {
        private const int MainEnergyWeight = 5;
        private const int EmotionalEnergyWeight = 3;
        private const int SurfaceEnergyWeight = 2;
        private const int YearlyCycleWeight = 12;
        private const int MonthlyCycleWeight = 8;

        public double StabilityFactor { get; set; }
        public double IntellectualFactor { get; set; }
        public double EmotionalFactor { get; set; }
        public double PhysicalFactor { get; set; }
        public double SpiritualFactor { get; set; }

        public NineStarKiModel NineStarKiModel { get; }

        public NineStarKiBiorhythmsModel(NineStarKiModel nineStarKiModel)
        {
            NineStarKiModel = nineStarKiModel;
            CalculateFactors();
        }

        private void CalculateFactors()
        {
            var stabilityFactors = GetStabilityFactors();

            StabilityFactor = stabilityFactors.Average();
            IntellectualFactor = GetAllFactors(EBiorhythmFactor.Intellectual).Average();
            EmotionalFactor = GetAllFactors(EBiorhythmFactor.Emotional).Average();
            PhysicalFactor = GetAllFactors(EBiorhythmFactor.Physical).Average();
            SpiritualFactor = GetAllFactors(EBiorhythmFactor.Spiritual).Average();
        }

        private List<double> GetStabilityFactors()
        {
            var factors = new List<double>();

            var mainEnergyFactors = GetPersonalFactors(NineStarKiModel.MainEnergy.Energy);
            var emotionalEnergyFactors = GetPersonalFactors(NineStarKiModel.CharacterEnergy.Energy);
            var surfaceEnergyFactors = GetPersonalFactors(NineStarKiModel.SurfaceEnergy.Energy);
            var yearlyCycleFactors = GetCycleFactors(NineStarKiModel.YearlyCycleEnergy.Energy);
            var monthlyCycleFactors = GetCycleFactors(NineStarKiModel.MonthlyCycleEnergy.Energy);

            factors.AddFactor(mainEnergyFactors.StabilityFactor, MainEnergyWeight);
            factors.AddFactor(emotionalEnergyFactors.StabilityFactor, EmotionalEnergyWeight);
            factors.AddFactor(surfaceEnergyFactors.StabilityFactor, SurfaceEnergyWeight);
            factors.AddFactor(yearlyCycleFactors.StabilityFactor, YearlyCycleWeight);
            factors.AddFactor(monthlyCycleFactors.StabilityFactor, MonthlyCycleWeight);

            return factors;
        }

        private List<double> GetAllFactors(EBiorhythmFactor biorhythmFactor)
        {
            var factors = new List<double>();

            var mainEnergyFactors = GetPersonalFactors(NineStarKiModel.MainEnergy.Energy);
            var characterEnergyFactors = GetPersonalFactors(NineStarKiModel.CharacterEnergy.Energy);
            var surfaceEnergyFactors = GetPersonalFactors(NineStarKiModel.SurfaceEnergy.Energy);
            var yearlyCycleFactors = GetCycleFactors(NineStarKiModel.YearlyCycleEnergy.Energy);
            var monthlyCycleFactors = GetCycleFactors(NineStarKiModel.MonthlyCycleEnergy.Energy);

            switch (biorhythmFactor)
            {
                case EBiorhythmFactor.Intellectual:
                    factors.AddFactor(mainEnergyFactors.IntellectualFactor, MainEnergyWeight);
                    factors.AddFactor(characterEnergyFactors.IntellectualFactor, EmotionalEnergyWeight);
                    factors.AddFactor(surfaceEnergyFactors.IntellectualFactor, SurfaceEnergyWeight);
                    factors.AddFactor(yearlyCycleFactors.IntellectualFactor, YearlyCycleWeight);
                    factors.AddFactor(monthlyCycleFactors.IntellectualFactor, MonthlyCycleWeight);
                    break;

                case EBiorhythmFactor.Emotional:
                    factors.AddFactor(mainEnergyFactors.EmotionalFactor, MainEnergyWeight);
                    factors.AddFactor(characterEnergyFactors.EmotionalFactor, EmotionalEnergyWeight);
                    factors.AddFactor(surfaceEnergyFactors.EmotionalFactor, SurfaceEnergyWeight);
                    factors.AddFactor(yearlyCycleFactors.EmotionalFactor, YearlyCycleWeight);
                    factors.AddFactor(monthlyCycleFactors.EmotionalFactor, MonthlyCycleWeight);
                    break;

                case EBiorhythmFactor.Physical:
                    factors.AddFactor(mainEnergyFactors.PhysicalFactor, MainEnergyWeight);
                    factors.AddFactor(characterEnergyFactors.PhysicalFactor, EmotionalEnergyWeight);
                    factors.AddFactor(surfaceEnergyFactors.PhysicalFactor, SurfaceEnergyWeight);
                    factors.AddFactor(yearlyCycleFactors.PhysicalFactor, YearlyCycleWeight);
                    factors.AddFactor(monthlyCycleFactors.PhysicalFactor, MonthlyCycleWeight);
                    break;

                case EBiorhythmFactor.Spiritual:
                    factors.AddFactor(mainEnergyFactors.SpiritualFactor, MainEnergyWeight);
                    factors.AddFactor(characterEnergyFactors.SpiritualFactor, EmotionalEnergyWeight);
                    factors.AddFactor(surfaceEnergyFactors.SpiritualFactor, SurfaceEnergyWeight);
                    factors.AddFactor(yearlyCycleFactors.SpiritualFactor, YearlyCycleWeight);
                    factors.AddFactor(monthlyCycleFactors.SpiritualFactor, MonthlyCycleWeight);
                    break;
            }

            return factors;
        }

        private NineStarKiBiorhythmFactors GetPersonalFactors(ENineStarKiEnergy energy)
        {
            var factors = new NineStarKiBiorhythmFactors();

            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    factors.EmotionalFactor = 1.3;
                    factors.IntellectualFactor = 1.3;
                    factors.PhysicalFactor = 0.9;
                    factors.SpiritualFactor = 1.2;
                    factors.StabilityFactor = 0.9;
                    break;

                case ENineStarKiEnergy.Soil:
                    factors.EmotionalFactor = 1.1;
                    factors.IntellectualFactor = 0.7;
                    factors.PhysicalFactor = 1.1;
                    factors.SpiritualFactor = 0.7;
                    factors.StabilityFactor = 1.2;
                    break;

                case ENineStarKiEnergy.Thunder:
                    factors.EmotionalFactor = 1.1;
                    factors.IntellectualFactor = 1.1;
                    factors.PhysicalFactor = 1.3;
                    factors.SpiritualFactor = 1.3;
                    factors.StabilityFactor = 0.7;
                    break;

                case ENineStarKiEnergy.Wind:
                    factors.EmotionalFactor = 1.3;
                    factors.IntellectualFactor = 1.2;
                    factors.PhysicalFactor = 0.8;
                    factors.SpiritualFactor = 1;
                    factors.StabilityFactor = 0.7;
                    break;

                case ENineStarKiEnergy.CoreEarth:
                    factors.EmotionalFactor = 1;
                    factors.IntellectualFactor = 1;
                    factors.PhysicalFactor = 1.3;
                    factors.SpiritualFactor = 0.9;
                    factors.StabilityFactor = 1;
                    break;

                case ENineStarKiEnergy.Heaven:
                    factors.EmotionalFactor = 1.2;
                    factors.IntellectualFactor = 1.2;
                    factors.PhysicalFactor = 1.1;
                    factors.SpiritualFactor = 1.2;
                    factors.StabilityFactor = 1;
                    break;

                case ENineStarKiEnergy.Lake:
                    factors.EmotionalFactor = 1.1;
                    factors.IntellectualFactor = 1.3;
                    factors.PhysicalFactor = 1.1;
                    factors.SpiritualFactor = 0.8;
                    factors.StabilityFactor = 1;
                    break;

                case ENineStarKiEnergy.Mountain:
                    factors.EmotionalFactor = 0.8;
                    factors.IntellectualFactor = 1.2;
                    factors.PhysicalFactor = 1.3;
                    factors.SpiritualFactor = 1.2;
                    factors.StabilityFactor = 1;
                    break;

                case ENineStarKiEnergy.Fire:
                    factors.EmotionalFactor = 1.3;
                    factors.IntellectualFactor = 0.8;
                    factors.PhysicalFactor = 0.8;
                    factors.SpiritualFactor = 1.1;
                    factors.StabilityFactor = 0.9;
                    break;
            }

            return factors;
        }

        private NineStarKiBiorhythmFactors GetCycleFactors(ENineStarKiEnergy energy)
        {
            var factors = new NineStarKiBiorhythmFactors();

            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    factors.EmotionalFactor = 0.7;
                    factors.IntellectualFactor = 0.7;
                    factors.PhysicalFactor = 0.7;
                    factors.SpiritualFactor = 1.2;
                    factors.StabilityFactor = 1;
                    break;

                case ENineStarKiEnergy.Soil:
                    factors.EmotionalFactor = 0.9;
                    factors.IntellectualFactor = 0.8;
                    factors.PhysicalFactor = 1;
                    factors.SpiritualFactor = 0.8;
                    factors.StabilityFactor = 1.2;
                    break;

                case ENineStarKiEnergy.Thunder:
                    factors.EmotionalFactor = 1.2;
                    factors.IntellectualFactor = 1.2;
                    factors.PhysicalFactor = 1.3;
                    factors.SpiritualFactor = 1.3;
                    factors.StabilityFactor = 0.9;
                    break;

                case ENineStarKiEnergy.Wind:
                    factors.EmotionalFactor = 1.3;
                    factors.IntellectualFactor = 1.3;
                    factors.PhysicalFactor = 1.1;
                    factors.SpiritualFactor = 1;
                    factors.StabilityFactor = 0.8;
                    break;

                case ENineStarKiEnergy.CoreEarth:
                    factors.EmotionalFactor = 1;
                    factors.IntellectualFactor = 1;
                    factors.PhysicalFactor = 1.3;
                    factors.SpiritualFactor = 1;
                    factors.StabilityFactor = 0.7;
                    break;

                case ENineStarKiEnergy.Heaven:
                    factors.EmotionalFactor = 1.3;
                    factors.IntellectualFactor = 1.2;
                    factors.PhysicalFactor = 1.3;
                    factors.SpiritualFactor = 1.3;
                    factors.StabilityFactor = 1.2;
                    break;

                case ENineStarKiEnergy.Lake:
                    factors.EmotionalFactor = 1.2;
                    factors.IntellectualFactor = 1.3;
                    factors.PhysicalFactor = 1.1;
                    factors.SpiritualFactor = 1.2;
                    factors.StabilityFactor = 1.3;
                    break;

                case ENineStarKiEnergy.Mountain:
                    factors.EmotionalFactor = 0.9;
                    factors.IntellectualFactor = 0.9;
                    factors.PhysicalFactor = 1.2;
                    factors.SpiritualFactor = 1.2;
                    factors.StabilityFactor = 1.3;
                    break;

                case ENineStarKiEnergy.Fire:
                    factors.EmotionalFactor = 1.3;
                    factors.IntellectualFactor = 0.9;
                    factors.PhysicalFactor = 1.1;
                    factors.SpiritualFactor = 1.2;
                    factors.StabilityFactor = 1;
                    break;
            }

            return factors;
        }
    }
}