using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

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
            CompatibilityDetails = new CompatibilityDetailsModel(nineStarKiModel1, nineStarKiModel2);
            FundamentalEnergyChemistryScore = GetFundamentalElementChemistryScore();
            FundamentalEnergyLearningPotentialScore = GetFundamentalEnergyLearningPotentialScore();
            FundamentalEnergyConflictPotentialScore = GetFundamentalEnergyConflictPotentialScore();
            FundamentalEnergyHarmonyScore = GetFundamentalEnergyHarmonyScore();
            CharacterEnergyChemistryScore = GetCharacterEnergyChemistryScore();
            CharacterEnergyLearningPotentialScore = GetCharacterEnergyLearningPotentialScore();
            CharacterEnergyConflictPotentialScore = GetCharacterEnergyConflictPotentialScore();
            CharacterEnergyHarmonyScore = GetCharacterEnergyHarmonyScore();

            FundamentalEnergiesCompatibility = TemplateProcessor.PopulateTemplate(GetFundamentalEnergiesCompatibilityDetails(), new
            {
                Person1 = FirstFundamentalEnergyPersonName,
                Person2 = SecondFundamentalEnergyPersonName,
                Person1Proper = FirstFundamentalEnergyPersonName.ToProperCase(),
                Person2Proper = SecondFundamentalEnergyPersonName.ToProperCase()
            });

            CharacterEnergiesCompatibility = TemplateProcessor.PopulateTemplate(GetCharacterEnergiesCompatibilityDetails(), new
            {
                Person1 = FirstCharacterEnergyPersonName,
                Person2 = SecondCharacterEnergyPersonName,
                Person1Proper = FirstCharacterEnergyPersonName.ToProperCase(),
                Person2Proper = SecondCharacterEnergyPersonName.ToProperCase()
            });

            SexualChemistryDetails = TemplateProcessor.PopulateTemplate(GetSexualChemistryDescription(), new
            {
                Person1 = NineStarKiModel1?.PersonModel?.Name,
                Person2 = NineStarKiModel2?.PersonModel?.Name,
                Person1Proper = NineStarKiModel1?.PersonModel?.Name?.ToProperCase(),
                Person2Proper = NineStarKiModel2?.PersonModel?.Name?.ToProperCase()
            });

            CompatibilitySummary = TemplateProcessor.PopulateTemplate(GetCompatibilitySummary(), new
            {
                Person1 = NineStarKiModel1?.PersonModel?.Name,
                Person2 = NineStarKiModel2?.PersonModel?.Name,
                Person1Proper = NineStarKiModel1?.PersonModel?.Name?.ToProperCase(),
                Person2Proper = NineStarKiModel2?.PersonModel?.Name?.ToProperCase()
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

        public string FundamentalEnergiesCompatibility { get; }

        public string CharacterEnergiesCompatibility { get; }

        public string CompatibilitySummary { get; }

        public string SexualChemistryDetails { get; }

        public bool IsProcessed { get; set; }

        public bool IsUpgradeRequired { get; set; }

        public CompatibilityDetailsModel CompatibilityDetails { get; set; }

        public string FirstPersonName => NineStarKiModel1.PersonModel.Name ?? Globalisation.Dictionary.FirstPerson;

        public string SecondPersonName => NineStarKiModel2.PersonModel.Name ?? Globalisation.Dictionary.SecondPerson;

        public string FirstPersonNameWithArticle => NineStarKiModel1.PersonModel.Name ?? $"the {Globalisation.Dictionary.FirstPerson.ToLower()}";

        public string SecondPersonNameWithArticle => NineStarKiModel2.PersonModel.Name ?? $"the {Globalisation.Dictionary.SecondPerson.ToLower()}";

        public string FirstFundamentalEnergyPersonName => NineStarKiModel1.MainEnergy.Energy <= NineStarKiModel2.MainEnergy.Energy
            ? FirstPersonNameWithArticle
            : SecondPersonNameWithArticle;

        public string SecondFundamentalEnergyPersonName => NineStarKiModel1.MainEnergy.Energy <= NineStarKiModel2.MainEnergy.Energy
            ? SecondPersonNameWithArticle
            : FirstPersonNameWithArticle;

        public string FirstCharacterEnergyPersonName => NineStarKiModel1.CharacterEnergy.Energy <= NineStarKiModel2.CharacterEnergy.Energy
            ? FirstPersonNameWithArticle
            : SecondPersonNameWithArticle;

        public string SecondCharacterEnergyPersonName => NineStarKiModel1.CharacterEnergy.Energy <= NineStarKiModel2.CharacterEnergy.Energy
            ? SecondPersonNameWithArticle
            : FirstPersonNameWithArticle;

        public ESexualChemistryScore TotalSexualChemistryScore => GetTotalSexualChemistryScore(NineStarKiModel1, NineStarKiModel2);

        public ECompatibilityScore TotalEnergyChemistryScore =>
            GetAverageScore(FundamentalEnergyChemistryScore, CharacterEnergyChemistryScore);

        public ECompatibilityScore TotalEnergyLearningPotentialScore => GetAverageScore(FundamentalEnergyLearningPotentialScore, CharacterEnergyLearningPotentialScore);

        public ECompatibilityScore TotalEnergyConflictPotentialScore => GetAverageScore(FundamentalEnergyConflictPotentialScore, CharacterEnergyConflictPotentialScore);

        public ECompatibilityScore TotalHarmonyScore => GetAverageScore(FundamentalEnergyHarmonyScore, CharacterEnergyHarmonyScore);

        private ESexualChemistryScore GetTotalSexualChemistryScore(NineStarKiModel energy1, NineStarKiModel energy2)
        {
            if (energy1.MainEnergy == null || energy2.MainEnergy == null)
            {
                return ESexualChemistryScore.Unspecified;
            }

            ESexualChemistryScore score = 0;

            // Main
            if (!CompatibilityDetails.IsFundamtenalGenderSame)
            {
                score += 3;
            }

            if (CompatibilityDetails.IsFundamentalElementChallenging)
            {
                score += 3;
            }
            else if (CompatibilityDetails.IsFundamentalElementSupportive)
            {
                score += 2;
            }

            if (!CompatibilityDetails.IsFundamentalModalitySame)
            {
                score += 1;
            }

            // Character
            if (!CompatibilityDetails.IsCharacterGenderSame)
            {
                score += 4;
            }

            if (CompatibilityDetails.IsCharacterElementChallenging)
            {
                score += 3;
            }
            else if (CompatibilityDetails.IsCharacterElementSupportive)
            {
                score += 2;
            }

            if (!CompatibilityDetails.IsCharacterModalitySame)
            {
                score += 1;
            }

            score = score < 0
                ? 0 :
                score > ESexualChemistryScore.OffTheCharts ?
                    ESexualChemistryScore.OffTheCharts
                    : score;

            return score;
        }

        private string GetSexualChemistryDescription()
        {
            switch (TotalSexualChemistryScore)
            {
                case ESexualChemistryScore.NonExistant:
                    return Globalisation.Dictionary.sexual_chemistry_nonexistant;

                case ESexualChemistryScore.ExtremelyLow:
                case ESexualChemistryScore.VeryVeryLow:
                    return Globalisation.Dictionary.sexual_chemistry_verylow;

                case ESexualChemistryScore.VeryLow:
                case ESexualChemistryScore.Low:
                    return Globalisation.Dictionary.sexual_chemistry_low;

                case ESexualChemistryScore.MediumToLow:
                case ESexualChemistryScore.LowerThanAverage:
                    return Globalisation.Dictionary.sexual_chemistry_mediumlow;

                case ESexualChemistryScore.Average:
                    return Globalisation.Dictionary.sexual_chemistry_medium;

                case ESexualChemistryScore.HigherThanAverage:
                case ESexualChemistryScore.MediumToHigh:
                    return Globalisation.Dictionary.sexual_chemistry_mediumhigh;

                case ESexualChemistryScore.High:
                case ESexualChemistryScore.VeryHigh:
                    return Globalisation.Dictionary.sexual_chemistry_high;

                case ESexualChemistryScore.VeryVeryHigh:
                case ESexualChemistryScore.ExtremelyHigh:
                    return Globalisation.Dictionary.sexual_chemistry_veryhigh;

                case ESexualChemistryScore.OffTheCharts:
                    return Globalisation.Dictionary.sexual_chemistry_off_the_charts;
            }

            return string.Empty;
        }

        private string GetCompatibilitySummary()
        {
            var sb = new StringBuilder();
            var bothEnergiesText = "Fundamental and Character Energies";
            var fundamentalEnergiesText = "Fundamental Energies";
            var characterEnergiesText = "Character Energies";

            // Fundamental
            if (CompatibilityDetails.IsFundamentalElementSame && CompatibilityDetails.IsCharacterElementSame)
            {
                sb.AppendLine(TemplateProcessor.PopulateTemplate(Globalisation.Dictionary.element_same, new
                {
                    EnergiesDescription = bothEnergiesText
                }));
            }
            else if (CompatibilityDetails.IsFundamentalElementSame)
            {
                sb.AppendLine(TemplateProcessor.PopulateTemplate(Globalisation.Dictionary.element_same, new
                {
                    EnergiesDescription = fundamentalEnergiesText
                }));
            }
            if (CompatibilityDetails.IsFundamentalElementChallenging && CompatibilityDetails.IsCharacterElementChallenging)
            {
                sb.AppendLine(TemplateProcessor.PopulateTemplate(Globalisation.Dictionary.element_challenging, new
                {
                    EnergiesDescription = bothEnergiesText
                }));
            }
            else if (CompatibilityDetails.IsFundamentalElementChallenging)
            {
                sb.AppendLine(TemplateProcessor.PopulateTemplate(Globalisation.Dictionary.element_challenging, new
                {
                    EnergiesDescription = fundamentalEnergiesText
                }));
            }
            if (CompatibilityDetails.IsFundamentalElementSupportive && CompatibilityDetails.IsCharacterElementSupportive)
            {
                sb.AppendLine(TemplateProcessor.PopulateTemplate(Globalisation.Dictionary.element_supportive, new
                {
                    EnergiesDescription = bothEnergiesText
                }));
            }
            else if (CompatibilityDetails.IsFundamentalElementSupportive)
            {
                sb.AppendLine(TemplateProcessor.PopulateTemplate(Globalisation.Dictionary.element_supportive, new
                {
                    EnergiesDescription = fundamentalEnergiesText
                }));
            }
            if (CompatibilityDetails.IsFundamtenalGenderSame)
            {
                sb.AppendLine(Globalisation.Dictionary.main_gender_same);
            }
            else
            {
                sb.AppendLine(Globalisation.Dictionary.main_gender_opposite);
            }
            if (CompatibilityDetails.IsFundamentalModalitySame)
            {
                sb.AppendLine(Globalisation.Dictionary.main_modality_same);
            }
            else
            {
                sb.AppendLine(Globalisation.Dictionary.main_modality_different);
            }

            // Character
            if (CompatibilityDetails.IsCharacterElementSame && !CompatibilityDetails.IsFundamentalElementSame)
            {
                sb.AppendLine(TemplateProcessor.PopulateTemplate(Globalisation.Dictionary.element_same, new
                {
                    EnergiesDescription = characterEnergiesText
                }));
            }
            if (CompatibilityDetails.IsCharacterElementChallenging && !CompatibilityDetails.IsCharacterElementChallenging)
            {
                sb.AppendLine(TemplateProcessor.PopulateTemplate(Globalisation.Dictionary.element_challenging, new
                {
                    EnergiesDescription = characterEnergiesText
                }));
            }
            if (CompatibilityDetails.IsCharacterElementSupportive && !CompatibilityDetails.IsCharacterElementSupportive)
            {
                sb.AppendLine(TemplateProcessor.PopulateTemplate(Globalisation.Dictionary.element_supportive, new
                {
                    EnergiesDescription = characterEnergiesText
                }));
            }
            if (CompatibilityDetails.IsCharacterGenderSame)
            {
                sb.AppendLine(Globalisation.Dictionary.character_gender_same);
            }
            else
            {
                sb.AppendLine(Globalisation.Dictionary.character_gender_opposite);
            }
            if (CompatibilityDetails.IsCharacterModalitySame)
            {
                sb.AppendLine(Globalisation.Dictionary.character_modality_same);
            }
            else
            {
                sb.AppendLine(Globalisation.Dictionary.character_modality_different);
            }

            return sb.ToString();
        }

        private string GetFundamentalEnergiesCompatibilityDetails()
        {
            switch (NineStarKiModel1.MainEnergy.Energy)
            {
                case ENineStarKiEnergy.Water:
                    switch (NineStarKiModel2.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            return Globalisation.Dictionary.main_11;

                        case ENineStarKiEnergy.Soil:
                            return Globalisation.Dictionary.main_12;

                        case ENineStarKiEnergy.Thunder:
                            return Globalisation.Dictionary.main_13;

                        case ENineStarKiEnergy.Wind:
                            return Globalisation.Dictionary.main_14;

                        case ENineStarKiEnergy.CoreEarth:
                            return Globalisation.Dictionary.main_15;

                        case ENineStarKiEnergy.Heaven:
                            return Globalisation.Dictionary.main_16;

                        case ENineStarKiEnergy.Lake:
                            return Globalisation.Dictionary.main_17;

                        case ENineStarKiEnergy.Mountain:
                            return Globalisation.Dictionary.main_18;

                        case ENineStarKiEnergy.Fire:
                            return Globalisation.Dictionary.main_19;
                    }

                    break;

                case ENineStarKiEnergy.Soil:
                    switch (NineStarKiModel2.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            return Globalisation.Dictionary.main_12;

                        case ENineStarKiEnergy.Soil:
                            return Globalisation.Dictionary.main_22;

                        case ENineStarKiEnergy.Thunder:
                            return Globalisation.Dictionary.main_23;

                        case ENineStarKiEnergy.Wind:
                            return Globalisation.Dictionary.main_24;

                        case ENineStarKiEnergy.CoreEarth:
                            return Globalisation.Dictionary.main_25;

                        case ENineStarKiEnergy.Heaven:
                            return Globalisation.Dictionary.main_26;

                        case ENineStarKiEnergy.Lake:
                            return Globalisation.Dictionary.main_27;

                        case ENineStarKiEnergy.Mountain:
                            return Globalisation.Dictionary.main_28;

                        case ENineStarKiEnergy.Fire:
                            return Globalisation.Dictionary.main_29;
                    }
                    break;

                case ENineStarKiEnergy.Thunder:
                    switch (NineStarKiModel2.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            return Globalisation.Dictionary.main_13;

                        case ENineStarKiEnergy.Soil:
                            return Globalisation.Dictionary.main_23;

                        case ENineStarKiEnergy.Thunder:
                            return Globalisation.Dictionary.main_33;

                        case ENineStarKiEnergy.Wind:
                            return Globalisation.Dictionary.main_34;

                        case ENineStarKiEnergy.CoreEarth:
                            return Globalisation.Dictionary.main_35;

                        case ENineStarKiEnergy.Heaven:
                            return Globalisation.Dictionary.main_36;

                        case ENineStarKiEnergy.Lake:
                            return Globalisation.Dictionary.main_37;

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
                            return Globalisation.Dictionary.main_14;

                        case ENineStarKiEnergy.Soil:
                            return Globalisation.Dictionary.main_24;

                        case ENineStarKiEnergy.Thunder:
                            return Globalisation.Dictionary.main_34;

                        case ENineStarKiEnergy.Wind:
                            return Globalisation.Dictionary.main_44;

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
                            return Globalisation.Dictionary.main_15;

                        case ENineStarKiEnergy.Soil:
                            return Globalisation.Dictionary.main_25;

                        case ENineStarKiEnergy.Thunder:
                            return Globalisation.Dictionary.main_35;

                        case ENineStarKiEnergy.Wind:
                            break;

                        case ENineStarKiEnergy.CoreEarth:
                            return Globalisation.Dictionary.main_55;

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
                            return Globalisation.Dictionary.main_16;

                        case ENineStarKiEnergy.Soil:
                            return Globalisation.Dictionary.main_26;

                        case ENineStarKiEnergy.Thunder:
                            return Globalisation.Dictionary.main_36;

                        case ENineStarKiEnergy.Wind:
                            break;

                        case ENineStarKiEnergy.CoreEarth:
                            break;

                        case ENineStarKiEnergy.Heaven:
                            return Globalisation.Dictionary.main_66;

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
                            return Globalisation.Dictionary.main_17;

                        case ENineStarKiEnergy.Soil:
                            return Globalisation.Dictionary.main_27;

                        case ENineStarKiEnergy.Thunder:
                            return Globalisation.Dictionary.main_37;

                        case ENineStarKiEnergy.Wind:
                            break;

                        case ENineStarKiEnergy.CoreEarth:
                            break;

                        case ENineStarKiEnergy.Heaven:
                            break;

                        case ENineStarKiEnergy.Lake:
                            return Globalisation.Dictionary.main_77;

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
                            return Globalisation.Dictionary.main_18;

                        case ENineStarKiEnergy.Soil:
                            return Globalisation.Dictionary.main_28;

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
                            return Globalisation.Dictionary.main_88;

                        case ENineStarKiEnergy.Fire:
                            break;
                    }
                    break;

                case ENineStarKiEnergy.Fire:
                    switch (NineStarKiModel2.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            return Globalisation.Dictionary.main_19;

                        case ENineStarKiEnergy.Soil:
                            return Globalisation.Dictionary.main_29;

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
                            return Globalisation.Dictionary.main_99;
                    }
                    break;
            }

            return string.Empty;
        }

        private string GetCharacterEnergiesCompatibilityDetails()
        {
            switch (NineStarKiModel1.MainEnergy.Energy)
            {
                case ENineStarKiEnergy.Water:
                    switch (NineStarKiModel2.MainEnergy.Energy)
                    {
                        case ENineStarKiEnergy.Water:
                            return Globalisation.Dictionary.main_11;

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
            return GetChemistryScore(NineStarKiModel1.CharacterEnergy, NineStarKiModel2.CharacterEnergy, 2);
        }

        private ECompatibilityScore GetChemistryScore(NineStarKiEnergy energy1, NineStarKiEnergy energy2, int genderScoreFactor = 1)
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
            return ECompatibilityScore.ExtremelyHigh - (int)GetFundamentalEnergyConflictPotentialScore();
        }

        private ECompatibilityScore GetCharacterEnergyHarmonyScore()
        {
            return ECompatibilityScore.ExtremelyHigh - (int)GetCharacterEnergyConflictPotentialScore();
        }

        private ECompatibilityScore GetEnergyConflictPotentialScore(NineStarKiEnergy energy1, NineStarKiEnergy energy2)
        {
            var transformationType = energy1.Energy.GetTransformationType(energy2.Energy);

            switch (transformationType)
            {
                case ETransformationType.IsChallenged:
                case ETransformationType.Challenges:
                    return ProcessScore(ECompatibilityScore.High, energy1, energy2, true, true, 0);

                case ETransformationType.Supports:
                case ETransformationType.IsSupported:
                case ETransformationType.Same:
                    return ProcessScore(ECompatibilityScore.Low, energy1, energy2, true, true, 0);
            }

            return ECompatibilityScore.Unspecified;
        }

        private ECompatibilityScore ProcessScore(ECompatibilityScore value, NineStarKiEnergy energy1, NineStarKiEnergy energy2, bool invertCalculation = false, bool isIntuitive = false, int genderScoreFactor = 1)
        {
            var transformationType = energy1.Energy.GetTransformationType(energy2.Energy);
            var isSameGender = energy1.YinYang == energy2.YinYang;
            var isSameModality = energy1.Modality == energy2.Modality;
            var isSameElement = energy1.Element == energy2.Element;
            var isOppositeElement = new List<ETransformationType>
            {
                ETransformationType.Challenges,
                ETransformationType.IsChallenged
            }.Contains(transformationType);

            var score = invertCalculation ?
                (!isSameGender ? genderScoreFactor : (isOppositeElement ? 0 : -1)) + (!isSameModality && !isSameElement ? 1 : 0) + (energy1.Energy == energy2.Energy ? -1 : 0)
                : (isSameGender ? genderScoreFactor : (isOppositeElement ? -1 : 0)) + (isSameModality ? 1 : 0) + (energy1.Energy == energy2.Energy ? 1 : 0);

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