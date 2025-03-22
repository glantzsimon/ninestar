using K9.WebApplication.Enums;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Models
{
    public static class NineStarKiBiorhythmsExtension
    {
        public static void AddFactor(this List<BiorhythmFactor> list, EBiorhythm biorhythm, double factor, int weight)
        {
            for (int i = 0; i < weight; i++)
            {
                list.Add(new BiorhythmFactor(biorhythm, factor));
            }
        }

        public static void AddStabilityFactor(this List<double> list, double factor, int weight, double influence)
        {
            double difference = factor > 0.7 ? factor - 0.7 : 0;
            double modification = (difference * influence);

            factor -= modification;

            for (int i = 0; i < weight; i++)
            {
                list.Add(factor);
            }
        }

        public static double GetStabilityFactor(this List<BiorhythmFactor> list)
        {
            return list.FirstOrDefault(e => e.Biorhythm == EBiorhythm.Stability).Value;
        }
    }

    public class NineStarKiBiorhythmsFactors
    {
        private const int MainEnergyWeight = 5;
        private const int CharacterEnergyWeight = 3;
        private const int SurfaceEnergyWeight = 2;
        private const int YearlyCycleWeight = 8;
        private const int MonthlyCycleWeight = 5;

        private const double StabilityInfluence = 0.66;

        private List<BiorhythmFactor> PersonalFactors { get; }
        private List<BiorhythmFactor> CycleFactors { get; }
        private List<BiorhythmFactor> AllFactors { get; }
        private List<double> StabilityFactors { get; }

        public double StabilityFactor { get; set; }
        public NineStarKiModel NineStarKiModel { get; }

        public NineStarKiBiorhythmsFactors(NineStarKiModel nineStarKiModel)
        {
            NineStarKiModel = nineStarKiModel;
            PersonalFactors = GetPersonalFactors();
            CycleFactors = GetCycleFactors();
            AllFactors = GetAllFactors();
            StabilityFactors = GetStabilityFactors();

            StabilityFactor = StabilityFactors.Average();
        }

        public double GetFactor(EBiorhythm biorhythm)
        {
            return AllFactors.Where(e => e.Biorhythm == biorhythm).Average(e => e.Value);
        }
        
        private List<double> GetStabilityFactors()
        {
            var factors = new List<double>();

            var mainEnergyFactors = PersonalFactors.Where(e => e.Energy == NineStarKiModel.MainEnergy.Energy).ToList();
            var emotionalEnergyFactors = PersonalFactors.Where(e => e.Energy == NineStarKiModel.CharacterEnergy.Energy).ToList(); 
            var surfaceEnergyFactors = PersonalFactors.Where(e => e.Energy == NineStarKiModel.SurfaceEnergy.Energy).ToList(); 
            var yearlyCycleFactors = CycleFactors.Where(e => e.Energy == NineStarKiModel.PersonalHousesOccupiedEnergies.Year.Energy).ToList(); 
            var monthlyCycleFactors = CycleFactors.Where(e => e.Energy == NineStarKiModel.PersonalHousesOccupiedEnergies.Month.Energy).ToList(); 

            factors.AddStabilityFactor(mainEnergyFactors.GetStabilityFactor(), MainEnergyWeight, StabilityInfluence);
            factors.AddStabilityFactor(emotionalEnergyFactors.GetStabilityFactor(), CharacterEnergyWeight, StabilityInfluence);
            factors.AddStabilityFactor(surfaceEnergyFactors.GetStabilityFactor(), SurfaceEnergyWeight, StabilityInfluence);
            factors.AddStabilityFactor(yearlyCycleFactors.GetStabilityFactor(), YearlyCycleWeight, StabilityInfluence);
            factors.AddStabilityFactor(monthlyCycleFactors.GetStabilityFactor(), MonthlyCycleWeight, StabilityInfluence);

            return factors;
        }

        private List<BiorhythmFactor> GetAllFactors()
        {
            var factors = new List<BiorhythmFactor>();

            var mainEnergyFactors = PersonalFactors.Where(e => e.Energy == NineStarKiModel.MainEnergy.Energy).ToList();
            var characterEnergyFactors = PersonalFactors.Where(e => e.Energy == NineStarKiModel.CharacterEnergy.Energy).ToList();
            var surfaceEnergyFactors = PersonalFactors.Where(e => e.Energy == NineStarKiModel.SurfaceEnergy.Energy).ToList();
            var yearlyCycleFactors = CycleFactors.Where(e => e.Energy == NineStarKiModel.PersonalHousesOccupiedEnergies.Year.Energy).ToList();
            var monthlyCycleFactors = CycleFactors.Where(e => e.Energy == NineStarKiModel.PersonalHousesOccupiedEnergies.Month.Energy).ToList();;

            foreach (var factor in mainEnergyFactors)
            {
                factors.AddFactor(factor.Biorhythm, factor.Value, MainEnergyWeight);
            }

            foreach (var factor in characterEnergyFactors)
            {
                factors.AddFactor(factor.Biorhythm, factor.Value, CharacterEnergyWeight);
            }

            foreach (var factor in surfaceEnergyFactors)
            {
                factors.AddFactor(factor.Biorhythm, factor.Value, SurfaceEnergyWeight);
            }

            foreach (var factor in yearlyCycleFactors)
            {
                factors.AddFactor(factor.Biorhythm, factor.Value, YearlyCycleWeight);
            }

            foreach (var factor in monthlyCycleFactors)
            {
                factors.AddFactor(factor.Biorhythm, factor.Value, MonthlyCycleWeight);
            }
            
            return factors;
        }

        private List<BiorhythmFactor> GetPersonalFactors()
        {
            var factors = new List<BiorhythmFactor>();

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 1.3, ENineStarKiEnergy.Water));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 1.2, ENineStarKiEnergy.Water));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 0.9, ENineStarKiEnergy.Water));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 1.2, ENineStarKiEnergy.Water));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 1, ENineStarKiEnergy.Water));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 1.3, ENineStarKiEnergy.Water));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 1, ENineStarKiEnergy.Water));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 1.1, ENineStarKiEnergy.Soil));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 0.7, ENineStarKiEnergy.Soil));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 1.1, ENineStarKiEnergy.Soil));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 0.7, ENineStarKiEnergy.Soil));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 0.8, ENineStarKiEnergy.Soil));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 1, ENineStarKiEnergy.Soil));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 1.2, ENineStarKiEnergy.Soil));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 1.1, ENineStarKiEnergy.Thunder));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 1.1, ENineStarKiEnergy.Thunder));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 1.3, ENineStarKiEnergy.Thunder));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 1.3, ENineStarKiEnergy.Thunder));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 1.3, ENineStarKiEnergy.Thunder));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 0.9, ENineStarKiEnergy.Thunder));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 0.7, ENineStarKiEnergy.Thunder));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 1.3, ENineStarKiEnergy.Wind));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 1.2, ENineStarKiEnergy.Wind));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 0.8, ENineStarKiEnergy.Wind));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 1, ENineStarKiEnergy.Wind));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 1.2, ENineStarKiEnergy.Wind));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 0.8, ENineStarKiEnergy.Wind));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 0.7, ENineStarKiEnergy.Wind));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 1, ENineStarKiEnergy.CoreEarth));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 1, ENineStarKiEnergy.CoreEarth));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 1.3, ENineStarKiEnergy.CoreEarth));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 0.9, ENineStarKiEnergy.CoreEarth));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 1, ENineStarKiEnergy.CoreEarth));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 1, ENineStarKiEnergy.CoreEarth));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 1, ENineStarKiEnergy.CoreEarth));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 0.8, ENineStarKiEnergy.Heaven));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 1.2, ENineStarKiEnergy.Heaven));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 1, ENineStarKiEnergy.Heaven));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 1.1, ENineStarKiEnergy.Heaven));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 1.1, ENineStarKiEnergy.Heaven));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 1, ENineStarKiEnergy.Heaven));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 1.2, ENineStarKiEnergy.Heaven));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 0.7, ENineStarKiEnergy.Lake));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 1.3, ENineStarKiEnergy.Lake));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 1.1, ENineStarKiEnergy.Lake));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 0.8, ENineStarKiEnergy.Lake));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 1.2, ENineStarKiEnergy.Lake));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 0.9, ENineStarKiEnergy.Lake));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 1.2, ENineStarKiEnergy.Lake));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 0.7, ENineStarKiEnergy.Mountain));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 1.2, ENineStarKiEnergy.Mountain));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 1.3, ENineStarKiEnergy.Mountain));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 1.2, ENineStarKiEnergy.Mountain));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 0.8, ENineStarKiEnergy.Mountain));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 0.7, ENineStarKiEnergy.Mountain));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 1.3, ENineStarKiEnergy.Mountain));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 1.3, ENineStarKiEnergy.Fire));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 0.9, ENineStarKiEnergy.Fire));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 0.9, ENineStarKiEnergy.Fire));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 1.1, ENineStarKiEnergy.Fire));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 1.3, ENineStarKiEnergy.Fire));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 1.1, ENineStarKiEnergy.Fire));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 0.9, ENineStarKiEnergy.Fire));

            return factors;
        }

        private List<BiorhythmFactor> GetCycleFactors()
        {
            var factors = new List<BiorhythmFactor>();

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 0.7, ENineStarKiEnergy.Water));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 0.7, ENineStarKiEnergy.Water));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 0.7, ENineStarKiEnergy.Water));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 1.3, ENineStarKiEnergy.Water));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 0.8, ENineStarKiEnergy.Water));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 1.3, ENineStarKiEnergy.Water));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 1, ENineStarKiEnergy.Water));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 0.8, ENineStarKiEnergy.Soil));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 0.8, ENineStarKiEnergy.Soil));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 1, ENineStarKiEnergy.Soil));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 0.8, ENineStarKiEnergy.Soil));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 0.8, ENineStarKiEnergy.Soil));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 1, ENineStarKiEnergy.Soil));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 1.2, ENineStarKiEnergy.Soil));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 1.2, ENineStarKiEnergy.Thunder));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 1.2, ENineStarKiEnergy.Thunder));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 1.3, ENineStarKiEnergy.Thunder));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 1.3, ENineStarKiEnergy.Thunder));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 1.3, ENineStarKiEnergy.Thunder));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 0.7, ENineStarKiEnergy.Thunder));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 0.8, ENineStarKiEnergy.Thunder));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 1.3, ENineStarKiEnergy.Wind));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 1.3, ENineStarKiEnergy.Wind));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 1.1, ENineStarKiEnergy.Wind));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 1, ENineStarKiEnergy.Wind));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 1.2, ENineStarKiEnergy.Wind));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 0.9, ENineStarKiEnergy.Wind));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 0.8, ENineStarKiEnergy.Wind));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 1, ENineStarKiEnergy.CoreEarth));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 1, ENineStarKiEnergy.CoreEarth));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 1.3, ENineStarKiEnergy.CoreEarth));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 1, ENineStarKiEnergy.CoreEarth));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 1, ENineStarKiEnergy.CoreEarth));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 1, ENineStarKiEnergy.CoreEarth));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 0.7, ENineStarKiEnergy.CoreEarth));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 1.3, ENineStarKiEnergy.Heaven));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 1.2, ENineStarKiEnergy.Heaven));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 1.2, ENineStarKiEnergy.Heaven));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 1.3, ENineStarKiEnergy.Heaven));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 1, ENineStarKiEnergy.Heaven));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 1, ENineStarKiEnergy.Heaven));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 1.2, ENineStarKiEnergy.Heaven));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 1.2, ENineStarKiEnergy.Lake));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 1.3, ENineStarKiEnergy.Lake));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 1.1, ENineStarKiEnergy.Lake));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 1.2, ENineStarKiEnergy.Lake));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 1.3, ENineStarKiEnergy.Lake));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 0.8, ENineStarKiEnergy.Lake));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 1.2, ENineStarKiEnergy.Lake));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 0.7, ENineStarKiEnergy.Mountain));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 0.9, ENineStarKiEnergy.Mountain));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 1.2, ENineStarKiEnergy.Mountain));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 1.3, ENineStarKiEnergy.Mountain));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 0.8, ENineStarKiEnergy.Mountain));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 0.9, ENineStarKiEnergy.Mountain));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 1.3, ENineStarKiEnergy.Mountain));

            factors.Add(new BiorhythmFactor(EBiorhythm.Emotional, 1.3, ENineStarKiEnergy.Fire));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intellectual, 1, ENineStarKiEnergy.Fire));
            factors.Add(new BiorhythmFactor(EBiorhythm.Physical, 1.2, ENineStarKiEnergy.Fire));
            factors.Add(new BiorhythmFactor(EBiorhythm.Spiritual, 1, ENineStarKiEnergy.Fire));
            factors.Add(new BiorhythmFactor(EBiorhythm.Creative, 1.3, ENineStarKiEnergy.Fire));
            factors.Add(new BiorhythmFactor(EBiorhythm.Intuitive, 0.9, ENineStarKiEnergy.Fire));
            factors.Add(new BiorhythmFactor(EBiorhythm.Stability, 1.1, ENineStarKiEnergy.Fire));

            return factors;
        }
    }
}