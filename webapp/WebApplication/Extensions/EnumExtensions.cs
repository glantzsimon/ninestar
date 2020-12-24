using K9.Base.DataAccessLayer.Enums;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using System.Collections.Generic;

namespace K9.WebApplication.Extensions
{
    public static partial class Extensions
    {
        public static bool IsYin(this EGender gender)
        {
            return new List<EGender>
            {
                EGender.Female,
                EGender.Other,
            }.Contains(gender);
        }

        public static ETransformationType GetTransformationType(this ENineStarKiEnergy energy1, ENineStarKiEnergy energy2)
        {
            switch (energy1)
            {
                case ENineStarKiEnergy.Water:
                    switch (energy2)
                    {
                        case ENineStarKiEnergy.Water:
                            return ETransformationType.Same;

                        case ENineStarKiEnergy.Thunder:
                        case ENineStarKiEnergy.Wind:
                            return ETransformationType.Supports;

                        case ENineStarKiEnergy.Soil:
                        case ENineStarKiEnergy.CoreEarth:
                        case ENineStarKiEnergy.Mountain:
                            return ETransformationType.IsChallenged;

                        case ENineStarKiEnergy.Heaven:
                        case ENineStarKiEnergy.Lake:
                            return ETransformationType.IsSupported;

                        case ENineStarKiEnergy.Fire:
                            return ETransformationType.Challenges;
                    }

                    break;

                case ENineStarKiEnergy.Soil:
                case ENineStarKiEnergy.CoreEarth:
                case ENineStarKiEnergy.Mountain:
                    switch (energy2)
                    {
                        case ENineStarKiEnergy.Water:
                            return ETransformationType.Challenges;

                        case ENineStarKiEnergy.Thunder:
                        case ENineStarKiEnergy.Wind:
                            return ETransformationType.IsChallenged;

                        case ENineStarKiEnergy.Soil:
                        case ENineStarKiEnergy.CoreEarth:
                        case ENineStarKiEnergy.Mountain:
                            return ETransformationType.Same;

                        case ENineStarKiEnergy.Heaven:
                        case ENineStarKiEnergy.Lake:
                            return ETransformationType.Supports;

                        case ENineStarKiEnergy.Fire:
                            return ETransformationType.IsSupported;
                    }

                    break;

                case ENineStarKiEnergy.Thunder:
                case ENineStarKiEnergy.Wind:
                    switch (energy2)
                    {
                        case ENineStarKiEnergy.Water:
                            return ETransformationType.IsSupported;

                        case ENineStarKiEnergy.Thunder:
                        case ENineStarKiEnergy.Wind:
                            return ETransformationType.Same;

                        case ENineStarKiEnergy.Soil:
                        case ENineStarKiEnergy.CoreEarth:
                        case ENineStarKiEnergy.Mountain:
                            return ETransformationType.Challenges;

                        case ENineStarKiEnergy.Heaven:
                        case ENineStarKiEnergy.Lake:
                            return ETransformationType.IsChallenged;

                        case ENineStarKiEnergy.Fire:
                            return ETransformationType.Supports;
                    }

                    break;

                case ENineStarKiEnergy.Heaven:
                case ENineStarKiEnergy.Lake:
                    switch (energy2)
                    {
                        case ENineStarKiEnergy.Water:
                            return ETransformationType.Supports;

                        case ENineStarKiEnergy.Thunder:
                        case ENineStarKiEnergy.Wind:
                            return ETransformationType.Challenges;

                        case ENineStarKiEnergy.Soil:
                        case ENineStarKiEnergy.CoreEarth:
                        case ENineStarKiEnergy.Mountain:
                            return ETransformationType.IsSupported;

                        case ENineStarKiEnergy.Heaven:
                        case ENineStarKiEnergy.Lake:
                            return ETransformationType.Same;

                        case ENineStarKiEnergy.Fire:
                            return ETransformationType.IsChallenged;
                    }

                    break;

                case ENineStarKiEnergy.Fire:
                    switch (energy2)
                    {
                        case ENineStarKiEnergy.Water:
                            return ETransformationType.IsChallenged;

                        case ENineStarKiEnergy.Thunder:
                        case ENineStarKiEnergy.Wind:
                            return ETransformationType.IsSupported;

                        case ENineStarKiEnergy.Soil:
                        case ENineStarKiEnergy.CoreEarth:
                        case ENineStarKiEnergy.Mountain:
                            return ETransformationType.Supports;

                        case ENineStarKiEnergy.Heaven:
                        case ENineStarKiEnergy.Lake:
                            return ETransformationType.Challenges;

                        case ENineStarKiEnergy.Fire:
                            return ETransformationType.Same;
                    }

                    break;
            }

            return ETransformationType.Unspecified;
        }
    }
}
