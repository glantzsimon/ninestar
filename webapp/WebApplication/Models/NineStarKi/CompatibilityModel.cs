﻿using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;

namespace K9.WebApplication.Models
{

    public class CompatibilityModel
    {
        public CompatibilityModel()
        {
            var dateOfBirth1 = new DateTime(DateTime.Now.Year - (27), DateTime.Now.Month, DateTime.Now.Day);
            var dateOfBirth2 = new DateTime(DateTime.Now.Year - (27), DateTime.Now.Month, DateTime.Now.Day).AddMonths(2);
            var personModel1 = new PersonModel
            {
                DateOfBirth = dateOfBirth1,
                Gender = Methods.GetRandomGender()
            };
            var personModel2 = new PersonModel
            {
                DateOfBirth = dateOfBirth2,
                Gender = Methods.GetRandomGender()
            };
            
            NineStarKiModel1 = new NineStarKiModel(personModel1);
            NineStarKiModel2 = new NineStarKiModel(personModel2);
        }

        public CompatibilityModel(NineStarKiModel nineStarKiModel1, NineStarKiModel nineStarKiModel2)
        {
            NineStarKiModel1 = nineStarKiModel1;
            NineStarKiModel2 = nineStarKiModel2;
            NineStarKiSummaryModel1 = new NineStarKiSummaryModel(nineStarKiModel1);
            NineStarKiSummaryModel2 = new NineStarKiSummaryModel(nineStarKiModel2);

            CompatibilityDetails = new CompatibilityDetailsModel(this);
            
            FundamentalEnergiesCompatibility = TemplateParser.Parse(GetFundamentalEnergiesCompatibilityDetails(), new
            {
                Person1 = FirstFundamentalEnergyPersonName,
                Person2 = SecondFundamentalEnergyPersonName,
                Person1Proper = FirstFundamentalEnergyPersonName.ToProperCase(),
                Person2Proper = SecondFundamentalEnergyPersonName.ToProperCase(),
                Gender1PossessivePronoun = FirstFundamentalEnergyGenderPossessivePronoun,
                Gender2PossessivePronoun = SecondFundamentalEnergyGenderPossessivePronoun,
                Gender1Pronoun = FirstFundamentalEnergyGenderPronoun,
                Gender2Pronoun = SecondFundamentalEnergyGenderPronoun,
                Gender1PossessivePronounProper = FirstFundamentalEnergyGenderPossessivePronoun.ToProperCase(),
                Gender2PossessivePronounProper = SecondFundamentalEnergyGenderPossessivePronoun.ToProperCase(),
                Gender1PronounProper = FirstFundamentalEnergyGenderPronoun.ToProperCase(),
                Gender2PronounProper = SecondFundamentalEnergyGenderPronoun.ToProperCase()
            });
            
            SexualChemistryDetails = TemplateParser.Parse(GetSexualChemistryDescription(), new
            {
                Person1 = FirstPersonNameWithArticle,
                Person2 = SecondPersonNameWithArticle,
                Person1Proper = FirstPersonNameWithArticle.ToProperCase(),
                Person2Proper = SecondPersonNameWithArticle.ToProperCase(),
                Gender1PossessivePronoun = NineStarKiModel1?.PersonModel?.GenderPossessivePronoun,
                Gender2PossessivePronoun = NineStarKiModel2?.PersonModel?.GenderPossessivePronoun,
                Gender1Pronoun = NineStarKiModel1?.PersonModel?.GenderPronoun,
                Gender2Pronoun = NineStarKiModel2?.PersonModel?.GenderPronoun,
                Gender1PossessivePronounProper = NineStarKiModel1?.PersonModel?.GenderPossessivePronoun.ToProperCase(),
                Gender2PossessivePronounProper = NineStarKiModel2?.PersonModel?.GenderPossessivePronoun.ToProperCase(),
                Gender1PronounProper = NineStarKiModel1?.PersonModel?.GenderPronoun.ToProperCase(),
                Gender2PronounProper = NineStarKiModel2?.PersonModel?.GenderPronoun.ToProperCase()
            });
        }

        [ScriptIgnore]
        public NineStarKiModel NineStarKiModel1 { get; }

        [ScriptIgnore]
        public NineStarKiModel NineStarKiModel2 { get; }

        public NineStarKiSummaryModel NineStarKiSummaryModel1 { get; }
        
        public NineStarKiSummaryModel NineStarKiSummaryModel2 { get; }
        
        public string FundamentalEnergiesCompatibility { get; }
        
        public string SexualChemistryDetails { get; }

        [ScriptIgnore]
        public bool IsProcessed { get; set; }

        /// <summary>
        /// Free accounts get 3 complementary readings of each type. This Flag is true when a complementary reading is used
        /// </summary>
        public bool IsComplementary { get; set; }

        [ScriptIgnore]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.DoNotDisplaySexualityLabel)]
        public bool IsHideSexualChemistry { get; set; }
        
        public CompatibilityDetailsModel CompatibilityDetails { get; set; }

        [ScriptIgnore]
        public string FirstPersonName => NineStarKiModel1.PersonModel.Name ?? Globalisation.Dictionary.FirstPerson;

        [ScriptIgnore]
        public string SecondPersonName => NineStarKiModel2.PersonModel.Name ?? Globalisation.Dictionary.SecondPerson;
        
        [ScriptIgnore]
        public string FirstPersonNameWithArticle => NineStarKiModel1.PersonModel.Name ?? $"the {Globalisation.Dictionary.FirstPerson.ToLower()}";

        [ScriptIgnore]
        public string SecondPersonNameWithArticle => NineStarKiModel2.PersonModel.Name ?? $"the {Globalisation.Dictionary.SecondPerson.ToLower()}";

        [ScriptIgnore]
        public string FirstFundamentalEnergyPersonName => NineStarKiModel1.MainEnergy.Energy <= NineStarKiModel2.MainEnergy.Energy
            ? FirstPersonNameWithArticle
            : SecondPersonNameWithArticle;

        [ScriptIgnore]
        public string SecondFundamentalEnergyPersonName => NineStarKiModel1.MainEnergy.Energy <= NineStarKiModel2.MainEnergy.Energy
            ? SecondPersonNameWithArticle
            : FirstPersonNameWithArticle;

        [ScriptIgnore]
        public string FirstFundamentalEnergyGenderPossessivePronoun => NineStarKiModel1.MainEnergy.Energy <= NineStarKiModel2.MainEnergy.Energy
            ? NineStarKiModel1?.PersonModel?.GenderPossessivePronoun
            : NineStarKiModel2?.PersonModel?.GenderPossessivePronoun;

        [ScriptIgnore]
        public string SecondFundamentalEnergyGenderPossessivePronoun => NineStarKiModel1.MainEnergy.Energy <= NineStarKiModel2.MainEnergy.Energy
            ? NineStarKiModel2?.PersonModel?.GenderPossessivePronoun
            : NineStarKiModel1?.PersonModel?.GenderPossessivePronoun;

        [ScriptIgnore]
        public string FirstCharacterEnergyGenderPossessivePronoun => NineStarKiModel1.CharacterEnergy.Energy <= NineStarKiModel2.CharacterEnergy.Energy
            ? NineStarKiModel1?.PersonModel?.GenderPossessivePronoun
            : NineStarKiModel2?.PersonModel?.GenderPossessivePronoun;

        [ScriptIgnore]
        public string SecondCharacterEnergyGenderPossessivePronoun => NineStarKiModel1.CharacterEnergy.Energy <= NineStarKiModel2.CharacterEnergy.Energy
            ? NineStarKiModel2?.PersonModel?.GenderPossessivePronoun
            : NineStarKiModel1?.PersonModel?.GenderPossessivePronoun;

        [ScriptIgnore]
        public string FirstFundamentalEnergyGenderPronoun => NineStarKiModel1.MainEnergy.Energy <= NineStarKiModel2.MainEnergy.Energy
            ? NineStarKiModel1?.PersonModel?.GenderPronoun
            : NineStarKiModel2?.PersonModel?.GenderPronoun;

        [ScriptIgnore]
        public string SecondFundamentalEnergyGenderPronoun => NineStarKiModel1.MainEnergy.Energy <= NineStarKiModel2.MainEnergy.Energy
            ? NineStarKiModel2?.PersonModel?.GenderPronoun
            : NineStarKiModel1?.PersonModel?.GenderPronoun;

        [ScriptIgnore]
        public string FirstCharacterEnergyGenderPronoun => NineStarKiModel1.CharacterEnergy.Energy <= NineStarKiModel2.CharacterEnergy.Energy
            ? NineStarKiModel1?.PersonModel?.GenderPronoun
            : NineStarKiModel2?.PersonModel?.GenderPronoun;

        [ScriptIgnore]
        public string SecondCharacterEnergyGenderPronoun => NineStarKiModel1.CharacterEnergy.Energy <= NineStarKiModel2.CharacterEnergy.Energy
            ? NineStarKiModel2?.PersonModel?.GenderPronoun
            : NineStarKiModel1?.PersonModel?.GenderPronoun;

        [ScriptIgnore]
        public string FirstCharacterEnergyPersonName => NineStarKiModel1.CharacterEnergy.Energy <= NineStarKiModel2.CharacterEnergy.Energy
            ? FirstPersonNameWithArticle
            : SecondPersonNameWithArticle;

        [ScriptIgnore]
        public string SecondCharacterEnergyPersonName => NineStarKiModel1.CharacterEnergy.Energy <= NineStarKiModel2.CharacterEnergy.Energy
            ? SecondPersonNameWithArticle
            : FirstPersonNameWithArticle;
        
        private string GetSexualChemistryDescription()
        {
            switch (CompatibilityDetails.Score.SexualChemistryScore)
            {
                case ESexualChemistryScore.Unspecified:
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
                            return Globalisation.Dictionary.main_38;

                        case ENineStarKiEnergy.Fire:
                            return Globalisation.Dictionary.main_39;
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
                            return Globalisation.Dictionary.main_45;

                        case ENineStarKiEnergy.Heaven:
                            return Globalisation.Dictionary.main_46;

                        case ENineStarKiEnergy.Lake:
                            return Globalisation.Dictionary.main_47;

                        case ENineStarKiEnergy.Mountain:
                            return Globalisation.Dictionary.main_48;

                        case ENineStarKiEnergy.Fire:
                            return Globalisation.Dictionary.main_49;
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
                            return Globalisation.Dictionary.main_45;

                        case ENineStarKiEnergy.CoreEarth:
                            return Globalisation.Dictionary.main_55;

                        case ENineStarKiEnergy.Heaven:
                            return Globalisation.Dictionary.main_56;

                        case ENineStarKiEnergy.Lake:
                            return Globalisation.Dictionary.main_57;

                        case ENineStarKiEnergy.Mountain:
                            return Globalisation.Dictionary.main_58;

                        case ENineStarKiEnergy.Fire:
                            return Globalisation.Dictionary.main_59;
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
                            return Globalisation.Dictionary.main_46;

                        case ENineStarKiEnergy.CoreEarth:
                            return Globalisation.Dictionary.main_56;

                        case ENineStarKiEnergy.Heaven:
                            return Globalisation.Dictionary.main_66;

                        case ENineStarKiEnergy.Lake:
                            return Globalisation.Dictionary.main_67;

                        case ENineStarKiEnergy.Mountain:
                            return Globalisation.Dictionary.main_68;

                        case ENineStarKiEnergy.Fire:
                            return Globalisation.Dictionary.main_69;
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
                            return Globalisation.Dictionary.main_47;

                        case ENineStarKiEnergy.CoreEarth:
                            return Globalisation.Dictionary.main_57;

                        case ENineStarKiEnergy.Heaven:
                            return Globalisation.Dictionary.main_67;

                        case ENineStarKiEnergy.Lake:
                            return Globalisation.Dictionary.main_77;

                        case ENineStarKiEnergy.Mountain:
                            return Globalisation.Dictionary.main_78;

                        case ENineStarKiEnergy.Fire:
                            return Globalisation.Dictionary.main_79;
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
                            return Globalisation.Dictionary.main_38;

                        case ENineStarKiEnergy.Wind:
                            return Globalisation.Dictionary.main_48;

                        case ENineStarKiEnergy.CoreEarth:
                            return Globalisation.Dictionary.main_58;

                        case ENineStarKiEnergy.Heaven:
                            return Globalisation.Dictionary.main_68;

                        case ENineStarKiEnergy.Lake:
                            return Globalisation.Dictionary.main_78;

                        case ENineStarKiEnergy.Mountain:
                            return Globalisation.Dictionary.main_88;

                        case ENineStarKiEnergy.Fire:
                            return Globalisation.Dictionary.main_89;
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
                            return Globalisation.Dictionary.main_39;

                        case ENineStarKiEnergy.Wind:
                            return Globalisation.Dictionary.main_49;

                        case ENineStarKiEnergy.CoreEarth:
                            return Globalisation.Dictionary.main_59;

                        case ENineStarKiEnergy.Heaven:
                            return Globalisation.Dictionary.main_69;

                        case ENineStarKiEnergy.Lake:
                            return Globalisation.Dictionary.main_79;

                        case ENineStarKiEnergy.Mountain:
                            return Globalisation.Dictionary.main_89;

                        case ENineStarKiEnergy.Fire:
                            return Globalisation.Dictionary.main_99;
                    }
                    break;
            }

            return Globalisation.Dictionary.ComingSoon;
        }
       }
}