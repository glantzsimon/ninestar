using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Config;
using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using K9.Globalisation;

namespace K9.WebApplication.Models
{
    public class ElementCompatibility
    {
        private readonly NineStarKiModel _nineStarKiModel1;
        private readonly NineStarKiModel _nineStarKiModel2;

        public ElementCompatibility(NineStarKiModel nineStarKiModel1, NineStarKiModel nineStarKiModel2,
            CompatibilityScoreModel score)
        {
            _nineStarKiModel1 = nineStarKiModel1;
            _nineStarKiModel2 = nineStarKiModel2;
            FundamentalEnergiesTransformationType =
                nineStarKiModel1.MainEnergy.Energy.GetTransformationType(nineStarKiModel2.MainEnergy.Energy);

            FundamentalEnergy1ToCharacterEnergy2TransformationType =
                nineStarKiModel1.MainEnergy.Energy.GetTransformationType(nineStarKiModel2.CharacterEnergy.Energy);

            FundamentalEnergy1ToSurfaceEnergy2TransformationType =
                nineStarKiModel1.MainEnergy.Energy.GetTransformationType(nineStarKiModel2.SurfaceEnergy.Energy);

            FundamentalEnergy2ToCharacterEnergy1TransformationType =
                nineStarKiModel2.MainEnergy.Energy.GetTransformationType(nineStarKiModel1.CharacterEnergy.Energy);

            FundamentalEnergy2ToSurfaceEnergy1TransformationType =
                nineStarKiModel2.MainEnergy.Energy.GetTransformationType(nineStarKiModel1.SurfaceEnergy.Energy);

            CharacterEnergiesTransformationType =
                nineStarKiModel1.CharacterEnergy.Energy.GetTransformationType(nineStarKiModel2.CharacterEnergy.Energy);

            CharacterEnergy1ToSurfaceEnergy2TransformationType =
                nineStarKiModel1.CharacterEnergy.Energy.GetTransformationType(nineStarKiModel2.SurfaceEnergy.Energy);

            CharacterEnergy2ToSurfaceEnergy1TransformationType =
                nineStarKiModel2.CharacterEnergy.Energy.GetTransformationType(nineStarKiModel1.SurfaceEnergy.Energy);

            SurfaceEnergiesTransformationType =
                nineStarKiModel1.SurfaceEnergy.Energy.GetTransformationType(nineStarKiModel2.SurfaceEnergy.Energy);

            Score = score;

            FundamentalElementsTransformationDetails = GetFundamentalElementsTransformationDetails();

            CharacterElementsTransformationDetails = GetCharacterElementsTransformationDetails();

            FundamentalElementsCompatibilityDetails =
                GetElementCompatibilityDetails(_nineStarKiModel1.MainEnergy, _nineStarKiModel2.MainEnergy);

            FundamentalElementsCompatibilityDetailsTitle = GetFundamentalElementsCompatibilityTitle();

            CharacterElementsCompatibilityDetails = GetElementCompatibilityDetails(_nineStarKiModel1.CharacterEnergy,
                _nineStarKiModel2.CharacterEnergy);

            CalculateScore();

            Score.CalculateAverages();

            AllOtherElementsCompatibility = GetElementsCompatibility();
        }

        public CompatibilityScoreModel Score { get; }

        [ScriptIgnore] public ETransformationType FundamentalEnergiesTransformationType { get; }

        [ScriptIgnore] public ETransformationType FundamentalEnergy1ToCharacterEnergy2TransformationType { get; }

        [ScriptIgnore] public ETransformationType FundamentalEnergy1ToSurfaceEnergy2TransformationType { get; }

        [ScriptIgnore] public ETransformationType FundamentalEnergy2ToCharacterEnergy1TransformationType { get; }

        [ScriptIgnore] public ETransformationType FundamentalEnergy2ToSurfaceEnergy1TransformationType { get; }

        [ScriptIgnore] public ETransformationType CharacterEnergiesTransformationType { get; }

        [ScriptIgnore] public ETransformationType CharacterEnergy1ToSurfaceEnergy2TransformationType { get; }

        [ScriptIgnore] public ETransformationType CharacterEnergy2ToSurfaceEnergy1TransformationType { get; }

        [ScriptIgnore] public ETransformationType SurfaceEnergiesTransformationType { get; }

        public string FundamentalElementsTransformationDetails { get; }
        public string CharacterElementsTransformationDetails { get; }
        public string FundamentalElementsCompatibilityDetails { get; }

        [ScriptIgnore] public string FundamentalElementsCompatibilityDetailsTitle { get; }

        public string CharacterElementsCompatibilityDetails { get; }

        public string AllOtherElementsCompatibility { get; }

        [ScriptIgnore]
        public bool FundamentalElementsAreSupportive => IsSupportive(FundamentalEnergiesTransformationType);

        [ScriptIgnore]
        public bool FundamentalEnergiesAreSame => FundamentalEnergiesTransformationType == ETransformationType.Sibling;

        [ScriptIgnore]
        public bool FundamentalEnergiesAreChallenging => IsChallenging(FundamentalEnergiesTransformationType);

        [ScriptIgnore]
        public bool FundamentalEnergiesAreSameEnergy =>
            _nineStarKiModel1.MainEnergy.Energy == _nineStarKiModel2.MainEnergy.Energy;

        [ScriptIgnore] public bool CharacterEnergiesAreSupportive => IsSupportive(CharacterEnergiesTransformationType);

        [ScriptIgnore]
        public bool CharacterEnergiesAreSame => CharacterEnergiesTransformationType == ETransformationType.Sibling;

        [ScriptIgnore]
        public bool CharacterEnergiesAreChallenging => IsChallenging(CharacterEnergiesTransformationType);

        [ScriptIgnore]
        public bool CharacterEnergiesAreSameEnergy =>
            _nineStarKiModel1.CharacterEnergy.Energy == _nineStarKiModel2.CharacterEnergy.Energy;

        [ScriptIgnore] public bool SurfaceEnergiesAreSupportive => IsSupportive(SurfaceEnergiesTransformationType);

        [ScriptIgnore]
        public bool SurfaceEnergiesAreSame => SurfaceEnergiesTransformationType == ETransformationType.Sibling;

        [ScriptIgnore] public bool SurfaceEnergiesAreChallenging => IsChallenging(SurfaceEnergiesTransformationType);

        [ScriptIgnore]
        public bool SurfaceEnergiesAreSameEnergy =>
            _nineStarKiModel1.SurfaceEnergy.Energy == _nineStarKiModel2.SurfaceEnergy.Energy;

        [ScriptIgnore]
        public bool FundamentalEnergy1ToCharacterEnergy2IsSupportive =>
            IsSupportive(FundamentalEnergy1ToCharacterEnergy2TransformationType);

        [ScriptIgnore]
        public bool FundamentalEnergy2ToCharacterEnergy1IsSupportive =>
            IsSupportive(FundamentalEnergy2ToCharacterEnergy1TransformationType);

        [ScriptIgnore]
        public bool FundamentalEnergy1ToSurfaceEnergy2IsSupportive =>
            IsSupportive(FundamentalEnergy1ToSurfaceEnergy2TransformationType);

        [ScriptIgnore]
        public bool FundamentalEnergy2ToSurfaceEnergy1IsSupportive =>
            IsSupportive(FundamentalEnergy2ToSurfaceEnergy1TransformationType);

        [ScriptIgnore]
        public bool CharacterEnergy1ToSurfaceEnergy2IsSupportive =>
            IsSupportive(CharacterEnergy1ToSurfaceEnergy2TransformationType);

        [ScriptIgnore]
        public bool CharacterEnergy2ToSurfaceEnergy1IsSupportive =>
            IsSupportive(CharacterEnergy2ToSurfaceEnergy1TransformationType);

        [ScriptIgnore]
        public bool FundamentalEnergy1ToCharacterEnergy2IsSame =>
            IsSame(FundamentalEnergy1ToCharacterEnergy2TransformationType);

        [ScriptIgnore]
        public bool FundamentalEnergy2ToCharacterEnergy1IsSame =>
            IsSame(FundamentalEnergy2ToCharacterEnergy1TransformationType);

        [ScriptIgnore]
        public bool FundamentalEnergy1ToSurfaceEnergy2IsSame =>
            IsSame(FundamentalEnergy1ToSurfaceEnergy2TransformationType);

        [ScriptIgnore]
        public bool FundamentalEnergy2ToSurfaceEnergy1IsSame =>
            IsSame(FundamentalEnergy2ToSurfaceEnergy1TransformationType);

        [ScriptIgnore]
        public bool CharacterEnergy1ToSurfaceEnergy2IsSame =>
            IsSame(CharacterEnergy1ToSurfaceEnergy2TransformationType);

        [ScriptIgnore]
        public bool CharacterEnergy2ToSurfaceEnergy1IsSame =>
            IsSame(CharacterEnergy2ToSurfaceEnergy1TransformationType);

        [ScriptIgnore]
        public bool FundamentalEnergy1ToCharacterEnergy2IsChallenging =>
            IsChallenging(FundamentalEnergy1ToCharacterEnergy2TransformationType);

        [ScriptIgnore]
        public bool FundamentalEnergy2ToCharacterEnergy1IsChallenging =>
            IsChallenging(FundamentalEnergy2ToCharacterEnergy1TransformationType);

        [ScriptIgnore]
        public bool FundamentalEnergy1ToSurfaceEnergy2IsChallenging =>
            IsChallenging(FundamentalEnergy1ToSurfaceEnergy2TransformationType);

        [ScriptIgnore]
        public bool FundamentalEnergy2ToSurfaceEnergy1IsChallenging =>
            IsChallenging(FundamentalEnergy2ToSurfaceEnergy1TransformationType);

        [ScriptIgnore]
        public bool CharacterEnergy1ToSurfaceEnergy2IsChallenging =>
            IsChallenging(CharacterEnergy1ToSurfaceEnergy2TransformationType);

        [ScriptIgnore]
        public bool CharacterEnergy2ToSurfaceEnergy1IsChallenging =>
            IsChallenging(CharacterEnergy2ToSurfaceEnergy1TransformationType);

        public (double SupportivePercentage, double ChallengingPercentage, double SiblingPercentage)
            GetTransformationFractions()
        {
            // Golden ratio
            double surfaceFactor = 3;
            double characterFactor = 5;
            double mainFactor = 8;

            double supportiveTotal = 0;
            double challengingTotal = 0;
            double siblingTotal = 0;

            if (FundamentalElementsAreSupportive)
            {
                supportiveTotal += mainFactor * 2;
            }

            if (FundamentalEnergiesAreSame || FundamentalEnergiesAreSameEnergy)
            {
                siblingTotal += mainFactor * 2;
            }

            if (FundamentalEnergiesAreChallenging)
            {
                challengingTotal += mainFactor * 2;
            }

            if (CharacterEnergiesAreSupportive)
            {
                supportiveTotal += characterFactor * 2;
            }

            if (CharacterEnergiesAreSame || CharacterEnergiesAreSameEnergy)
            {
                siblingTotal += characterFactor * 2;
            }

            if (CharacterEnergiesAreChallenging)
            {
                challengingTotal += characterFactor * 2;
            }

            if (SurfaceEnergiesAreSupportive)
            {
                supportiveTotal += surfaceFactor * 2;
            }

            if (SurfaceEnergiesAreSame || SurfaceEnergiesAreSameEnergy)
            {
                siblingTotal += surfaceFactor * 2;
            }

            if (SurfaceEnergiesAreChallenging)
            {
                challengingTotal += surfaceFactor * 2;
            }

            if (FundamentalEnergy1ToCharacterEnergy2IsSupportive || FundamentalEnergy2ToCharacterEnergy1IsSupportive)
            {
                supportiveTotal += characterFactor + mainFactor;
            }

            if (FundamentalEnergy1ToSurfaceEnergy2IsSupportive || FundamentalEnergy2ToSurfaceEnergy1IsSupportive)
            {
                supportiveTotal += surfaceFactor + mainFactor;
            }

            if (CharacterEnergy1ToSurfaceEnergy2IsSupportive || CharacterEnergy2ToSurfaceEnergy1IsSupportive)
            {
                supportiveTotal += surfaceFactor + characterFactor;
            }

            if (FundamentalEnergy1ToCharacterEnergy2IsSame || FundamentalEnergy2ToCharacterEnergy1IsSame)
            {
                siblingTotal += mainFactor + characterFactor;
            }

            if (FundamentalEnergy1ToSurfaceEnergy2IsSame || FundamentalEnergy2ToSurfaceEnergy1IsSame)
            {
                siblingTotal += mainFactor + surfaceFactor;
            }

            if (CharacterEnergy1ToSurfaceEnergy2IsSame || CharacterEnergy2ToSurfaceEnergy1IsSame)
            {
                siblingTotal += characterFactor + surfaceFactor;
            }

            if (FundamentalEnergy1ToCharacterEnergy2IsChallenging || FundamentalEnergy2ToCharacterEnergy1IsChallenging)
            {
                challengingTotal += mainFactor + characterFactor;
            }

            if (FundamentalEnergy1ToSurfaceEnergy2IsChallenging || FundamentalEnergy2ToSurfaceEnergy1IsChallenging)
            {
                challengingTotal += mainFactor + surfaceFactor;
            }

            if (CharacterEnergy1ToSurfaceEnergy2IsChallenging || CharacterEnergy2ToSurfaceEnergy1IsChallenging)
            {
                challengingTotal += characterFactor + surfaceFactor;
            }

            var total = supportiveTotal + challengingTotal + siblingTotal;
            var supportivePercentage = (supportiveTotal / total) * 100;
            var challengingPercentage = (challengingTotal / total) * 100;
            var siblingPercentage = (siblingTotal / total) * 100;

            return (supportivePercentage, challengingPercentage, siblingPercentage);
        }

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
                return TemplateParser.Parse(Globalisation.Dictionary.main_element_supportive,
                    new { DefaultValuesConfiguration.Instance.BaseImagesPath });
            }

            if (FundamentalEnergiesAreChallenging)
            {
                return TemplateParser.Parse(Globalisation.Dictionary.main_element_challenging,
                    new { DefaultValuesConfiguration.Instance.BaseImagesPath });
            }

            if (FundamentalEnergiesAreSame)
            {
                return TemplateParser.Parse(Globalisation.Dictionary.main_element_same,
                    new { DefaultValuesConfiguration.Instance.BaseImagesPath });
            }

            return string.Empty;
        }

        private string GetCharacterElementsTransformationDetails()
        {
            if (CharacterEnergiesAreSupportive)
            {
                return TemplateParser.Parse(Globalisation.Dictionary.character_element_supportive,
                    new { DefaultValuesConfiguration.Instance.BaseImagesPath });
            }

            if (CharacterEnergiesAreChallenging)
            {
                return TemplateParser.Parse(Globalisation.Dictionary.character_element_challenging,
                    new { DefaultValuesConfiguration.Instance.BaseImagesPath });
            }

            if (CharacterEnergiesAreSame)
            {
                return TemplateParser.Parse(Globalisation.Dictionary.character_element_same,
                    new { DefaultValuesConfiguration.Instance.BaseImagesPath });
            }

            return string.Empty;
        }

        private Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, string,
                Tuple<ETransformationType, ETransformationType, PersonModel, PersonModel, bool>>
            GetTransformationDetailItem(
                ETransformationType transformationType, NineStarKiEnergy energy1, NineStarKiEnergy energy2,
                string type1Name, string type2Name, string verb, ETransformationType comparisonTransformationType1,
                ETransformationType comparisonTransformationType2, PersonModel person1, PersonModel person2,
                bool titleOnly)
        {
            return new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, string,
                Tuple<ETransformationType, ETransformationType, PersonModel, PersonModel, bool>>(
                transformationType,
                energy1,
                energy2,
                type1Name,
                type2Name,
                verb,
                new Tuple<ETransformationType, ETransformationType, PersonModel, PersonModel, bool>(
                    comparisonTransformationType1,
                    comparisonTransformationType2,
                    person1,
                    person2,
                    titleOnly)
            );
        }

        private string GetTransformationDetails(
            params Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, string,
                Tuple<ETransformationType, ETransformationType, PersonModel, PersonModel, bool>>[] transformationTypes)
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
                var person1 = item.Item7.Item3.Name ?? $"the {Globalisation.Dictionary.FirstPerson.ToLower()}";
                var person2 = item.Item7.Item4.Name ?? $"the {Globalisation.Dictionary.SecondPerson.ToLower()}";
                var person1Proper = person1?.ToProperCase();
                var person2Proper = person2?.ToProperCase();
                var titleOnly = item.Item7.Item5;

                if (transformationType == transformationType1)
                {
                    sb.AppendLine(
                        $"<h5>{person1Proper}'s {type1} element ({energy1.ElementName}) {verb} {person2}'s {type2} element ({energy2.ElementName})</h5>");
                }
                else if (transformationType == transformationType2)
                {
                    sb.AppendLine(
                        $"<h5>{person2Proper}'s {type2} element ({energy2.ElementName}) {verb} {person1}'s {type1} element ({energy1.ElementName})</h5>");
                }

                if (sb.Length > 0 && !titleOnly)
                {
                    sb.AppendLine($"<p>{GetElementCompatibilityDetails(energy1, energy2)}</p>");
                }
            }

            return sb.ToString();
        }

        private string GetSupportiveCompatibilityDetails(ETransformationType transformationType,
            NineStarKiEnergy energy1, NineStarKiEnergy energy2, string type1Name, string type2Name, PersonModel person1,
            PersonModel person2, bool titleOnly = false)
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
                ETransformationType.IsSupported,
                person1,
                person2,
                titleOnly)));

            return sb.ToString();
        }

        private string GetSameCompatibilityDetails(ETransformationType transformationType, NineStarKiEnergy energy1,
            NineStarKiEnergy energy2, string type1Name, string type2Name, PersonModel person1, PersonModel person2,
            bool titleOnly = false)
        {
            var sb = new StringBuilder();

            sb.Append(GetTransformationDetails(GetTransformationDetailItem(
                transformationType,
                energy1,
                energy2,
                type1Name,
                type2Name,
                "is the same as",
                ETransformationType.Sibling,
                ETransformationType.Sibling,
                person1,
                person2,
                titleOnly)));

            return sb.ToString();
        }

        private string GetChallengingCompatibilityDetails(ETransformationType transformationType,
            NineStarKiEnergy energy1, NineStarKiEnergy energy2, string type1Name, string type2Name, PersonModel person1,
            PersonModel person2, bool titleOnly = false)
        {
            var sb = new StringBuilder();

            sb.Append(GetTransformationDetails(GetTransformationDetailItem(
                transformationType,
                energy1,
                energy2,
                type1Name,
                type2Name,
                "challenges",
                ETransformationType.Controls,
                ETransformationType.IsControlled,
                person1,
                person2,
                titleOnly)));

            return sb.ToString();
        }

        private List<Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, PersonModel,
            PersonModel>> GetAllOtherElements()
        {
            var items =
                new List<Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, PersonModel,
                    PersonModel>>
                {
                    new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, PersonModel,
                        PersonModel>
                    (FundamentalEnergiesTransformationType,
                        _nineStarKiModel1.MainEnergy,
                        _nineStarKiModel2.MainEnergy,
                        "Fundamental",
                        "Fundamental",
                        _nineStarKiModel1.PersonModel,
                        _nineStarKiModel2.PersonModel),
                    new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, PersonModel,
                        PersonModel>
                    (CharacterEnergiesTransformationType,
                        _nineStarKiModel1.CharacterEnergy,
                        _nineStarKiModel2.CharacterEnergy,
                        "Character",
                        "Character",
                        _nineStarKiModel1.PersonModel,
                        _nineStarKiModel2.PersonModel),
                    new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, PersonModel,
                        PersonModel>
                    (FundamentalEnergy1ToCharacterEnergy2TransformationType,
                        _nineStarKiModel1.MainEnergy,
                        _nineStarKiModel2.CharacterEnergy,
                        "Fundamental",
                        "Character",
                        _nineStarKiModel1.PersonModel,
                        _nineStarKiModel2.PersonModel),
                    new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, PersonModel,
                        PersonModel>
                    (FundamentalEnergy2ToCharacterEnergy1TransformationType,
                        _nineStarKiModel2.MainEnergy,
                        _nineStarKiModel1.CharacterEnergy,
                        "Fundamental",
                        "Character",
                        _nineStarKiModel2.PersonModel,
                        _nineStarKiModel1.PersonModel),
                    new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, PersonModel,
                        PersonModel>(
                        FundamentalEnergy1ToSurfaceEnergy2TransformationType,
                        _nineStarKiModel1.MainEnergy,
                        _nineStarKiModel2.SurfaceEnergy,
                        "Fundamental",
                        "Surface",
                        _nineStarKiModel1.PersonModel,
                        _nineStarKiModel2.PersonModel),
                    new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, PersonModel,
                        PersonModel>(
                        FundamentalEnergy2ToSurfaceEnergy1TransformationType,
                        _nineStarKiModel2.MainEnergy,
                        _nineStarKiModel1.SurfaceEnergy,
                        "Fundamental",
                        "Surface",
                        _nineStarKiModel2.PersonModel,
                        _nineStarKiModel1.PersonModel),
                    new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, PersonModel,
                        PersonModel>(
                        CharacterEnergy1ToSurfaceEnergy2TransformationType,
                        _nineStarKiModel1.CharacterEnergy,
                        _nineStarKiModel2.SurfaceEnergy,
                        "Character",
                        "Surface",
                        _nineStarKiModel1.PersonModel,
                        _nineStarKiModel2.PersonModel),
                    new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, PersonModel,
                        PersonModel>(
                        CharacterEnergy2ToSurfaceEnergy1TransformationType,
                        _nineStarKiModel2.CharacterEnergy,
                        _nineStarKiModel1.SurfaceEnergy,
                        "Character",
                        "Surface",
                        _nineStarKiModel2.PersonModel,
                        _nineStarKiModel1.PersonModel)
                };

            return items;
        }

        private string GetFundamentalElementsCompatibilityTitle()
        {
            var sb = new StringBuilder();

            var mainDetails =
                new Tuple<ETransformationType, NineStarKiEnergy, NineStarKiEnergy, string, string, PersonModel,
                    PersonModel>
                (FundamentalEnergiesTransformationType,
                    _nineStarKiModel1.MainEnergy,
                    _nineStarKiModel2.MainEnergy,
                    "Fundamental",
                    "Fundamental",
                    _nineStarKiModel1.PersonModel,
                    _nineStarKiModel2.PersonModel);

            sb.AppendLine(GetSupportiveCompatibilityDetails(mainDetails.Item1, mainDetails.Item2, mainDetails.Item3,
                mainDetails.Item4, mainDetails.Item5, mainDetails.Item6, mainDetails.Item7, true));
            sb.AppendLine(GetSameCompatibilityDetails(mainDetails.Item1, mainDetails.Item2, mainDetails.Item3,
                mainDetails.Item4, mainDetails.Item5, mainDetails.Item6, mainDetails.Item7, true));
            sb.AppendLine(GetChallengingCompatibilityDetails(mainDetails.Item1, mainDetails.Item2, mainDetails.Item3,
                mainDetails.Item4, mainDetails.Item5, mainDetails.Item6, mainDetails.Item7, true));

            return sb.ToString();
        }

        private string GetElementsCompatibility()
        {
            var sb = new StringBuilder();
            var sbSupport = new StringBuilder();
            var sbChallenge = new StringBuilder();
            var sbSame = new StringBuilder();

            var allOtherElements = GetAllOtherElements();
            var challengingItems = allOtherElements.Where(e =>
                e.Item1 == ETransformationType.Controls || e.Item1 == ETransformationType.IsControlled);
            var supportiveItems = allOtherElements.Where(e =>
                e.Item1 == ETransformationType.Supports || e.Item1 == ETransformationType.IsSupported);
            var sameItems = allOtherElements.Where(e => e.Item1 == ETransformationType.Sibling);

            var fractions = GetTransformationFractions();

            if (supportiveItems.Any())
            {
                sbSupport.AppendLine(
                    $"<h4>{Globalisation.Dictionary.SupportiveElements} ({fractions.SupportivePercentage.ToString("0")}%)</h4>");
                foreach (var item in supportiveItems)
                {
                    sbSupport.AppendLine(GetSupportiveCompatibilityDetails(item.Item1, item.Item2, item.Item3,
                        item.Item4, item.Item5, item.Item6, item.Item7));
                }
            }

            if (sameItems.Any())
            {
                sbSame.AppendLine(
                    $"<h4>{Globalisation.Dictionary.SiblingElements} ({fractions.SiblingPercentage.ToString("0")}%)</h4>");
                foreach (var item in sameItems)
                {
                    sbSame.AppendLine(GetSameCompatibilityDetails(item.Item1, item.Item2, item.Item3, item.Item4,
                        item.Item5, item.Item6, item.Item7));
                }
            }

            if (challengingItems.Any())
            {
                sbChallenge.AppendLine(
                    $"<h4>{Globalisation.Dictionary.ChallengingElements} ({fractions.ChallengingPercentage.ToString("0")}%)</h4>");
                foreach (var item in challengingItems)
                {
                    sbChallenge.AppendLine(GetChallengingCompatibilityDetails(item.Item1, item.Item2, item.Item3,
                        item.Item4, item.Item5, item.Item6, item.Item7));
                }
            }

            if (Score.HarmonyScore > Score.ConflictScore)
            {
                sb.Append(sbSupport);
                sb.Append(sbSame);
                sb.Append(sbChallenge);
            }
            else
            {
                sb.Append(sbChallenge);
                sb.Append(sbSame);
                sb.Append(sbSupport);
            }

            return sb.ToString();
        }

        private bool IsSupportive(ETransformationType transformationType)
        {
            return new List<ETransformationType> { ETransformationType.IsSupported, ETransformationType.Supports }
                .Contains(transformationType);
        }

        private bool IsSame(ETransformationType transformationType)
        {
            return transformationType == ETransformationType.Sibling;
        }

        private bool IsChallenging(ETransformationType transformationType)
        {
            return new List<ETransformationType> { ETransformationType.IsControlled, ETransformationType.Controls }
                .Contains(transformationType);
        }

        private void CalculateScore()
        {
            switch (FundamentalEnergiesTransformationType)
            {
                case ETransformationType.Sibling:
                    Score.AddSameScore(ECompatibilityScore.ExtremelyHigh, 9);

                    Score.AddHarmonyScore(ECompatibilityScore.ExtremelyHigh, 9);
                    Score.AddConflictScore(ECompatibilityScore.ExtremelyLow, 9);
                    Score.AddSupportScore(ECompatibilityScore.LowToMedium, 9);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.ExtremelyHigh, 9);

                    Score.AddComplementarityScore(
                        FundamentalEnergiesAreSameEnergy ? ECompatibilityScore.ExtremelyLow : ECompatibilityScore.Low,
                        9);
                    Score.AddSexualChemistryScore(
                        FundamentalEnergiesAreSameEnergy
                            ? ESexualChemistryScore.NonExistant
                            : ESexualChemistryScore.Low, 20);
                    Score.AddSparkScore(
                        FundamentalEnergiesAreSameEnergy ? ECompatibilityScore.ExtremelyLow : ECompatibilityScore.Low,
                        20);
                    Score.AddLearningPotentialScore(
                        FundamentalEnergiesAreSameEnergy ? ECompatibilityScore.VeryLow : ECompatibilityScore.Low, 9);
                    break;

                case ETransformationType.Supports:
                case ETransformationType.IsSupported:
                    Score.AddSupportiveScore(ECompatibilityScore.ExtremelyHigh, 9);

                    Score.AddHarmonyScore(ECompatibilityScore.VeryHigh, 9);
                    Score.AddConflictScore(ECompatibilityScore.VeryLow, 9);
                    Score.AddSupportScore(ECompatibilityScore.ExtremelyHigh, 9);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.High, 9);
                    Score.AddComplementarityScore(ECompatibilityScore.ExtremelyHigh, 9);
                    Score.AddSexualChemistryScore(ESexualChemistryScore.MediumToHigh, 9);
                    Score.AddSparkScore(ECompatibilityScore.MediumToHigh, 9);
                    Score.AddLearningPotentialScore(ECompatibilityScore.MediumToHigh, 9);
                    break;

                case ETransformationType.Controls:
                case ETransformationType.IsControlled:
                    Score.AddChallengingScore(ECompatibilityScore.ExtremelyHigh, 9);

                    Score.AddHarmonyScore(ECompatibilityScore.ExtremelyLow, 12);
                    Score.AddConflictScore(ECompatibilityScore.ExtremelyHigh, 9);
                    Score.AddSupportScore(ECompatibilityScore.MediumToHigh, 9);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.VeryLow, 9);
                    Score.AddComplementarityScore(ECompatibilityScore.High, 9);
                    Score.AddSexualChemistryScore(ESexualChemistryScore.OffTheCharts, 20);
                    Score.AddSparkScore(ECompatibilityScore.ExtremelyHigh, 20);
                    Score.AddLearningPotentialScore(ECompatibilityScore.ExtremelyHigh, 9);
                    break;
            }

            AddScore(CharacterEnergiesTransformationType, CharacterEnergiesAreSameEnergy, 4, 5);
            AddScore(SurfaceEnergiesTransformationType, SurfaceEnergiesAreSameEnergy, 2);

            if (!(_nineStarKiModel1.MainEnergy.Energy == _nineStarKiModel2.MainEnergy.Energy &&
                  _nineStarKiModel1.CharacterEnergy.Energy == _nineStarKiModel2.CharacterEnergy.Energy))
            {
                AddCrossReferencedScore(FundamentalEnergy1ToCharacterEnergy2TransformationType, 3);
                AddCrossReferencedScore(FundamentalEnergy2ToCharacterEnergy1TransformationType, 3);
            }

            if (!(_nineStarKiModel1.MainEnergy.Energy == _nineStarKiModel2.MainEnergy.Energy &&
                  _nineStarKiModel1.SurfaceEnergy.Energy == _nineStarKiModel2.SurfaceEnergy.Energy))
            {
                AddCrossReferencedScore(FundamentalEnergy1ToSurfaceEnergy2TransformationType, 1);
                AddCrossReferencedScore(FundamentalEnergy2ToSurfaceEnergy1TransformationType, 1);
            }

            if (!(_nineStarKiModel1.CharacterEnergy.Energy == _nineStarKiModel2.CharacterEnergy.Energy &&
                  _nineStarKiModel1.SurfaceEnergy.Energy == _nineStarKiModel2.SurfaceEnergy.Energy))
            {
                AddCrossReferencedScore(CharacterEnergy1ToSurfaceEnergy2TransformationType);
                AddCrossReferencedScore(CharacterEnergy2ToSurfaceEnergy1TransformationType);
            }
        }

        private void AddScore(ETransformationType transformationType, bool sameEnergy, int factor = 1,
            int sparkFactor = 0)
        {
            switch (transformationType)
            {
                case ETransformationType.Sibling:
                    Score.AddSameScore(ECompatibilityScore.ExtremelyHigh, factor);

                    Score.AddHarmonyScore(ECompatibilityScore.ExtremelyHigh, factor);
                    Score.AddConflictScore(ECompatibilityScore.ExtremelyLow, factor);
                    Score.AddSupportScore(ECompatibilityScore.Medium, factor);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.ExtremelyHigh, factor);

                    Score.AddComplementarityScore(
                        sameEnergy ? ECompatibilityScore.ExtremelyLow : ECompatibilityScore.Low, factor);
                    Score.AddSexualChemistryScore(
                        sameEnergy ? ESexualChemistryScore.NonExistant : ESexualChemistryScore.Low,
                        factor + sparkFactor);
                    Score.AddSparkScore(sameEnergy ? ECompatibilityScore.ExtremelyLow : ECompatibilityScore.Low,
                        factor + sparkFactor);
                    Score.AddLearningPotentialScore(sameEnergy ? ECompatibilityScore.VeryLow : ECompatibilityScore.Low,
                        factor);
                    break;

                case ETransformationType.Supports:
                case ETransformationType.IsSupported:
                    Score.AddSupportiveScore(ECompatibilityScore.ExtremelyHigh, factor);

                    Score.AddHarmonyScore(ECompatibilityScore.VeryHigh, factor);
                    Score.AddConflictScore(ECompatibilityScore.VeryLow, factor);
                    Score.AddSupportScore(ECompatibilityScore.ExtremelyHigh, factor);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.High, factor);
                    Score.AddComplementarityScore(ECompatibilityScore.ExtremelyHigh, factor);
                    Score.AddSexualChemistryScore(ESexualChemistryScore.High, factor);
                    Score.AddSparkScore(ECompatibilityScore.High, factor);
                    Score.AddLearningPotentialScore(ECompatibilityScore.MediumToHigh, factor);
                    break;

                case ETransformationType.Controls:
                case ETransformationType.IsControlled:
                    Score.AddChallengingScore(ECompatibilityScore.ExtremelyHigh, factor);

                    Score.AddHarmonyScore(ECompatibilityScore.ExtremelyLow, factor);
                    Score.AddConflictScore(ECompatibilityScore.ExtremelyHigh, factor);
                    Score.AddSupportScore(ECompatibilityScore.MediumToHigh, factor);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.VeryLow, factor);
                    Score.AddComplementarityScore(ECompatibilityScore.High, factor);
                    Score.AddSexualChemistryScore(ESexualChemistryScore.OffTheCharts, factor + sparkFactor);
                    Score.AddSparkScore(ECompatibilityScore.ExtremelyHigh, factor + sparkFactor);
                    Score.AddLearningPotentialScore(ECompatibilityScore.ExtremelyHigh, factor);
                    break;
            }
        }

        private void AddCrossReferencedScore(ETransformationType transformationType, int factor = 1)
        {
            switch (transformationType)
            {
                case ETransformationType.Sibling:
                    Score.AddSameScore(ECompatibilityScore.ExtremelyHigh, factor);

                    Score.AddHarmonyScore(ECompatibilityScore.High, factor);
                    Score.AddConflictScore(ECompatibilityScore.Low, factor);
                    Score.AddSupportScore(ECompatibilityScore.High, factor);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.High, factor);
                    Score.AddComplementarityScore(ECompatibilityScore.Low, factor);
                    Score.AddLearningPotentialScore(ECompatibilityScore.LowToMedium, factor);
                    break;

                case ETransformationType.Supports:
                case ETransformationType.IsSupported:
                    Score.AddSupportiveScore(ECompatibilityScore.ExtremelyHigh, factor);

                    Score.AddHarmonyScore(ECompatibilityScore.High, factor);
                    Score.AddConflictScore(ECompatibilityScore.Low, factor);
                    Score.AddSupportScore(ECompatibilityScore.High, factor);
                    Score.AddMutualUnderstandingScore(ECompatibilityScore.High, factor);
                    Score.AddComplementarityScore(ECompatibilityScore.High, factor);
                    Score.AddLearningPotentialScore(ECompatibilityScore.MediumToHigh, factor);
                    break;

                case ETransformationType.Controls:
                case ETransformationType.IsControlled:
                    Score.AddChallengingScore(ECompatibilityScore.ExtremelyHigh, factor);

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
        private readonly NineStarKiModel _nineStarKiModel1;
        private readonly NineStarKiModel _nineStarKiModel2;

        public GenderCompatibility(NineStarKiModel nineStarKiModel1, NineStarKiModel nineStarKiModel2,
            CompatibilityScoreModel score)
        {
            _nineStarKiModel1 = nineStarKiModel1;
            _nineStarKiModel2 = nineStarKiModel2;
            Score = score;
            IsFundamtenalGenderSame = nineStarKiModel1.MainEnergy.YinYang == nineStarKiModel2.MainEnergy.YinYang;
            IsCharacterGenderSame =
                nineStarKiModel1.CharacterEnergy.YinYang == nineStarKiModel2.CharacterEnergy.YinYang;

            CalculateScore();
        }

        public CompatibilityScoreModel Score { get; }

        [ScriptIgnore] public bool IsFundamtenalGenderSame { get; set; }

        [ScriptIgnore] public bool IsCharacterGenderSame { get; set; }

        [ScriptIgnore] public bool IsBothGenderSame => IsFundamtenalGenderSame && IsCharacterGenderSame;

        [ScriptIgnore]
        public ENineStarKiYinYang FundamentalGenderSameYinYang => IsFundamtenalGenderSame
            ? _nineStarKiModel1.MainEnergy.YinYang
            : ENineStarKiYinYang.Unspecified;

        [ScriptIgnore]
        public ENineStarKiYinYang CharacterGenderSameYinYang => IsCharacterGenderSame
            ? _nineStarKiModel1.CharacterEnergy.YinYang
            : ENineStarKiYinYang.Unspecified;

        private void CalculateScore()
        {
            if (IsFundamtenalGenderSame && IsCharacterGenderSame)
            {
                Score.AddComplementarityScore(ECompatibilityScore.VeryLow, 10);
                Score.AddSexualChemistryScore(ESexualChemistryScore.NonExistant, 10);
                Score.AddSparkScore(ECompatibilityScore.VeryLow, 10);
            }
            else if (!IsFundamtenalGenderSame && !IsCharacterGenderSame)
            {
                Score.AddComplementarityScore(ECompatibilityScore.VeryHigh, 10);
                Score.AddSexualChemistryScore(ESexualChemistryScore.OffTheCharts, 10);
                Score.AddSparkScore(ECompatibilityScore.VeryHigh, 10);
            }
            else
            {
                if (!IsFundamtenalGenderSame)
                {
                    Score.AddComplementarityScore(ECompatibilityScore.VeryHigh, 5);
                    Score.AddSexualChemistryScore(ESexualChemistryScore.OffTheCharts, 5);
                    Score.AddSparkScore(ECompatibilityScore.VeryHigh, 5);
                }
                else
                {
                    Score.AddComplementarityScore(ECompatibilityScore.VeryLow, 5);
                    Score.AddSexualChemistryScore(ESexualChemistryScore.VeryLow, 5);
                    Score.AddSparkScore(ECompatibilityScore.VeryLow, 5);
                }

                if (IsCharacterGenderSame)
                {
                    Score.AddComplementarityScore(ECompatibilityScore.VeryHigh, 5);
                    Score.AddSexualChemistryScore(ESexualChemistryScore.OffTheCharts, 5);
                    Score.AddSparkScore(ECompatibilityScore.VeryHigh, 5);
                }
                else
                {
                    Score.AddComplementarityScore(ECompatibilityScore.VeryLow, 5);
                    Score.AddSexualChemistryScore(ESexualChemistryScore.VeryLow, 5);
                    Score.AddSparkScore(ECompatibilityScore.VeryLow, 5);
                }
            }
        }
    }

    public class ModalityCompatibility
    {
        public ModalityCompatibility(NineStarKiModel nineStarKiModel1, NineStarKiModel nineStarKiModel2,
            CompatibilityScoreModel score)
        {
            Score = score;
            IsCharacterModalitySame = nineStarKiModel1.MainEnergy.Modality == nineStarKiModel2.MainEnergy.Modality;
            IsFundamentalModalitySame =
                nineStarKiModel1.CharacterEnergy.Modality == nineStarKiModel2.CharacterEnergy.Modality;
            IsSurfaceModalitySame = nineStarKiModel1.SurfaceEnergy.Modality == nineStarKiModel2.SurfaceEnergy.Modality;

            CalculateScore();
        }

        public CompatibilityScoreModel Score { get; }

        [ScriptIgnore] public bool IsCharacterModalitySame { get; set; }

        [ScriptIgnore] public bool IsFundamentalModalitySame { get; set; }

        [ScriptIgnore] public bool IsSurfaceModalitySame { get; set; }

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

    public class BirthMonthCompatibility
    {
        public BirthMonthCompatibility(NineStarKiModel nineStarKiModel1, NineStarKiModel nineStarKiModel2,
            CompatibilityScoreModel score)
        {
            Score = score;
            BornDaysApart = GetBornDaysApart(Math.Abs(nineStarKiModel1.PersonModel.DateOfBirth.DayOfYear -
                                                      nineStarKiModel2.PersonModel.DateOfBirth.DayOfYear));

            PolarisedPercent = (double)BornDaysApart / halfYearDays * 100;
            SharedWeaknessPercent = BornDaysApart <= quarterYearDays
                ? (1.0 - ((double)BornDaysApart / quarterYearDays)) * 100
                : 0;
            BalancePointPercent = Math.Max(0, 100 - PolarisedPercent - SharedWeaknessPercent);
            AwayFromBalancePointPercent = PolarisedPercent + SharedWeaknessPercent;

            CompatibilityDetails = GetCompatibilityReportHtml();

            CalculateScore();
        }

        private static int halfYearDays = 365 / 2;
        private static int quarterYearDays = 365 / 4;

        private double PolarisedPercent { get; }
        private double SharedWeaknessPercent { get; }
        private double BalancePointPercent { get; }
        private double AwayFromBalancePointPercent { get; }
        private int BornDaysApart { get; }

        public CompatibilityScoreModel Score { get; }

        public string CompatibilityDetails { get; }

        private int GetBornDaysApart(int value)
        {
            if (value > halfYearDays)
            {
                value = 365 - value; // shorter arc around the circle
            }

            return value;
        }

        private void CalculateScore()
        {
            Score.AddHarmonyScore(CompatibilityExtensions.GetEnumFromPercentage<ECompatibilityScore>(BalancePointPercent));
            Score.AddConflictScore(
                CompatibilityExtensions.GetEnumFromPercentage<ECompatibilityScore>(AwayFromBalancePointPercent));
            Score.AddMutualUnderstandingScore(
                CompatibilityExtensions.GetEnumFromPercentage<ECompatibilityScore>(PolarisedPercent));
            Score.AddComplementarityScore(
                CompatibilityExtensions.GetEnumFromPercentage<ECompatibilityScore>(PolarisedPercent));
            Score.AddSexualChemistryScore(CompatibilityExtensions.GetEnumFromPercentage<ESexualChemistryScore>(PolarisedPercent));
            Score.AddSparkScore(CompatibilityExtensions.GetEnumFromPercentage<ECompatibilityScore>(PolarisedPercent));
            Score.AddLearningPotentialScore(CompatibilityExtensions.GetEnumFromPercentage<ECompatibilityScore>(PolarisedPercent));
        }

        public string GetCompatibilityReportHtml()
        {
            return $@"
            <h5>{Dictionary.Polarity} {PolarisedPercent.ToString("0")}%</h5>
            <p>{GetPolarityParagraph(PolarisedPercent)}</p>

            <h5>{Dictionary.SharedVulnerability} {SharedWeaknessPercent.ToString("0")}%</h5>
            <p>{GetSamenessParagraph(SharedWeaknessPercent)}</p>

            <h5>{Dictionary.BalanceAndComplementarity} {BalancePointPercent.ToString("0")}%</h5>
            <p>{GetBalanceParagraph(BalancePointPercent)}</p>
            ".Trim();
        }

        private static string GetPolarityParagraph(double percent)
        {
            if (percent <= 20)
                return Dictionary.polarity_very_low;
            if (percent <= 40)
                return Dictionary.polarity_low;
            if (percent <= 60)
                return Dictionary.polarity_medium;
            if (percent <= 80)
                return Dictionary.polarity_high;
            return Dictionary.polarity_very_high;
        }

        private static string GetSamenessParagraph(double percent)
        {
            if (percent <= 20)
                return Dictionary.sameness_very_low;
            if (percent <= 40)
                return Dictionary.sameness_low;
            if (percent <= 60)
                return Dictionary.sameness_medium;
            if (percent <= 80)
                return Dictionary.sameness_high;
            return Dictionary.sameness_very_high;
        }

        private static string GetBalanceParagraph(double percent)
        {
            if (percent <= 20)
                return Dictionary.harmony_very_low;
            if (percent <= 40)
                return Dictionary.harmony_low;
            if (percent <= 60)
                return Dictionary.harmony_medium;
            if (percent <= 80)
                return Dictionary.harmony_high;
            return Dictionary.harmony_very_high;
        }
    }

    public class CompatibilityDetailsModel
    {
        public ElementCompatibility ElementCompatibility { get; }
        public GenderCompatibility GenderCompatibility { get; }
        public ModalityCompatibility ModalityCompatibility { get; }
        public BirthMonthCompatibility BirthMonthCompatibility { get; }
        public CompatibilityScoreModel Score { get; }

        public CompatibilityDetailsModel(CompatibilityModel compatibilityModel)
        {
            if (compatibilityModel.NineStarKiModel1.MainEnergy == null ||
                compatibilityModel.NineStarKiModel2.MainEnergy == null)
            {
                return;
            }

            Score = new CompatibilityScoreModel();
            ElementCompatibility = new ElementCompatibility(compatibilityModel.NineStarKiModel1,
                compatibilityModel.NineStarKiModel2, Score);
            GenderCompatibility = new GenderCompatibility(compatibilityModel.NineStarKiModel1,
                compatibilityModel.NineStarKiModel2, Score);
            ModalityCompatibility = new ModalityCompatibility(compatibilityModel.NineStarKiModel1,
                compatibilityModel.NineStarKiModel2, Score);
            BirthMonthCompatibility = new BirthMonthCompatibility(compatibilityModel.NineStarKiModel1,
                compatibilityModel.NineStarKiModel2, Score);

            Score.CalculateAverages();
        }
    }

    public static class CompatibilityExtensions
    {
        public static TEnum GetEnumFromPercentage<TEnum>(double percent) where TEnum : struct, Enum
        {
            // Clamp the input between 0 and 100
            percent = Math.Max(0, Math.Min(100, percent));

            // Get all enum values and sort them to ensure correct order
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();

            // Assume the first value is a placeholder (e.g., Unspecified), skip it
            int scoreLevels = values.Length - 1;

            // Calculate the index into the enum (excluding index 0)
            int index = (int)Math.Round((percent / 100.0) * scoreLevels);

            // Clamp between 1 and scoreLevels to avoid selecting the first (placeholder) value
            index = Math.Max(1, Math.Min(scoreLevels, index));

            return values[index];
        }
    }
}