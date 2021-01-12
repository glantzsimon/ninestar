using K9.WebApplication.Enums;
using K9.WebApplication.Extensions;
using System.Collections.Generic;

namespace K9.WebApplication.Models
{

    public class CompatibilityDetailsModel
    {
        
        public CompatibilityDetailsModel(NineStarKiModel energy1, NineStarKiModel energy2)
        {
            if (energy1.MainEnergy == null || energy2.MainEnergy == null)
            {
                return;
            }

            var mainTransformationType = energy1.MainEnergy.Energy.GetTransformationType(energy2.MainEnergy.Energy);
            IsFundamtenalGenderSame = energy1.MainEnergy.YinYang == energy2.MainEnergy.YinYang;
            IsFundamentalModalitySame = energy1.MainEnergy.Modality == energy2.MainEnergy.Modality;
            IsFundamentalElementSame = energy1.MainEnergy.Element == energy2.MainEnergy.Element;
            IsFundamentalElementChallenging = new List<ETransformationType>
            {
                ETransformationType.Challenges,
                ETransformationType.IsChallenged
            }.Contains(mainTransformationType);
            IsFundamentalElementSupportive = !IsFundamentalElementSame && !IsFundamentalElementChallenging;

            var characterTransformationType = energy1.CharacterEnergy.Energy.GetTransformationType(energy2.CharacterEnergy.Energy);
            IsCharacterGenderSame = energy1.CharacterEnergy.YinYang == energy2.CharacterEnergy.YinYang;
            IsCharacterModalitySame = energy1.CharacterEnergy.Modality == energy2.CharacterEnergy.Modality;
            IsCharacterElementSame = energy1.CharacterEnergy.Element == energy2.CharacterEnergy.Element;
            IsCharacterElementChallenging = new List<ETransformationType>
            {
                ETransformationType.Challenges,
                ETransformationType.IsChallenged
            }.Contains(characterTransformationType);
            IsCharacterElementSupportive = !IsCharacterElementSame && !IsCharacterElementChallenging;
        }

        public bool IsFundamtenalGenderSame { get; }
        public bool IsFundamentalModalitySame { get; }
        public bool IsFundamentalElementSame { get; }
        public bool IsFundamentalElementSupportive { get; }
        public bool IsFundamentalElementChallenging { get; }
        
        public bool IsCharacterGenderSame { get; }
        public bool IsCharacterModalitySame { get; }
        public bool IsCharacterElementSame { get; }
        public bool IsCharacterElementSupportive { get; }
        public bool IsCharacterElementChallenging { get; }

            //public bool IsFundamentalToCharacterElementSame { get; }
            //public bool IsFundamentalToCharacterElementSupportive { get; }
            //public bool IsFundamentalToCharacterElementChallenging { get; }
            //public bool IsCharacterToElementSame { get; }
            //public bool IsFundamentalToCharacterElementSupportive { get; }
            //public bool IsFundamentalToCharacterElementChallenging { get; }

       
    }
}