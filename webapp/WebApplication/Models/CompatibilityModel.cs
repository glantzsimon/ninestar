using K9.SharedLibrary.Helpers;
using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;
using System;
using System.Collections.Generic;

namespace K9.WebApplication.Models
{

    public class CompatibilityModel
    {
        public CompatibilityModel()
        {
            NineStarKiModel1 = new NineStarKiModel();
            NineStarKiModel2 = new NineStarKiModel();
        }

        public CompatibilityModel(NineStarKiModel nineStarKiModel1, NineStarKiModel nineStarKiModel2)
        {
            NineStarKiModel1 = nineStarKiModel1;
            NineStarKiModel2 = nineStarKiModel2;
            FundamentalEnergyChemistryScore = GetFundamentalElementChemistryScore();
            FundamentalEnergyLearningPotentialScore = GetFundamentalEnergyLearningPotentialScore();
            FundamentalEnergyConflictPotentialScore = GetFundamentalEnergyConflictPotentialScore();
            FundamentalEnergyHarmonyScore = GetFundamentalEnergyHarmonyScore();
            CharacterEnergyChemistryScore = GetCharacterEnergyChemistryScore();
            CharacterEnergyLearningPotentialScore = GetCharacterEnergyLearningPotentialScore();
            CharacterEnergyConflictPotentialScore = GetCharacterEnergyConflictPotentialScore();
            CharacterEnergyHarmonyScore = GetCharacterEnergyHarmonyScore();

            FundamentalEnergyDetails = TemplateProcessor.PopulateTemplate(GetFundamentalEnergyDetails(), new
            {
                Person1 = FirstPersonName,
                Person2 = SecondPersonName
            });

            CharacterEnergyDetails = TemplateProcessor.PopulateTemplate(GetCharacterEnergyDetails(), new
            {
                Person1 = FirstPersonName,
                Person2 = SecondPersonName
            });
        }

        public NineStarKiModel NineStarKiModel1 { get; }

        public NineStarKiModel NineStarKiModel2 { get; }

        public ECompatibilityScore FundamentalEnergyChemistryScore { get; }

        public ECompatibilityScore FundamentalEnergyLearningPotentialScore { get; }

        public ECompatibilityScore FundamentalEnergyConflictPotentialScore { get; }

        public ECompatibilityScore FundamentalEnergyHarmonyScore { get; }

        public ECompatibilityScore CharacterEnergyChemistryScore { get; }

        public ECompatibilityScore CharacterEnergyLearningPotentialScore { get; }

        public ECompatibilityScore CharacterEnergyConflictPotentialScore { get; }

        public ECompatibilityScore CharacterEnergyHarmonyScore { get; }

        public string FundamentalEnergyDetails { get; }
        
        public string CharacterEnergyDetails { get; }

        public bool IsProcessed { get; set; }

        public bool IsUpgradeRequired { get; set; }

        public string FirstPersonName => NineStarKiModel1.PersonModel.Name ?? Globalisation.Dictionary.FirstPerson;

        public string SecondPersonName => NineStarKiModel2.PersonModel.Name ?? Globalisation.Dictionary.SecondPerson;

        public ECompatibilityScore TotalEnergyChemistryScore =>
            GetAverageScore(FundamentalEnergyChemistryScore, CharacterEnergyChemistryScore);

        public ECompatibilityScore TotalEnergyLearningPotentialScore => GetAverageScore(FundamentalEnergyLearningPotentialScore, CharacterEnergyLearningPotentialScore);

        public ECompatibilityScore TotalEnergyConflictPotentialScore => GetAverageScore(FundamentalEnergyConflictPotentialScore, CharacterEnergyConflictPotentialScore);

        public ECompatibilityScore TotalHarmonyScore => GetAverageScore(FundamentalEnergyHarmonyScore, CharacterEnergyHarmonyScore);

        private string GetFundamentalEnergyDetails()
        {
            switch (NineStarKiModel1.MainEnergy.Energy)
            {
                case ENineStarKiEnergy.Water:
                    switch (NineStarKiModel2.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            return Globalisation.Dictionary.water_water_main;

                        case ENineStarKiEnergy.Soil:
                            break;

                        case ENineStarKiEnergy.Thunder:
                            break;

                        case ENineStarKiEnergy.Wind:
                            break;

                        case ENineStarKiEnergy.CoreEarth:
                            break;

                        case ENineStarKiEnergy.Heaven:
                            break;

                        case ENineStarKiEnergy.Lake:
                            break;

                        case ENineStarKiEnergy.Mountain:
                            break;

                        case ENineStarKiEnergy.Fire:
                            break;
                    }

                    break;

                case ENineStarKiEnergy.Soil:
                    switch (NineStarKiModel2.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            break;

                        case ENineStarKiEnergy.Soil:
                            return Globalisation.Dictionary.soil_soil_main;

                        case ENineStarKiEnergy.Thunder:
                            break;

                        case ENineStarKiEnergy.Wind:
                            break;

                        case ENineStarKiEnergy.CoreEarth:
                            break;

                        case ENineStarKiEnergy.Heaven:
                            break;

                        case ENineStarKiEnergy.Lake:
                            break;

                        case ENineStarKiEnergy.Mountain:
                            break;

                        case ENineStarKiEnergy.Fire:
                            break;
                    }
                    break;

                case ENineStarKiEnergy.Thunder:
                    switch (NineStarKiModel2.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            break;

                        case ENineStarKiEnergy.Soil:
                            break;

                        case ENineStarKiEnergy.Thunder:
                            return Globalisation.Dictionary.thunder_thunder_main;

                        case ENineStarKiEnergy.Wind:
                            break;

                        case ENineStarKiEnergy.CoreEarth:
                            break;

                        case ENineStarKiEnergy.Heaven:
                            break;

                        case ENineStarKiEnergy.Lake:
                            break;

                        case ENineStarKiEnergy.Mountain:
                            break;

                        case ENineStarKiEnergy.Fire:
                            break;
                    }
                    break;

                case ENineStarKiEnergy.Wind:
                    switch (NineStarKiModel2.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            break;

                        case ENineStarKiEnergy.Soil:
                            break;

                        case ENineStarKiEnergy.Thunder:
                            break;

                        case ENineStarKiEnergy.Wind:
                            return Globalisation.Dictionary.wind_wind_main;

                        case ENineStarKiEnergy.CoreEarth:
                            break;

                        case ENineStarKiEnergy.Heaven:
                            break;

                        case ENineStarKiEnergy.Lake:
                            break;

                        case ENineStarKiEnergy.Mountain:
                            break;

                        case ENineStarKiEnergy.Fire:
                            break;
                    }
                    break;

                case ENineStarKiEnergy.CoreEarth:
                    switch (NineStarKiModel2.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            break;

                        case ENineStarKiEnergy.Soil:
                            break;

                        case ENineStarKiEnergy.Thunder:
                            break;

                        case ENineStarKiEnergy.Wind:
                            break;

                        case ENineStarKiEnergy.CoreEarth:
                            return Globalisation.Dictionary.coreearth_coreearth_main;

                        case ENineStarKiEnergy.Heaven:
                            break;

                        case ENineStarKiEnergy.Lake:
                            break;

                        case ENineStarKiEnergy.Mountain:
                            break;

                        case ENineStarKiEnergy.Fire:
                            break;
                    }
                    break;

                case ENineStarKiEnergy.Heaven:
                    switch (NineStarKiModel2.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            break;

                        case ENineStarKiEnergy.Soil:
                            break;

                        case ENineStarKiEnergy.Thunder:
                            break;

                        case ENineStarKiEnergy.Wind:
                            break;

                        case ENineStarKiEnergy.CoreEarth:
                            break;

                        case ENineStarKiEnergy.Heaven:
                            return Globalisation.Dictionary.heaven_heaven_main;

                        case ENineStarKiEnergy.Lake:
                            break;

                        case ENineStarKiEnergy.Mountain:
                            break;

                        case ENineStarKiEnergy.Fire:
                            break;
                    }
                    break;

                case ENineStarKiEnergy.Lake:
                    switch (NineStarKiModel2.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            break;

                        case ENineStarKiEnergy.Soil:
                            break;

                        case ENineStarKiEnergy.Thunder:
                            break;

                        case ENineStarKiEnergy.Wind:
                            break;

                        case ENineStarKiEnergy.CoreEarth:
                            break;

                        case ENineStarKiEnergy.Heaven:
                            break;

                        case ENineStarKiEnergy.Lake:
                            return Globalisation.Dictionary.lake_lake_main;

                        case ENineStarKiEnergy.Mountain:
                            break;

                        case ENineStarKiEnergy.Fire:
                            break;
                    }
                    break;

                case ENineStarKiEnergy.Mountain:
                    switch (NineStarKiModel2.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            break;

                        case ENineStarKiEnergy.Soil:
                            break;

                        case ENineStarKiEnergy.Thunder:
                            break;

                        case ENineStarKiEnergy.Wind:
                            break;

                        case ENineStarKiEnergy.CoreEarth:
                            break;

                        case ENineStarKiEnergy.Heaven:
                            break;

                        case ENineStarKiEnergy.Lake:
                            break;

                        case ENineStarKiEnergy.Mountain:
                            return Globalisation.Dictionary.mountain_mountain_main;

                        case ENineStarKiEnergy.Fire:
                            break;
                    }
                    break;

                case ENineStarKiEnergy.Fire:
                    break;
            }

            return string.Empty;
        }

        private string GetCharacterEnergyDetails()
        {
            switch (NineStarKiModel1.MainEnergy.Energy)
            {
                case ENineStarKiEnergy.Water:
                    switch (NineStarKiModel2.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            return Globalisation.Dictionary.water_water_main;

                        case ENineStarKiEnergy.Soil:
                            break;

                        case ENineStarKiEnergy.Thunder:
                            break;

                        case ENineStarKiEnergy.Wind:
                            break;

                        case ENineStarKiEnergy.CoreEarth:
                            break;

                        case ENineStarKiEnergy.Heaven:
                            break;

                        case ENineStarKiEnergy.Lake:
                            break;

                        case ENineStarKiEnergy.Mountain:
                            break;

                        case ENineStarKiEnergy.Fire:
                            break;
                    }

                    break;

                case ENineStarKiEnergy.Soil:
                    break;

                case ENineStarKiEnergy.Thunder:
                    break;

                case ENineStarKiEnergy.Wind:
                    break;

                case ENineStarKiEnergy.CoreEarth:
                    break;

                case ENineStarKiEnergy.Heaven:
                    break;

                case ENineStarKiEnergy.Lake:
                    break;

                case ENineStarKiEnergy.Mountain:
                    break;

                case ENineStarKiEnergy.Fire:
                    break;
            }

            return string.Empty;
        }

        private ECompatibilityScore GetFundamentalElementChemistryScore()
        {
            return GetChemistryScore(NineStarKiModel1.MainEnergy, NineStarKiModel2.MainEnergy);
        }

        private ECompatibilityScore GetCharacterEnergyChemistryScore()
        {
            return GetChemistryScore(NineStarKiModel1.CharacterEnergy, NineStarKiModel2.CharacterEnergy, 1);
        }

        private ECompatibilityScore GetChemistryScore(NineStarKiEnergy energy1, NineStarKiEnergy energy2, int genderScoreFactor = 0)
        {
            var transformationType = energy1.Energy.GetTransformationType(energy2.Energy);

            switch (transformationType)
            {
                case ETransformationType.Challenges:
                case ETransformationType.IsChallenged:
                    return ProcessScore(ECompatibilityScore.High, energy1, energy2, true, false, genderScoreFactor);

                case ETransformationType.IsSupported:
                case ETransformationType.Supports:
                    return ProcessScore(ECompatibilityScore.Medium, energy1, energy2, true, false, genderScoreFactor);

                case ETransformationType.Same:
                    return ProcessScore(ECompatibilityScore.Low, energy1, energy2, true, false, genderScoreFactor);
            }

            return ECompatibilityScore.Unspecified;
        }

        private ECompatibilityScore GetFundamentalEnergyLearningPotentialScore()
        {
            return GetEnergyLearningPotentialScore(NineStarKiModel1.MainEnergy, NineStarKiModel2.MainEnergy);
        }

        private ECompatibilityScore GetCharacterEnergyLearningPotentialScore()
        {
            return GetEnergyLearningPotentialScore(NineStarKiModel1.CharacterEnergy, NineStarKiModel2.CharacterEnergy);
        }

        private ECompatibilityScore GetEnergyLearningPotentialScore(NineStarKiEnergy energy1, NineStarKiEnergy energy2)
        {
            var transformationType = energy1.Energy.GetTransformationType(energy2.Energy);

            switch (transformationType)
            {
                case ETransformationType.IsChallenged:
                case ETransformationType.Challenges:
                case ETransformationType.Supports:
                case ETransformationType.IsSupported:
                    return ProcessScore(ECompatibilityScore.High, energy1, energy2, true);

                case ETransformationType.Same:
                    return ProcessScore(ECompatibilityScore.Low, energy1, energy2, true);
            }

            return ECompatibilityScore.Unspecified;
        }

        private ECompatibilityScore GetFundamentalEnergyConflictPotentialScore()
        {
            return GetEnergyConflictPotentialScore(NineStarKiModel1.MainEnergy, NineStarKiModel2.MainEnergy);
        }

        private ECompatibilityScore GetCharacterEnergyConflictPotentialScore()
        {
            return GetEnergyConflictPotentialScore(NineStarKiModel1.CharacterEnergy, NineStarKiModel2.CharacterEnergy);
        }

        private ECompatibilityScore GetFundamentalEnergyHarmonyScore()
        {
            return (ECompatibilityScore)10 - (int)GetFundamentalEnergyConflictPotentialScore();
        }

        private ECompatibilityScore GetCharacterEnergyHarmonyScore()
        {
            return (ECompatibilityScore)10 - (int)GetCharacterEnergyConflictPotentialScore();
        }

        private ECompatibilityScore GetEnergyConflictPotentialScore(NineStarKiEnergy energy1, NineStarKiEnergy energy2)
        {
            var transformationType = energy1.Energy.GetTransformationType(energy2.Energy);

            switch (transformationType)
            {
                case ETransformationType.IsChallenged:
                case ETransformationType.Challenges:
                    return ProcessScore(ECompatibilityScore.High, energy1, energy2, true, true);

                case ETransformationType.Supports:
                case ETransformationType.IsSupported:
                case ETransformationType.Same:
                    return ProcessScore(ECompatibilityScore.Low, energy1, energy2, true, true);
            }

            return ECompatibilityScore.Unspecified;
        }

        private ECompatibilityScore ProcessScore(ECompatibilityScore value, NineStarKiEnergy energy1, NineStarKiEnergy energy2, bool invertCalculation = false, bool isIntuitive = false, int genderScoreFactor = 0)
        {
            var isSameGender = energy1.YinYang == energy2.YinYang;
            var isSameModality = energy1.Modality == energy2.Modality;
            var transformationType = energy1.Energy.GetTransformationType(energy2.Energy);
            var isSameElement = energy1.Element == energy2.Element;
            var isOppositeElement = new List<ETransformationType>
            {
                ETransformationType.Challenges,
                ETransformationType.IsChallenged
            }.Contains(transformationType);

            var score = invertCalculation ?
                (!isSameGender ? 1 + genderScoreFactor : (isOppositeElement ? 0 : -1)) + (!isSameModality && !isSameElement ? 1 : 0) + (energy1.Energy == energy2.Energy ? -1 : 0)
                : (isSameGender ? 1 + genderScoreFactor : (isOppositeElement ? -1 : 0)) + (isSameModality ? 1 : 0) + (energy1.Energy == energy2.Energy ? 1 : 0);

            if (isIntuitive)
            {
                score = AdjustScoreIntuitively(energy1, energy2, invertCalculation, isSameElement, score);
            }

            var result = value + score;
            result = (int)result < 1 ? ECompatibilityScore.ExtremelyLow : result;
            result = result > ECompatibilityScore.ExtremelyHigh ? ECompatibilityScore.ExtremelyHigh : result;
            return result;
        }

        private static int AdjustScoreIntuitively(NineStarKiEnergy energy1, NineStarKiEnergy energy2, bool invertCalculation, bool isSameElement,
int score)
        {
            if (energy1.Energy == ENineStarKiEnergy.Water || energy2.Energy == ENineStarKiEnergy.Water)
            {
                if (invertCalculation)
                {
                    score -= 2;
                }
                else
                {
                    score += 2;
                }
            }

            if (energy1.Energy == ENineStarKiEnergy.CoreEarth || energy2.Energy == ENineStarKiEnergy.CoreEarth ||
                energy1.Energy == ENineStarKiEnergy.Fire || energy2.Energy == ENineStarKiEnergy.Fire)
            {
                if (invertCalculation)
                {
                    score -= 1;
                }
                else
                {
                    score += 1;
                }
            }

            if (!isSameElement && (energy1.Energy == ENineStarKiEnergy.Thunder || energy2.Energy == ENineStarKiEnergy.Thunder ||
                energy1.Energy == ENineStarKiEnergy.Heaven || energy2.Energy == ENineStarKiEnergy.Heaven))
            {
                if (invertCalculation)
                {
                    score += 1;
                }
                else
                {
                    score -= 1;
                }
            }
            return score;
        }

        private ECompatibilityScore GetAverageScore(ECompatibilityScore fundamentalScore, ECompatibilityScore characterScore)
        {
            var firstScore = (double)fundamentalScore * 1;
            var secondScore = (double)characterScore * 1;
            var average = (int)Math.Round((firstScore + secondScore) / 2, MidpointRounding.AwayFromZero);
            return (ECompatibilityScore)Enum.Parse(typeof(ECompatibilityScore), average.ToString());
        }
    }
}