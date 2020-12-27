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

        public bool IsProcessed { get; set; }

        public bool IsUpgradeRequired { get; set; }

        public ECompatibilityScore TotalEnergyChemistryScore =>
            GetAverageScore(FundamentalEnergyChemistryScore, CharacterEnergyChemistryScore);

        public ECompatibilityScore TotalEnergyLearningPotentialScore => GetAverageScore(FundamentalEnergyLearningPotentialScore, CharacterEnergyLearningPotentialScore);

        public ECompatibilityScore TotalEnergyConflictPotentialScore => GetAverageScore(FundamentalEnergyConflictPotentialScore, CharacterEnergyConflictPotentialScore);

        public ECompatibilityScore TotalHarmonyScore => GetAverageScore(FundamentalEnergyHarmonyScore, CharacterEnergyHarmonyScore);

        private ECompatibilityScore GetFundamentalElementChemistryScore()
        {
            return GetChemistryScore(NineStarKiModel1.MainEnergy, NineStarKiModel2.MainEnergy);
        }

        private ECompatibilityScore GetCharacterEnergyChemistryScore()
        {
            return GetChemistryScore(NineStarKiModel1.CharacterEnergy, NineStarKiModel2.CharacterEnergy);
        }

        private ECompatibilityScore GetChemistryScore(NineStarKiEnergy energy1, NineStarKiEnergy energy2)
        {
            var transformationType = energy1.Energy.GetTransformationType(energy2.Energy);

            switch (transformationType)
            {
                case ETransformationType.Challenges:
                case ETransformationType.IsChallenged:
                    return ProcessScore(ECompatibilityScore.High, energy1, energy2, true);

                case ETransformationType.IsSupported:
                case ETransformationType.Supports:
                    return ProcessScore(ECompatibilityScore.Medium, energy1, energy2, true);

                case ETransformationType.Same:
                    return ProcessScore(ECompatibilityScore.Low, energy1, energy2, true);
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
            var transformationType = NineStarKiModel1.MainEnergy.Energy.GetTransformationType(NineStarKiModel2.MainEnergy.Energy);

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
            var transformationType = NineStarKiModel1.MainEnergy.Energy.GetTransformationType(NineStarKiModel2.MainEnergy.Energy);

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

        private ECompatibilityScore ProcessScore(ECompatibilityScore value, NineStarKiEnergy energy1, NineStarKiEnergy energy2, bool invertCalculation = false, bool isIntuitive = false)
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
                (!isSameGender ? 1 : (isOppositeElement ? 0 : -1)) + (!isSameModality && !isSameElement ? 1 : 0) + (energy1.Energy == energy2.Energy ? -1 : 0)
                : (isSameGender ? 1 : (isOppositeElement ? -1 : 0)) + (isSameModality ? 1 : 0) + (energy1.Energy == energy2.Energy ? 1 : 0);

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