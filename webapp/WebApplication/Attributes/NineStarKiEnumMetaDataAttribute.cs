using K9.Base.DataAccessLayer.Attributes;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K9.WebApplication.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class NineStarKiEnumMetaDataAttribute : Attribute
    {
        public ENineStarKiFamilyMember FamilyMember { get; set; }
        public ENineStarKiElement Element { get; set; }
        public ENineStarKiColour Colour { get; set; }
        public ENineStarKiDirection Direction { get; set; }
        public ENineStarKiYinYang YinYang { get; set; }
        public ENineStarKiYinYangExpansion YinYangExpansion { get; set; }
        public ENineStarKiDescriptiveName DescriptiveName { get; set; }
        public ENineStarKiModality Modality { get; set; }
        public ENineStarKiCycle Cycle { get; set; }

        public EBodyPart[] InternalRepresentation { get; set; }
        public EBodyPart[] Physiognomy { get; set; }
        public EPrimaryAttribute[] PrimaryAttributes { get; set; }
        public int[] MostHarmoniousCombinations { get; set; }
        public int[] MostChallengingCombinations { get; set; }

        public EOrgan StrongYinOrgans => GetStrongYinOrgans();
        public EOrgan StrongYangOrgans => GetStrongYangOrgans();
        public EOrgan[] WeakYinOrgans => GetWeakYinOrgans();
        public EOrgan[] WeakYangOrgans => GetWeakYangOrgans();

        public Type ResourceType { get; set; }
        public string TrigramName { get; set; }
        public string Name { get; set; }

        public string ModalityDescription
        {
            get { return GetModalityDescription(); }
        }

        public string GetDescription()
        {
            return ResourceType.GetValueFromResource(Name);
        }

        public string GetDescriptiveTitle()
        {
            return GetEnumDescription(DescriptiveName);
        }

        public string GetYinYang()
        {
            return GetEnumDescription(YinYang);
        }

        public string GetFamilyMember()
        {
            return GetEnumDescription(FamilyMember);
        }

        public string GetInternalRepresentation()
        {
            return string.Join(", ", GetResourceNames(InternalRepresentation));
        }

        public string GetPhysiognomy()
        {
            return string.Join(", ", GetResourceNames(Physiognomy));
        }

        public string GetPrimaryAttributes()
        {
            return string.Join(", ", GetResourceNames(PrimaryAttributes));
        }

        public List<ENineStarKiEnergy> GetMostHarmoniousCombinations()
        {
            return MostHarmoniousCombinations.Select(e => (ENineStarKiEnergy)e).ToList();
        }

        public string GetMostHarmoniousCombinationsString()
        {
            return string.Join(", ", GetMostHarmoniousCombinations().Select(e => GetNumberAndElementName(e)));
        }

        public List<ENineStarKiEnergy> GetMostChallengingCombinations()
        {
            return MostChallengingCombinations.Select(e => (ENineStarKiEnergy)e).ToList();
        }

        public string GetMostChallengingCombinationsString()
        {
            return string.Join(", ", GetMostChallengingCombinations().Select(e => GetNumberAndElementName(e)));
        }

        public string GetElement()
        {
            return GetEnumDescription(Element);
        }

        public string GetColour()
        {
            return GetEnumDescription(Colour);
        }

        public string GetDirection()
        {
            return GetEnumDescription(Direction);
        }

        public string GetTrigram()
        {
            return ResourceType.GetValueFromResource(TrigramName);
        }

        private EOrgan GetStrongYinOrgans()
        {
            return Element.GetAttribute<ENineStarKiElementEnumMetaDataAttribute>().StrongYinOrgans;
        }

        private EOrgan GetStrongYangOrgans()
        {
            return Element.GetAttribute<ENineStarKiElementEnumMetaDataAttribute>().StrongYangOrgans;
        }

        private EOrgan[] GetWeakYinOrgans()
        {
            return Element.GetAttribute<ENineStarKiElementEnumMetaDataAttribute>().WeakYinOrgans;
        }

        private EOrgan[] GetWeakYangOrgans()
        {
            return Element.GetAttribute<ENineStarKiElementEnumMetaDataAttribute>().WeakYangOrgans;
        }

        /// <summary>
        /// Generic method to get the description of any enum with EnumDescriptionAttribute.
        /// </summary>
        private static string GetEnumDescription<TEnum>(TEnum value) where TEnum : Enum
        {
            var attr = value.GetAttribute<EnumDescriptionAttribute>();
            return attr != null ? attr.GetDescription() : value.ToString();
        }

        private string[] GetResourceNames<TEnum>(TEnum[] values) where TEnum : Enum
        {
            return values
                .Select(value =>
                {
                    var key = value.GetAttribute<EnumDescriptionAttribute>().Name;
                    return ResourceType.GetValueFromResource(key);
                })
                .ToArray();
        }

        /// <summary>
        /// Dictionary for fast lookup of element descriptions.
        /// </summary>
        private static readonly Dictionary<ENineStarKiElement, string> _elementDescriptions = new Dictionary<ENineStarKiElement, string>
        {
            { ENineStarKiElement.Earth, Dictionary.earth_element },
            { ENineStarKiElement.Fire, Dictionary.fire_element },
            { ENineStarKiElement.Metal, Dictionary.metal_element },
            { ENineStarKiElement.Tree, Dictionary.tree_element },
            { ENineStarKiElement.Water, Dictionary.water_element }
        };

        public string GetElementDescription()
        {
            return _elementDescriptions.TryGetValue(Element, out var desc) ? desc : string.Empty;
        }

        private static readonly Dictionary<ENineStarKiYinYangExpansion, string> _yinYangExpansionGroupDescriptions = new Dictionary<ENineStarKiYinYangExpansion, string>
        {
            { ENineStarKiYinYangExpansion.YinExpanding, Dictionary.yin_expanding_climate },
            { ENineStarKiYinYangExpansion.YangContracting, Dictionary.yang_expanding_climate },
            { ENineStarKiYinYangExpansion.Balanced, Dictionary.balanced_climate }
        };

        private static readonly Dictionary<ENineStarKiYinYangExpansion, string> _yinYangExpansionGroupTitles = new Dictionary<ENineStarKiYinYangExpansion, string>
        {
            { ENineStarKiYinYangExpansion.YinExpanding, Dictionary.YinExpandingGroup },
            { ENineStarKiYinYangExpansion.YangContracting, Dictionary.YangExpandingGroup},
            { ENineStarKiYinYangExpansion.Balanced, Dictionary.YinYangBalancedGroup }
        };

        public string GetYinYangExpansionDescription()
        {
            return _yinYangExpansionGroupDescriptions.TryGetValue(YinYangExpansion, out var desc) ? desc : string.Empty;
        }

        public string GetYinYangExpansionTitle()
        {
            return _yinYangExpansionGroupTitles.TryGetValue(YinYangExpansion, out var desc) ? desc : string.Empty;
        }

        /// <summary>
        /// Returns the energy number and name in a formatted string.
        /// </summary>
        private string GetEnergyNumberAndName(ENineStarKiEnergy energy)
        {
            var attr = energy.GetAttribute<NineStarKiEnumMetaDataAttribute>();
            return string.Format("{0} {1}", (int)energy, attr != null ? attr.Name : energy.ToString());
        }

        /// <summary>
        /// Returns the energy number and name in a formatted string.
        /// </summary>
        private string GetNumberAndElementName(ENineStarKiEnergy energy)
        {
            var attr = energy.GetAttribute<NineStarKiEnumMetaDataAttribute>();
            var energyName = attr != null ? attr.GetElement() : energy.ToString();
            return $"{(int)energy} {energyName}";
        }

        /// <summary>
        /// Dictionary for fast lookup of modality descriptions.
        /// </summary>
        private static readonly Dictionary<ENineStarKiModality, string> _modalityDescriptions = new Dictionary<ENineStarKiModality, string>
        {
            { ENineStarKiModality.Dynamic, Dictionary.dynamic_modality },
            { ENineStarKiModality.Stable, Dictionary.stable_modality },
            { ENineStarKiModality.Reflective, Dictionary.reflective_modality }
        };

        /// <summary>
        /// Returns the description of the modality with formatted energy names.
        /// </summary>
        private string GetModalityDescription()
        {
            if (!_modalityDescriptions.TryGetValue(Modality, out var modalityText))
            {
                modalityText = string.Empty;
            }

            return TemplateParser.Parse(
                modalityText,
                new
                {
                    water = GetEnergyNumberAndName(ENineStarKiEnergy.Water),
                    soil = GetEnergyNumberAndName(ENineStarKiEnergy.Soil),
                    thunder = GetEnergyNumberAndName(ENineStarKiEnergy.Thunder),
                    wind = GetEnergyNumberAndName(ENineStarKiEnergy.Wind),
                    coreearth = GetEnergyNumberAndName(ENineStarKiEnergy.CoreEarth),
                    heaven = GetEnergyNumberAndName(ENineStarKiEnergy.Heaven),
                    lake = GetEnergyNumberAndName(ENineStarKiEnergy.Lake),
                    mountain = GetEnergyNumberAndName(ENineStarKiEnergy.Mountain),
                    fire = GetEnergyNumberAndName(ENineStarKiEnergy.Fire)
                }
            );
        }
    }
}
