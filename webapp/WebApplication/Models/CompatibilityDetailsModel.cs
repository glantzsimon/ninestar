using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace K9.WebApplication.Models
{
    public class ElementCompatibility
    {
        private readonly NineStarKiModel _nineStarKiModel1;
        private readonly NineStarKiModel _nineStarKiModel2;

        public ElementCompatibility(NineStarKiModel nineStarKiModel1, NineStarKiModel nineStarKiModel2,
            CompatibilityScoreModel score, object pronouns)
        {
            _nineStarKiModel1 = nineStarKiModel1;
            _nineStarKiModel2 = nineStarKiModel2;
            FundamentalEnergiesTransformationType = nineStarKiModel1.MainEnergy.Energy.GetTransformationType(nineStarKiModel2.MainEnergy.Energy);

            FundamentalEnergy1ToCharacterEnergy2TransformationType = nineStarKiModel1.MainEnergy.Energy.GetTransformationType(nineStarKiModel2.CharacterEnergy.Energy);

            FundamentalEnergy1ToSurfaceEnergy2TransformationType = nineStarKiModel1.MainEnergy.Energy.GetTransformationType(nineStarKiModel2.SurfaceEnergy.Energy);

            FundamentalEnergy2ToCharacterEnergy1TransformationType = nineStarKiModel2.MainEnergy.Energy.GetTransformationType(nineStarKiModel1.CharacterEnergy.Energy);

            FundamentalEnergy2ToSurfaceEnergy1TransformationType = nineStarKiModel2.MainEnergy.Energy.GetTransformationType(nineStarKiModel1.SurfaceEnergy.Energy);

            CharacterEnergiesTransformationType = nineStarKiModel1.CharacterEnergy.Energy.GetTransformationType(nineStarKiModel2.CharacterEnergy.Energy);

            CharacterEnergy1ToSurfaceEnergy2TransformationType = nineStarKiModel1.CharacterEnergy.Energy.GetTransformationType(nineStarKiModel2.SurfaceEnergy.Energy);

            CharacterEnergy2ToSurfaceEnergy1TransformationType = nineStarKiModel2.CharacterEnergy.Energy.GetTransformationType(nineStarKiModel1.SurfaceEnergy.Energy);

            SurfaceEnergiesTransformationType = nineStarKiModel1.SurfaceEnergy.Energy.GetTransformationType(nineStarKiModel2.SurfaceEnergy.Energy);

            Score = score;

            FundamentalElementsTransformationDetails = GetFundamentalElementsTransformationDetails();

            CharacterElementsTransformationDetails = GetCharacterElementsTransformationDetails();

            FundamentalElementsCompatibilityDetails = GetElementCompatibilityDetails(_nineStarKiModel1.MainEnergy, _nineStarKiModel2.MainEnergy);

            CharacterElementsCompatibilityDetails = GetElementCompatibilityDetails(_nineStarKiModel1.CharacterEnergy, _nineStarKiModel2.CharacterEnergy);

            AllOtherElementsCompatibility = GetElementsCompatibility(pronouns);

            CalculateScore();
        }

        public CompatibilityScoreModel Score { get; }

        public ETransformationType FundamentalEnergiesTransformationType { get; }
        public ETransformationType FundamentalEnergy1ToCharacterEnergy2TransformationType { get; }
        public ETransformationType FundamentalEnergy1ToSurfaceEnergy2TransformationType { get; }
        public ETransformationType FundamentalEnergy2ToCharacterEnergy1TransformationType { get; }
        public ETransformationType FundamentalEnergy2ToSurfaceEnergy1TransformationType { get; }

        public ETransformationType CharacterEnergiesTransformationType { get; }
        public ETransformationType CharacterEnergy1ToSurfaceEnergy2TransformationType { get; }
        public ETransformationType CharacterEnergy2ToSurfaceEnergy1TransformationType { get; }

        public ETransformationType SurfaceEnergiesTransformationType { get; }

        public string FundamentalElementsTransformationDetails { get; }
        public string CharacterElementsTransformationDetails { get; }
        public string FundamentalElementsCompatibilityDetails { get; }
        public string CharacterElementsCompatibilityDetails { get; }

        public string AllOtherElementsCompatibility { get; }

        public bool FundamentalElementsAreSupportive => IsSupportive(FundamentalEnergiesTransformationType);
        public bool FundamentalEnergiesAreSame => FundamentalEnergiesTransformationType == ETransformationType.Same;
        public bool FundamentalEnergiesAreChallenging => IsChallenging(FundamentalEnergiesTransformationType);
        public bool FundamentalEnergiesAreSameEnergy => _nineStarKiModel1.MainEnergy.Energy == _nineStarKiModel2.MainEnergy.Energy;

        public bool CharacterEnergiesAreSupportive => IsSupportive(CharacterEnergiesTransformationType);
        public bool CharacterEnergiesAreSame => CharacterEnergiesTransformationType == ETransformationType.Same;
        public bool CharacterEnergiesAreChallenging => IsChallenging(CharacterEnergiesTransformationType);
        public bool CharacterEnergiesAreSameEnergy => _nineStarKiModel1.CharacterEnergy.Energy == _nineStarKiModel2.CharacterEnergy.Energy;

        public bool SurfaceEnergiesAreSupportive => IsSupportive(SurfaceEnergiesTransformationType);
        public bool SurfaceEnergiesAreSame => SurfaceEnergiesTransformationType == ETransformationType.Same;
        public bool SurfaceEnergiesAreChallenging => IsChallenging(SurfaceEnergiesTransformationType);
        public bool SurfaceEnergiesAreSameEnergy => _nineStarKiModel1.SurfaceEnergy.Energy == _nineStarKiModel2.SurfaceEnergy.Energy;

        public bool FundamentalEnergy1ToCharacterEnergy2IsSupportive => IsSupportive(FundamentalEnergy1ToCharacterEnergy2TransformationType);
        public bool FundamentalEnergy2ToCharacterEnergy1IsSupportive => IsSupportive(FundamentalEnergy2ToCharacterEnergy1TransformationType);
        public bool FundamentalEnergy1ToSurfaceEnergy2IsSupportive => IsSupportive(FundamentalEnergy1ToSurfaceEnergy2TransformationType);
        public bool FundamentalEnergy2ToSurfaceEnergy1IsSupportive => IsSupportive(FundamentalEnergy2ToSurfaceEnergy1TransformationType);
        public bool CharacterEnergy1ToSurfaceEnergy2IsSupportive => IsSupportive(CharacterEnergy1ToSurfaceEnergy2TransformationType);
        public bool CharacterEnergy2ToSurfaceEnergy1IsSupportive => IsSupportive(CharacterEnergy2ToSurfaceEnergy1TransformationType);

        public bool FundamentalEnergy1ToCharacterEnergy2IsSame => IsSame(FundamentalEnergy1ToCharacterEnergy2TransformationType);
        public bool FundamentalEnergy2ToCharacterEnergy1IsSame => IsSame(FundamentalEnergy2ToCharacterEnergy1TransformationType);
        public bool FundamentalEnergy1ToSurfaceEnergy2IsSame => IsSame(FundamentalEnergy1ToSurfaceEnergy2TransformationType);
        public bool FundamentalEnergy2ToSurfaceEnergy1IsSame => IsSame(FundamentalEnergy2ToSurfaceEnergy1TransformationType);
        public bool CharacterEnergy1ToSurfaceEnergy2IsSame => IsSame(CharacterEnergy1ToSurfaceEnergy2TransformationType);
        public bool CharacterEnergy2ToSurfaceEnergy1IsSame => IsSame(CharacterEnergy2ToSurfaceEnergy1TransformationType);

        public bool FundamentalEnergy1ToCharacterEnergy2IsChallenging => IsChallenging(FundamentalEnergy1ToCharacterEnergy2TransformationType);
        public bool FundamentalEnergy2ToCharacterEnergy1IsChallenging => IsChallenging(FundamentalEnergy2ToCharacterEnergy1TransformationType);
        public bool FundamentalEnergy1ToSurfaceEnergy2IsChallenging => IsChallenging(FundamentalEnergy1ToSurfaceEnergy2TransformationType);
        public bool FundamentalEnergy2ToSurfaceEnergy1IsChallenging => IsChallenging(FundamentalEnergy2ToSurfaceEnergy1TransformationType);
        public bool CharacterEnergy1ToSurfaceEnergy2IsChallenging => IsChallenging(CharacterEnergy1ToSurfaceEnergy2TransformationType);
        public bool CharacterEnergy2ToSurfaceEnergy1IsChallenging => IsChallenging(CharacterEnergy2ToSurfaceEnergy1TransformationType);

        private string GetElementCompatibilityDetails(NineStarKiEnergy energy1, NineStarKiEnergy energy2)
        {
            switch (energy1.Element)
            {
                case ENineStarKiElement.Water:

                    switch (energy2.Element)
                    {
                        case ENineStarKiElement.Water:
                            return Globalisation.Dictionary.water_water;

                        case ENineStarKiElement.Tree:
                            return Globalisation.Dictionary.water_tree;

                        case ENineStarKiElement.Fire:
                            return Globalisation.Dictionary.water_fire;

                        case ENineStarKiElement.Metal:
                            return Globalisation.Dictionary.metal_water;

                        case ENineStarKiElement.Earth:
                            return Globalisation.Dictionary.earth_water;
                    }

                    break;

                case ENineStarKiElement.Tree:

                    switch (energy2.Element)
                    {
                        case ENineStarKiElement.Water:
                            return Globalisation.Dictionary.water_tree;

                        case ENineStarKiElement.Tree:
                            return Globalisation.Dictionary.tree_tree;

                        case ENineStarKiElement.Fire:
                            return Globalisation.Dictionary.tree_fire;

                        case ENineStarKiElement.Metal:
                            return Globalisation.Dictionary.metal_tree;

                        case ENineStarKiElement.Earth:
                            return Globalisation.Dictionary.tree_earth;
                    }
                    break;

                case ENineStarKiElement.Fire:

                    switch (energy2.Element)
                    {
                        case ENineStarKiElement.Water:
                            return Globalisation.Dictionary.water_fire;

                        case ENineStarKiElement.Tree:
                            return Globalisation.Dictionary.tree_fire;

                        case ENineStarKiElement.Fire:
                            return Globalisation.Dictionary.fire_fire;

                        case ENineStarKiElement.Metal:
                            return Globalisation.Dictionary.fire_metal;

                        case ENineStarKiElement.Earth:
                            return Globalisation.Dictionary.fire_earth;
                    }

                    break;

                case ENineStarKiElement.Metal:

                    switch (energy2.Element)
                    {
                        case ENineStarKiElement.Water:
                            return Globalisation.Dictionary.metal_water;

                        case ENineStarKiElement.Tree:
                            return Globalisation.Dictionary.metal_tree;

                        case ENineStarKiElement.Fire:
                            return Globalisation.Dictionary.tree_fire;

                        case ENineStarKiElement.Metal:
                            return Globalisation.Dictionary.metal_metal;

                        case ENineStarKiElement.Earth:
                            return Globalisation.Dictionary.earth_metal;
                    }

                    break;

                case ENineStarKiElement.Earth:

                    switch (energy2.Element)
                    {
                        case ENineStarKiElement.Water:
                            return Globalisation.Dictionary.earth_water;

                        case ENineStarKiElement.Tree:
                            return Globalisation.Dictionary.tree_earth;

                        case ENineStarKiElement.Fire:
                            return Globalisation.Dictionary.fire_earth;

                        case ENineStarKiElement.Metal:
                            return Globalisation.Dictionary.earth_metal;

                        case ENineStarKiElement.Earth:
                            return Globalisation.Dictionary.earth_earth;
                    }

                    break;
            }

            return string.Empty;
        }

        private string GetFundamentalElementsTransformationDetails()
        {
            if (FundamentalElementsAreSupportive)
            {
                return Globalisation.Dictionary.main_element_supportive;
            }
            if (FundamentalEnergiesAreChallenging)
            {
                return Globalisation.Dictionary.main_element_challenging;
            }
            if (FundamentalEnergiesAreSame)
            {
                return Globalisation.Dictionary.main_element_same;
            }

            return string.Empty;
        }

        private string GetCharacterElementsTransformationDetails()
        {
            if (CharacterEnergiesAreSupportive)
            {
                return Globalisation.Dictionary.character_element_supportive;
            }
            if (CharacterEnergiesAreChallenging)
            {
                return Globalisation.Dictionary.character_element_challenging;
            }
            if (CharacterEnergiesAreSame)
            {
                return Globalisation.Dictionary.character_element_same;
            }

            return string.Empty;
        }

        private Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, string,
            Tuple<ETransformationType, ETransformationType>> GetTransformationDetailItem(
            ETransformationType transformationType, NineStarKiEnergy energy1, NineStarKiEnergy energy2,
            string type1Name, string type2Name, string verb, ETransformationType comparisonTransformationType1,
            ETransformationType comparisonTransformationType2)
        {
            return new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, string, Tuple<ETransformationType, ETransformationType>>(
                transformationType,
                energy1,
                energy2,
                type1Name,
                type2Name,
                verb,
                new Tuple<ETransformationType, ETransformationType>(
                    comparisonTransformationType1,
                    comparisonTransformationType2)
            );
        }

        private string GetTransformationDetails(
            params Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, string,
                Tuple<ETransformationType, ETransformationType>>[] transformationTypes)
        {
            var sb = new StringBuilder();

            foreach (var item in transformationTypes)
            {
                var transformationType = item.Item1;
                var energy1 = item.Item2;
                var energy2 = item.Item3;
                var type1 = item.Item4;
                var type2 = item.Item5;
                var verb = item.Item6;
                var transformationType1 = item.Item7.Item1;
                var transformationType2 = item.Item7.Item2;

                if (transformationType == transformationType1)
                {
                    sb.AppendLine(
                        $"<h5>{{Person1Proper}}'s {type1} element '{energy1.ElementName}' {verb} {{Person2}}'s {type2} element '{energy2.ElementName}'</h5>.");
                }
                else if (transformationType == transformationType2)
                {
                    sb.AppendLine(
                        $"<h5>{{Person2Proper}}'s {type2} element '{energy2.ElementName}' {verb} {{Person1}}'s {type1} element '{energy1.ElementName}'</h5>.");
                }

                if (sb.Length > 0)
                {
                    sb.AppendLine($"<p>{GetElementCompatibilityDetails(energy1, energy2)}</p>");
                }
            }

            return sb.ToString();
        }

        private string GetSupportiveCompatibilityDetails(ETransformationType transformationType, NineStarKiEnergy energy1, NineStarKiEnergy energy2, string type1Name, string type2Name)
        {
            var sb = new StringBuilder();

            sb.Append(GetTransformationDetails(GetTransformationDetailItem(
                transformationType,
                energy1,
                energy2,
                type1Name,
                type2Name,
                "supports",
                ETransformationType.Supports,
                ETransformationType.IsSupported)));

            return sb.ToString();
        }

        private string GetSameCompatibilityDetails(ETransformationType transformationType, NineStarKiEnergy energy1, NineStarKiEnergy energy2, string type1Name, string type2Name)
        {
            var sb = new StringBuilder();

            sb.Append(GetTransformationDetails(GetTransformationDetailItem(
                transformationType,
                energy1,
                energy2,
                type1Name,
                type2Name,
                "is the same as",
                ETransformationType.Same,
                ETransformationType.Same)));

            return sb.ToString();
        }

        private string GetChallengingCompatibilityDetails(ETransformationType transformationType, NineStarKiEnergy energy1, NineStarKiEnergy energy2, string type1Name, string type2Name)
        {
            var sb = new StringBuilder();

            sb.Append(GetTransformationDetails(GetTransformationDetailItem(
                transformationType,
                energy1,
                energy2,
                type1Name,
                type2Name,
                "challenges",
                ETransformationType.IsChallenged,
                ETransformationType.Challenges)));

            return sb.ToString();
        }

        private List<Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string>> GetAllOtherElements()
        {
            var items = new List<Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string>>();

            items.Add(new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string>
            (FundamentalEnergy1ToCharacterEnergy2TransformationType,
                _nineStarKiModel1.MainEnergy,
                _nineStarKiModel2.CharacterEnergy,
                "Fundamental",
                "Character"));

            items.Add(new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string>
            (FundamentalEnergy2ToCharacterEnergy1TransformationType,
                _nineStarKiModel2.MainEnergy,
                _nineStarKiModel1.CharacterEnergy,
                "Fundamental",
                "Character"));

            items.Add(new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string>(
                FundamentalEnergy1ToSurfaceEnergy2TransformationType,
                _nineStarKiModel1.MainEnergy,
                _nineStarKiModel2.SurfaceEnergy,
                "Fundamental",
                "Surface"));

            items.Add(new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string>(
                FundamentalEnergy2ToSurfaceEnergy1TransformationType,
                _nineStarKiModel2.MainEnergy,
                _nineStarKiModel1.SurfaceEnergy,
                "Fundamental",
                "Surface"));

            items.Add(new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string>(
                CharacterEnergy1ToSurfaceEnergy2TransformationType,
                _nineStarKiModel1.CharacterEnergy,
                _nineStarKiModel2.SurfaceEnergy,
                "Character",
                "Surface"));

            items.Add(new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string>(
                CharacterEnergy2ToSurfaceEnergy1TransformationType,
                _nineStarKiModel2.CharacterEnergy,
                _nineStarKiModel1.SurfaceEnergy,
                "Character",
                "Surface"));

            return items;
        }

        private string GetElementsCompatibility(object pronouns)
        {
            var sb = new StringBuilder();

            foreach (var item in GetAllOtherElements())
            {
                if (Score.HarmonyScore > Score.ConflictScore)
                {
                   sb.AppendLine(GetSupportiveCompatibilityDetails(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5));
                   sb.AppendLine(GetSameCompatibilityDetails(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5));
                   sb.AppendLine(GetChallengingCompatibilityDetails(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5));

                }
                else
                {
                    sb.AppendLine(GetChallengingCompatibilityDetails(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5));
                    sb.AppendLine(GetSameCompatibilityDetails(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5));
                    sb.AppendLine(GetSupportiveCompatibilityDetails(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5));
                }
            }

            return TemplateProcessor.PopulateTemplate(sb.ToString(), pronouns);
        }

        private bool IsSupportive(ETransformationType transformationType)
        {
            return new List<ETransformationType> { ETransformationType.IsSupported, ETransformationType.Supports }.Contains(transformationType);
        }

        private bool IsSame(ETransformationType transformationType)
        {
            return transformationType == ETransformationType.Same;
        }

        private bool IsChallenging(ETransformationType transformationType)
        {
            return new List<ETransformationType> { ETransformationType.IsChallenged, ETransformationType.Challenges }.Contains(transformationType);
        }

        private void CalculateScore()
        {
            switch (FundamentalEnergiesTransformationType)
            {
                case ETransformationType.Same:
                    Score.AddHarmonyScore(ECompatibilityScore.ExtremelyHigh, 6);
                    Score.AddConflictScore(ECompatibilityScore.ExtremelyLow, 6);
                    Score.AddSupportScore(ECompatibilityScore.LowToMedium, 6);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.ExtremelyHigh, 6);
                    
                    Score.AddComplementarityScore(FundamentalEnergiesAreSameEnergy ? ECompatibilityScore.ExtremelyLow : ECompatibilityScore.Low, 6);
                    Score.AddSexualChemistryScore(FundamentalEnergiesAreSameEnergy ? ESexualChemistryScore.NonExistant : ESexualChemistryScore.Low, 10);
                    Score.AddSparkScore(FundamentalEnergiesAreSameEnergy ? ECompatibilityScore.ExtremelyLow : ECompatibilityScore.Low, 10);
                    Score.AddLearningPotentialScore(FundamentalEnergiesAreSameEnergy ? ECompatibilityScore.VeryLow : ECompatibilityScore.Low, 6);
                    break;

                case ETransformationType.Supports:
                case ETransformationType.IsSupported:
                    Score.AddHarmonyScore(ECompatibilityScore.VeryHigh, 6);
                    Score.AddConflictScore(ECompatibilityScore.VeryLow, 6);
                    Score.AddSupportScore(ECompatibilityScore.ExtremelyHigh, 6);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.High, 6);
                    Score.AddComplementarityScore(ECompatibilityScore.ExtremelyHigh, 6);
                    Score.AddSexualChemistryScore(ESexualChemistryScore.MediumToHigh, 6);
                    Score.AddSparkScore(ECompatibilityScore.MediumToHigh, 6);
                    Score.AddLearningPotentialScore(ECompatibilityScore.MediumToHigh, 6);
                    break;

                case ETransformationType.Challenges:
                case ETransformationType.IsChallenged:
                    Score.AddHarmonyScore(ECompatibilityScore.ExtremelyLow, 8);
                    Score.AddConflictScore(ECompatibilityScore.ExtremelyHigh, 6);
                    Score.AddSupportScore(ECompatibilityScore.MediumToHigh, 6);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.VeryLow, 6);
                    Score.AddComplementarityScore(ECompatibilityScore.High, 6);
                    Score.AddSexualChemistryScore(ESexualChemistryScore.OffTheCharts, 10);
                    Score.AddSparkScore(ECompatibilityScore.ExtremelyHigh, 10);
                    Score.AddLearningPotentialScore(ECompatibilityScore.ExtremelyHigh, 6);
                    break;
            }

            AddScore(CharacterEnergiesTransformationType, CharacterEnergiesAreSameEnergy, 5);
            AddScore(SurfaceEnergiesTransformationType, SurfaceEnergiesAreSameEnergy, 3);

            if (!(_nineStarKiModel1.MainEnergy.Energy == _nineStarKiModel2.MainEnergy.Energy &&
                  _nineStarKiModel1.CharacterEnergy.Energy == _nineStarKiModel2.CharacterEnergy.Energy))
            {
                AddCrossReferencedScore(FundamentalEnergy1ToCharacterEnergy2TransformationType, 4);
                AddCrossReferencedScore(FundamentalEnergy2ToCharacterEnergy1TransformationType, 4);
            }
            
            
            if (!(_nineStarKiModel1.MainEnergy.Energy == _nineStarKiModel2.MainEnergy.Energy &&
                  _nineStarKiModel1.SurfaceEnergy.Energy == _nineStarKiModel2.SurfaceEnergy.Energy))
            {
                AddCrossReferencedScore(FundamentalEnergy1ToSurfaceEnergy2TransformationType, 2);
                AddCrossReferencedScore(FundamentalEnergy2ToSurfaceEnergy1TransformationType, 2);
            }

            if (!(_nineStarKiModel1.CharacterEnergy.Energy == _nineStarKiModel2.CharacterEnergy.Energy &&
                  _nineStarKiModel1.SurfaceEnergy.Energy == _nineStarKiModel2.SurfaceEnergy.Energy))
            {
                AddCrossReferencedScore(CharacterEnergy1ToSurfaceEnergy2TransformationType);
                AddCrossReferencedScore(CharacterEnergy2ToSurfaceEnergy1TransformationType);
            }
        }

        private void AddScore(ETransformationType transformationType, bool sameEnergy,  int factor = 1)
        {
            switch (transformationType)
            {
                case ETransformationType.Same:
                    Score.AddHarmonyScore(ECompatibilityScore.ExtremelyHigh, factor);
                    Score.AddConflictScore(ECompatibilityScore.ExtremelyLow, factor);
                    Score.AddSupportScore(ECompatibilityScore.Medium, factor);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.ExtremelyHigh, factor);
                    
                    Score.AddComplementarityScore(sameEnergy ? ECompatibilityScore.ExtremelyLow : ECompatibilityScore.Low, 6);
                    Score.AddSexualChemistryScore(sameEnergy ? ESexualChemistryScore.NonExistant : ESexualChemistryScore.Low, 6);
                    Score.AddSparkScore(sameEnergy ? ECompatibilityScore.ExtremelyLow : ECompatibilityScore.Low, 6);
                    Score.AddLearningPotentialScore(sameEnergy ? ECompatibilityScore.VeryLow : ECompatibilityScore.Low, 6);
                    break;

                case ETransformationType.Supports:
                case ETransformationType.IsSupported:
                    Score.AddHarmonyScore(ECompatibilityScore.VeryHigh, factor);
                    Score.AddConflictScore(ECompatibilityScore.VeryLow, factor);
                    Score.AddSupportScore(ECompatibilityScore.ExtremelyHigh, factor);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.High, factor);
                    Score.AddComplementarityScore(ECompatibilityScore.ExtremelyHigh, factor);
                    Score.AddSexualChemistryScore(ESexualChemistryScore.High, factor);
                    Score.AddSparkScore(ECompatibilityScore.High, factor);
                    Score.AddLearningPotentialScore(ECompatibilityScore.MediumToHigh, factor);
                    break;

                case ETransformationType.Challenges:
                case ETransformationType.IsChallenged:
                    Score.AddHarmonyScore(ECompatibilityScore.ExtremelyLow, factor);
                    Score.AddConflictScore(ECompatibilityScore.ExtremelyHigh, factor);
                    Score.AddSupportScore(ECompatibilityScore.MediumToHigh, factor);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.VeryLow, factor);
                    Score.AddComplementarityScore(ECompatibilityScore.High, factor);
                    Score.AddSexualChemistryScore(ESexualChemistryScore.OffTheCharts, factor);
                    Score.AddSparkScore(ECompatibilityScore.ExtremelyHigh, factor);
                    Score.AddLearningPotentialScore(ECompatibilityScore.ExtremelyHigh, factor);
                    break;
            }
        }

        private void AddCrossReferencedScore(ETransformationType transformationType, int factor = 1)
        {
            switch (transformationType)
            {
                case ETransformationType.Same:
                    Score.AddHarmonyScore(ECompatibilityScore.High, factor);
                    Score.AddConflictScore(ECompatibilityScore.Low, factor);
                    Score.AddSupportScore(ECompatibilityScore.High, factor);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.High, factor);
                    Score.AddComplementarityScore(ECompatibilityScore.Low, factor);
                    Score.AddLearningPotentialScore(ECompatibilityScore.LowToMedium, factor);
                    break;

                case ETransformationType.Supports:
                case ETransformationType.IsSupported:
                    Score.AddHarmonyScore(ECompatibilityScore.High, factor);
                    Score.AddConflictScore(ECompatibilityScore.Low, factor);
                    Score.AddSupportScore(ECompatibilityScore.High, factor);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.High, factor);
                    Score.AddComplementarityScore(ECompatibilityScore.High, factor);
                    Score.AddLearningPotentialScore(ECompatibilityScore.MediumToHigh, factor);
                    break;

                case ETransformationType.Challenges:
                case ETransformationType.IsChallenged:
                    Score.AddHarmonyScore(ECompatibilityScore.Low, factor);
                    Score.AddConflictScore(ECompatibilityScore.High, factor);
                    Score.AddSupportScore(ECompatibilityScore.MediumToHigh, factor);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.Low, factor);
                    Score.AddComplementarityScore(ECompatibilityScore.High, factor);
                    Score.AddLearningPotentialScore(ECompatibilityScore.High, factor);
                    break;
            }
        }
    }

    public class GenderCompatibility
    {
        public GenderCompatibility(NineStarKiModel nineStarKiModel1, NineStarKiModel nineStarKiModel2, CompatibilityScoreModel score)
        {
            Score = score;
            IsFundamtenalGenderSame = nineStarKiModel1.MainEnergy.YinYang == nineStarKiModel2.MainEnergy.YinYang;
            IsCharacterGenderSame = nineStarKiModel1.CharacterEnergy.YinYang == nineStarKiModel2.CharacterEnergy.YinYang;

            CalculateScore();
        }

        public CompatibilityScoreModel Score { get; }
        public bool IsFundamtenalGenderSame { get; set; }
        public bool IsCharacterGenderSame { get; set; }

        private void CalculateScore()
        {
            if (IsFundamtenalGenderSame)
            {
                Score.AddConflictScore(ECompatibilityScore.MediumToHigh);
                Score.AddComplementarityScore(ECompatibilityScore.Low);
                Score.AddSexualChemistryScore(ESexualChemistryScore.Low);
                Score.AddSparkScore(ECompatibilityScore.Low);
            }
            else
            {
                Score.AddConflictScore(ECompatibilityScore.LowToMedium);
                Score.AddComplementarityScore(ECompatibilityScore.High);
                Score.AddSexualChemistryScore(ESexualChemistryScore.OffTheCharts);
                Score.AddSparkScore(ECompatibilityScore.VeryHigh);
                Score.AddLearningPotentialScore(ECompatibilityScore.High);
            }

            if (IsCharacterGenderSame)
            {
                Score.AddComplementarityScore(ECompatibilityScore.Low);
                Score.AddSexualChemistryScore(ESexualChemistryScore.Low, 2);
                Score.AddSparkScore(ECompatibilityScore.Low, 2);
            }
            else
            {
                Score.AddComplementarityScore(ECompatibilityScore.High);
                Score.AddSexualChemistryScore(ESexualChemistryScore.OffTheCharts, 3);
                Score.AddSparkScore(ECompatibilityScore.VeryHigh, 2);
                Score.AddLearningPotentialScore(ECompatibilityScore.High);
            }
        }
    }

    public class ModalityCompatibility
    {
        public ModalityCompatibility(NineStarKiModel nineStarKiModel1, NineStarKiModel nineStarKiModel2, CompatibilityScoreModel score)
        {
            Score = score;
            IsCharacterModalitySame = nineStarKiModel1.MainEnergy.Modality == nineStarKiModel2.MainEnergy.Modality;
            IsFundamentalModalitySame = nineStarKiModel1.CharacterEnergy.Modality == nineStarKiModel2.CharacterEnergy.Modality;
            IsSurfaceModalitySame = nineStarKiModel1.SurfaceEnergy.Modality == nineStarKiModel2.SurfaceEnergy.Modality;

            CalculateScore();
        }

        public CompatibilityScoreModel Score { get; }
        public bool IsCharacterModalitySame { get; set; }
        public bool IsFundamentalModalitySame { get; set; }
        public bool IsSurfaceModalitySame { get; set; }

        private void CalculateScore()
        {
            if (IsFundamentalModalitySame)
            {
                Score.AddHarmonyScore(ECompatibilityScore.High);
                Score.AddConflictScore(ECompatibilityScore.Low);
                Score.AddMutualUnderstandingScore(ECompatibilityScore.High);
                Score.AddComplementarityScore(ECompatibilityScore.Low);
                Score.AddSexualChemistryScore(ESexualChemistryScore.VeryLow);
                Score.AddSparkScore(ECompatibilityScore.Low);
                Score.AddLearningPotentialScore(ECompatibilityScore.Low);
            }
            else
            {
                Score.AddConflictScore(ECompatibilityScore.High);
                Score.AddMutualUnderstandingScore(ECompatibilityScore.Low);
                Score.AddComplementarityScore(ECompatibilityScore.ExtremelyHigh);
                Score.AddSexualChemistryScore(ESexualChemistryScore.VeryHigh);
                Score.AddSparkScore(ECompatibilityScore.VeryHigh);
                Score.AddLearningPotentialScore(ECompatibilityScore.High);
            }
        }
    }

    public class CompatibilityDetailsModel
    {
        public ElementCompatibility ElementCompatibility { get; }
        public GenderCompatibility GenderCompatibility { get; }
        public ModalityCompatibility ModalityCompatibility { get; }
        public CompatibilityScoreModel Score { get; }
        public object Pronouns { get; }

        public CompatibilityDetailsModel(CompatibilityModel compatibilityModel)
        {
            if (compatibilityModel.NineStarKiModel1.MainEnergy == null || compatibilityModel.NineStarKiModel2.MainEnergy == null)
            {
                return;
            }

            Pronouns = new
            {
                Person1 = compatibilityModel.FirstPersonNameWithArticle,
                Person2 = compatibilityModel.SecondPersonNameWithArticle,
                Person1Proper = compatibilityModel.FirstPersonNameWithArticle.ToProperCase(),
                Person2Proper = compatibilityModel.SecondPersonNameWithArticle.ToProperCase(),
                Gender1PossessivePronoun = compatibilityModel.NineStarKiModel1?.PersonModel?.GenderPossessivePronoun,
                Gender2PossessivePronoun = compatibilityModel.NineStarKiModel2?.PersonModel?.GenderPossessivePronoun,
                Gender1Pronoun = compatibilityModel.NineStarKiModel1?.PersonModel?.GenderPronoun,
                Gender2Pronoun = compatibilityModel.NineStarKiModel2?.PersonModel?.GenderPronoun,
                Gender1PossessivePronounProper =
                compatibilityModel.NineStarKiModel1?.PersonModel?.GenderPossessivePronoun.ToProperCase(),
                Gender2PossessivePronounProper =
                compatibilityModel.NineStarKiModel2?.PersonModel?.GenderPossessivePronoun.ToProperCase(),
                Gender1PronounProper = compatibilityModel.NineStarKiModel1?.PersonModel?.GenderPronoun.ToProperCase(),
                Gender2PronounProper = compatibilityModel.NineStarKiModel2?.PersonModel?.GenderPronoun.ToProperCase()
            };

            Score = new CompatibilityScoreModel();
            ElementCompatibility = new ElementCompatibility(compatibilityModel.NineStarKiModel1, compatibilityModel.NineStarKiModel2, Score, Pronouns);
            GenderCompatibility = new GenderCompatibility(compatibilityModel.NineStarKiModel1, compatibilityModel.NineStarKiModel2, Score);
            ModalityCompatibility = new ModalityCompatibility(compatibilityModel.NineStarKiModel1, compatibilityModel.NineStarKiModel2, Score);
        }
    }
}