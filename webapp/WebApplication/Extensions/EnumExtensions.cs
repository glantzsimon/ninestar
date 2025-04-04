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

        public static ETransformationType GetTransformationTypeWithYingYang(this ENineStarKiEnergy energy1,
            ENineStarKiEnergy energy2)
        {
            if (energy1 == energy2)
            {
                return ETransformationType.Same;
            }
            return GetTransformationType(energy1, energy2);
        }

        public static ETransformationType GetTransformationType(this ENineStarKiEnergy energy1, ENineStarKiEnergy energy2)
        {
            switch (energy1)
            {
                case ENineStarKiEnergy.Water:
                    switch (energy2)
                    {
                        case ENineStarKiEnergy.Water:
                            return ETransformationType.Sibling;

                        case ENineStarKiEnergy.Thunder:
                        case ENineStarKiEnergy.Wind:
                            return ETransformationType.Supports;

                        case ENineStarKiEnergy.Soil:
                        case ENineStarKiEnergy.CoreEarth:
                        case ENineStarKiEnergy.Mountain:
                            return ETransformationType.IsControlled;

                        case ENineStarKiEnergy.Heaven:
                        case ENineStarKiEnergy.Lake:
                            return ETransformationType.IsSupported;

                        case ENineStarKiEnergy.Fire:
                            return ETransformationType.Controls;
                    }

                    break;

                case ENineStarKiEnergy.Soil:
                case ENineStarKiEnergy.CoreEarth:
                case ENineStarKiEnergy.Mountain:
                    switch (energy2)
                    {
                        case ENineStarKiEnergy.Water:
                            return ETransformationType.Controls;

                        case ENineStarKiEnergy.Thunder:
                        case ENineStarKiEnergy.Wind:
                            return ETransformationType.IsControlled;

                        case ENineStarKiEnergy.Soil:
                        case ENineStarKiEnergy.CoreEarth:
                        case ENineStarKiEnergy.Mountain:
                            return ETransformationType.Sibling;

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
                            return ETransformationType.Sibling;

                        case ENineStarKiEnergy.Soil:
                        case ENineStarKiEnergy.CoreEarth:
                        case ENineStarKiEnergy.Mountain:
                            return ETransformationType.Controls;

                        case ENineStarKiEnergy.Heaven:
                        case ENineStarKiEnergy.Lake:
                            return ETransformationType.IsControlled;

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
                            return ETransformationType.Controls;

                        case ENineStarKiEnergy.Soil:
                        case ENineStarKiEnergy.CoreEarth:
                        case ENineStarKiEnergy.Mountain:
                            return ETransformationType.IsSupported;

                        case ENineStarKiEnergy.Heaven:
                        case ENineStarKiEnergy.Lake:
                            return ETransformationType.Sibling;

                        case ENineStarKiEnergy.Fire:
                            return ETransformationType.IsControlled;
                    }

                    break;

                case ENineStarKiEnergy.Fire:
                    switch (energy2)
                    {
                        case ENineStarKiEnergy.Water:
                            return ETransformationType.IsControlled;

                        case ENineStarKiEnergy.Thunder:
                        case ENineStarKiEnergy.Wind:
                            return ETransformationType.IsSupported;

                        case ENineStarKiEnergy.Soil:
                        case ENineStarKiEnergy.CoreEarth:
                        case ENineStarKiEnergy.Mountain:
                            return ETransformationType.Supports;

                        case ENineStarKiEnergy.Heaven:
                        case ENineStarKiEnergy.Lake:
                            return ETransformationType.Controls;

                        case ENineStarKiEnergy.Fire:
                            return ETransformationType.Sibling;
                    }

                    break;
            }

            return ETransformationType.Unspecified;
        }
    }
}
