using K9.Base.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.ViewModels;
using System;
using System.Collections.Generic;

namespace K9.WebApplication.Services
{
    public class NineStarKiService : INineStarKiService
    {
        private readonly IMembershipService _membershipService;
        private readonly IAuthentication _authentication;
        private readonly IRoles _roles;
        private readonly IRepository<UserProfileReading> _userProfileReadingsRepository;

        private readonly IRepository<UserRelationshipCompatibilityReading>
            _userRelationshipCompatibilityReadingRepository;

        public NineStarKiService(IMembershipService membershipService, IAuthentication authentication, IRoles roles,
            IRepository<UserProfileReading> userProfileReadingsRepository,
            IRepository<UserRelationshipCompatibilityReading> userRelationshipCompatibilityReadingRepository)
        {
            _membershipService = membershipService;
            _authentication = authentication;
            _roles = roles;
            _userProfileReadingsRepository = userProfileReadingsRepository;
            _userRelationshipCompatibilityReadingRepository = userRelationshipCompatibilityReadingRepository;
        }

        public NineStarKiModel CalculateNineStarKiProfile(DateTime dateOfBirth, EGender gender = EGender.Male)
        {
            return CalculateNineStarKiProfile(new PersonModel
            {
                DateOfBirth = dateOfBirth,
                Gender = gender
            });
        }

        public NineStarKiModel CalculateNineStarKiProfile(PersonModel personModel, bool isCompatibility = false,
            bool isMyProfile = false, DateTime? today = null)
        {
            var model = new NineStarKiModel(personModel, today);

            model.MainEnergy.EnergyDescription = GetMainEnergyDescription(model.MainEnergy.Energy);
            model.CharacterEnergy.EnergyDescription = GetCharacterEnergyDescription(model.CharacterEnergy.Energy);
            model.SurfaceEnergy.EnergyDescription = GetSurfaceEnergyDescription(model.SurfaceEnergy.Energy);
            model.Health = GetHealth(model.MainEnergy.Energy);
            model.Occupations = GetOccupations(model.MainEnergy.Energy);
            model.PersonalDevelopemnt = GetPersonalDevelopemnt(model.MainEnergy.Energy);
            model.Summary = GetSummary(model);
            model.Overview = GetOverview(model.MainEnergy.Energy);

            if (_authentication.IsAuthenticated)
            {
                if (isCompatibility || _roles.CurrentUserIsInRoles(RoleNames.Administrators) ||
                    _membershipService.IsCompleteProfileReading(Current.UserId, personModel))
                {
                    model.ReadingType = EReadingType.Complete;
                    model.IsUpgradeRequired = false;
                }
            }

            if (isCompatibility)
            {
                model.IsShowSummary = false;
            }

            model.IsMyProfile = isMyProfile;
            model.IsProcessed = true;
            model.IsCompatibility = isCompatibility;

            return model;
        }

        public NineStarKiModel RetrieveNineStarKiProfile(int userProfileId)
        {
            if (!_authentication.IsAuthenticated)
            {
                throw new UnauthorizedAccessException();
            }

            var userProfile = _userProfileReadingsRepository.Find(userProfileId);
            if (!_roles.CurrentUserIsInRoles(RoleNames.Administrators))
            {
                if (userProfile.UserId != Current.UserId)
                {
                    throw new UnauthorizedAccessException();
                }
            }

            return CalculateNineStarKiProfile(new PersonModel
            {
                Name = userProfile.FullName,
                DateOfBirth = userProfile.DateOfBirth,
                Gender = userProfile.Gender
            });
        }

        public CompatibilityModel CalculateCompatibility(DateTime dateOfBirth1, EGender gender1, DateTime dateOfBirth2,
            EGender gender2)
        {
            return CalculateCompatibility(
                new PersonModel
                {
                    DateOfBirth = dateOfBirth1,
                    Gender = gender1
                }, new PersonModel
                {
                    DateOfBirth = dateOfBirth2,
                    Gender = gender2
                }, false);
        }

        public CompatibilityModel CalculateCompatibility(PersonModel personModel1, PersonModel personModel2,
            bool isHideSexuality)
        {
            var nineStarKiModel1 = CalculateNineStarKiProfile(personModel1, true);
            var nineStarKiModel2 = CalculateNineStarKiProfile(personModel2, true);

            var model = new CompatibilityModel(nineStarKiModel1, nineStarKiModel2)
            {
                IsProcessed = true,
                IsUpgradeRequired = true,
                IsHideSexualChemistry = isHideSexuality
            };

            if (_authentication.IsAuthenticated)
            {
                if (_roles.CurrentUserIsInRoles(RoleNames.Administrators) ||
                    _membershipService.IsCompleteRelationshipCompatibilityReading(Current.UserId,
                        personModel1, personModel2, isHideSexuality))
                {
                    model.IsUpgradeRequired = false;
                }
            }

            if (string.IsNullOrEmpty(nineStarKiModel1.PersonModel.Name))
            {
                nineStarKiModel1.PersonModel.Name = Dictionary.FirstPerson;
            }

            if (string.IsNullOrEmpty(nineStarKiModel2.PersonModel.Name))
            {
                nineStarKiModel2.PersonModel.Name = Dictionary.SecondPerson;
            }

            return model;
        }

        public CompatibilityModel RetrieveCompatibility(int userRelationshipCompatibilityId)
        {
            var reading = _userRelationshipCompatibilityReadingRepository.Find(userRelationshipCompatibilityId);
            return CalculateCompatibility(new PersonModel
                {
                    Name = reading.FirstName,
                    DateOfBirth = reading.FirstDateOfBirth,
                    Gender = reading.FirstGender
                },
                new PersonModel
                {
                    Name = reading.SecondName,
                    DateOfBirth = reading.SecondDateOfBirth,
                    Gender = reading.SecondGender
                },
                reading.IsHideSexuality);
        }

        public NineStarKiSummaryViewModel GetNineStarKiSummaryViewModel()
        {
            var mainEnergies = new List<NineStarKiModel>
            {
                CalculateNineStarKiProfile(new DateTime(1981, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1980, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1979, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1978, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1977, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1976, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1984, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1974, 3, 3)),
                CalculateNineStarKiProfile(new DateTime(1973, 3, 3)),
            };
            var characterEnergies = new List<NineStarKiModel>
            {
                CalculateNineStarKiProfile(new DateTime(1980, 2, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 3, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 4, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 5, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 6, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 7, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 8, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 9, 10)),
                CalculateNineStarKiProfile(new DateTime(1980, 10, 10)),
            };
            var dynamicEnergies = new List<NineStarKiEnergy>
            {
                new NineStarKiEnergy(ENineStarKiEnergy.Thunder, ENineStarKiEnergyType.MainEnergy),
                new NineStarKiEnergy(ENineStarKiEnergy.Heaven, ENineStarKiEnergyType.MainEnergy),
                new NineStarKiEnergy(ENineStarKiEnergy.Fire, ENineStarKiEnergyType.MainEnergy)
            };
            var staticEnergies = new List<NineStarKiEnergy>
            {
                new NineStarKiEnergy(ENineStarKiEnergy.Soil, ENineStarKiEnergyType.MainEnergy),
                new NineStarKiEnergy(ENineStarKiEnergy.CoreEarth, ENineStarKiEnergyType.MainEnergy),
                new NineStarKiEnergy(ENineStarKiEnergy.Mountain, ENineStarKiEnergyType.MainEnergy)
            };
            var flexibleEnergies = new List<NineStarKiEnergy>
            {
                new NineStarKiEnergy(ENineStarKiEnergy.Water, ENineStarKiEnergyType.MainEnergy),
                new NineStarKiEnergy(ENineStarKiEnergy.Wind, ENineStarKiEnergyType.MainEnergy),
                new NineStarKiEnergy(ENineStarKiEnergy.Lake, ENineStarKiEnergyType.MainEnergy)
            };

            return new NineStarKiSummaryViewModel(mainEnergies, characterEnergies, dynamicEnergies, staticEnergies,
                flexibleEnergies);
        }

        private string GetOverview(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_overview;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_overview;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_overview;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_overview;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_overview;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_overview;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_overview;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_overview;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_overview;
            }

            return string.Empty;
        }

        private string GetSummary(NineStarKiModel model)
        {
            if (!model.PersonModel.IsAdult())
            {
                switch (model.CharacterEnergy.Energy)
                {
                    case ENineStarKiEnergy.Water:
                        return Dictionary.water_child;

                    case ENineStarKiEnergy.Soil:
                        return Dictionary.soil_child;

                    case ENineStarKiEnergy.Thunder:
                        return Dictionary.thunder_child;

                    case ENineStarKiEnergy.Wind:
                        return Dictionary.wind_child;

                    case ENineStarKiEnergy.CoreEarth:
                        return Dictionary.coreearth_child;

                    case ENineStarKiEnergy.Heaven:
                        return Dictionary.heaven_child;

                    case ENineStarKiEnergy.Lake:
                        return Dictionary.lake_child;

                    case ENineStarKiEnergy.Mountain:
                        return Dictionary.mountain_child;

                    case ENineStarKiEnergy.Fire:
                        return Dictionary.fire_child;
                }
            }
            else
            {
                switch (model.MainEnergy.Energy)
                {
                    case ENineStarKiEnergy.Water:
                        switch (model.CharacterEnergy.Energy)
                        {
                            case ENineStarKiEnergy.Water:
                                return Dictionary._115;

                            case ENineStarKiEnergy.Soil:
                                return Dictionary._124;

                            case ENineStarKiEnergy.Thunder:
                                return Dictionary._133;

                            case ENineStarKiEnergy.Wind:
                                return Dictionary._142;

                            case ENineStarKiEnergy.CoreEarth:
                                return Dictionary._151;

                            case ENineStarKiEnergy.Heaven:
                                return Dictionary._169;

                            case ENineStarKiEnergy.Lake:
                                return Dictionary._178;

                            case ENineStarKiEnergy.Mountain:
                                return Dictionary._187;

                            case ENineStarKiEnergy.Fire:
                                return Dictionary._196;
                        }

                        break;

                    case ENineStarKiEnergy.Soil:
                        switch (model.CharacterEnergy.Energy)
                        {
                            case ENineStarKiEnergy.Water:
                                return Dictionary._216;

                            case ENineStarKiEnergy.Soil:
                                return Dictionary._225;

                            case ENineStarKiEnergy.Thunder:
                                return Dictionary._234;

                            case ENineStarKiEnergy.Wind:
                                return Dictionary._243;

                            case ENineStarKiEnergy.CoreEarth:
                                return Dictionary._252;

                            case ENineStarKiEnergy.Heaven:
                                return Dictionary._261;

                            case ENineStarKiEnergy.Lake:
                                return Dictionary._279;

                            case ENineStarKiEnergy.Mountain:
                                return Dictionary._288;

                            case ENineStarKiEnergy.Fire:
                                return Dictionary._297;
                        }

                        break;

                    case ENineStarKiEnergy.Thunder:
                        switch (model.CharacterEnergy.Energy)
                        {
                            case ENineStarKiEnergy.Water:
                                return Dictionary._317;

                            case ENineStarKiEnergy.Soil:
                                return Dictionary._326;

                            case ENineStarKiEnergy.Thunder:
                                return Dictionary._335;

                            case ENineStarKiEnergy.Wind:
                                return Dictionary._344;

                            case ENineStarKiEnergy.CoreEarth:
                                return Dictionary._353;

                            case ENineStarKiEnergy.Heaven:
                                return Dictionary._362;

                            case ENineStarKiEnergy.Lake:
                                return Dictionary._371;

                            case ENineStarKiEnergy.Mountain:
                                return Dictionary._389;

                            case ENineStarKiEnergy.Fire:
                                return Dictionary._398;
                        }

                        break;

                    case ENineStarKiEnergy.Wind:
                        switch (model.CharacterEnergy.Energy)
                        {
                            case ENineStarKiEnergy.Water:
                                return Dictionary._418;

                            case ENineStarKiEnergy.Soil:
                                return Dictionary._427;

                            case ENineStarKiEnergy.Thunder:
                                return Dictionary._436;

                            case ENineStarKiEnergy.Wind:
                                return Dictionary._445;

                            case ENineStarKiEnergy.CoreEarth:
                                return Dictionary._454;

                            case ENineStarKiEnergy.Heaven:
                                return Dictionary._463;

                            case ENineStarKiEnergy.Lake:
                                return Dictionary._472;

                            case ENineStarKiEnergy.Mountain:
                                return Dictionary._481;

                            case ENineStarKiEnergy.Fire:
                                return Dictionary._499;
                        }

                        return string.Empty;

                    case ENineStarKiEnergy.CoreEarth:
                        switch (model.CharacterEnergy.Energy)
                        {
                            case ENineStarKiEnergy.Water:
                                return Dictionary._519;

                            case ENineStarKiEnergy.Soil:
                                return Dictionary._528;

                            case ENineStarKiEnergy.Thunder:
                                return Dictionary._537;

                            case ENineStarKiEnergy.Wind:
                                return Dictionary._546;

                            case ENineStarKiEnergy.CoreEarth:
                                return Dictionary._555;

                            case ENineStarKiEnergy.Heaven:
                                return Dictionary._564;

                            case ENineStarKiEnergy.Lake:
                                return Dictionary._573;

                            case ENineStarKiEnergy.Mountain:
                                return Dictionary._582;

                            case ENineStarKiEnergy.Fire:
                                return Dictionary._591;
                        }

                        return string.Empty;

                    case ENineStarKiEnergy.Heaven:
                        switch (model.CharacterEnergy.Energy)
                        {
                            case ENineStarKiEnergy.Water:
                                return Dictionary._611;

                            case ENineStarKiEnergy.Soil:
                                return Dictionary._629;

                            case ENineStarKiEnergy.Thunder:
                                return Dictionary._638;

                            case ENineStarKiEnergy.Wind:
                                return Dictionary._647;

                            case ENineStarKiEnergy.CoreEarth:
                                return Dictionary._656;

                            case ENineStarKiEnergy.Heaven:
                                return Dictionary._665;

                            case ENineStarKiEnergy.Lake:
                                return Dictionary._674;

                            case ENineStarKiEnergy.Mountain:
                                return Dictionary._683;

                            case ENineStarKiEnergy.Fire:
                                return Dictionary._692;
                        }

                        return string.Empty;

                    case ENineStarKiEnergy.Lake:
                        switch (model.CharacterEnergy.Energy)
                        {
                            case ENineStarKiEnergy.Water:
                                return Dictionary._712;

                            case ENineStarKiEnergy.Soil:
                                return Dictionary._721;

                            case ENineStarKiEnergy.Thunder:
                                return Dictionary._739;

                            case ENineStarKiEnergy.Wind:
                                return Dictionary._748;

                            case ENineStarKiEnergy.CoreEarth:
                                return Dictionary._757;

                            case ENineStarKiEnergy.Heaven:
                                return Dictionary._766;

                            case ENineStarKiEnergy.Lake:
                                return Dictionary._775;

                            case ENineStarKiEnergy.Mountain:
                                return Dictionary._784;

                            case ENineStarKiEnergy.Fire:
                                return Dictionary._793;
                        }

                        return string.Empty;

                    case ENineStarKiEnergy.Mountain:
                        switch (model.CharacterEnergy.Energy)
                        {
                            case ENineStarKiEnergy.Water:
                                return Dictionary._813;

                            case ENineStarKiEnergy.Soil:
                                return Dictionary._822;

                            case ENineStarKiEnergy.Thunder:
                                return Dictionary._831;

                            case ENineStarKiEnergy.Wind:
                                return Dictionary._849;

                            case ENineStarKiEnergy.CoreEarth:
                                return Dictionary._858;

                            case ENineStarKiEnergy.Heaven:
                                return Dictionary._867;

                            case ENineStarKiEnergy.Lake:
                                return Dictionary._876;

                            case ENineStarKiEnergy.Mountain:
                                return Dictionary._885;

                            case ENineStarKiEnergy.Fire:
                                return Dictionary._894;
                        }

                        return string.Empty;

                    case ENineStarKiEnergy.Fire:
                        switch (model.CharacterEnergy.Energy)
                        {
                            case ENineStarKiEnergy.Water:
                                return Dictionary._914;

                            case ENineStarKiEnergy.Soil:
                                return Dictionary._923;

                            case ENineStarKiEnergy.Thunder:
                                return Dictionary._932;

                            case ENineStarKiEnergy.Wind:
                                return Dictionary._941;

                            case ENineStarKiEnergy.CoreEarth:
                                return Dictionary._959;

                            case ENineStarKiEnergy.Heaven:
                                return Dictionary._968;

                            case ENineStarKiEnergy.Lake:
                                return Dictionary._977;

                            case ENineStarKiEnergy.Mountain:
                                return Dictionary._986;

                            case ENineStarKiEnergy.Fire:
                                return Dictionary._995;
                        }

                        return string.Empty;
                }
            }

            return string.Empty;
        }

        private string GetMainEnergyDescription(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_description;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_description;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_description;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_description;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_description;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_description;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_description;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_description;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_description;
            }

            return string.Empty;
        }

        private string GetCharacterEnergyDescription(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_emotional_description;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_emotional_description;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_emotional_description;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_emotional_description;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_emotional_description;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_emotional_description;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_emotional_description;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_emotional_description;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_emotional_description;
            }

            return string.Empty;
        }

        private string GetSurfaceEnergyDescription(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_surface_description;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_surface_description;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_surface_description;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_surface_description;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_surface_description;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_surface_description;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_surface_description;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_surface_description;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_surface_description;
            }

            return string.Empty;
        }

        private string GetHealth(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_health;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_health;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_health;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_health;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_health;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_health;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_health;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_health;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_health;
            }

            return string.Empty;
        }

        private string GetOccupations(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_occupations;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_occupations;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_occupations;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_occupations;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_occupations;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_occupations;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_occupations;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_occupations;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_occupations;
            }

            return string.Empty;
        }

        private string GetPersonalDevelopemnt(ENineStarKiEnergy energy)
        {
            switch (energy)
            {
                case ENineStarKiEnergy.Water:
                    return Dictionary.water_personal_development;

                case ENineStarKiEnergy.Soil:
                    return Dictionary.soil_personal_development;

                case ENineStarKiEnergy.Thunder:
                    return Dictionary.thunder_personal_development;

                case ENineStarKiEnergy.Wind:
                    return Dictionary.wind_personal_development;

                case ENineStarKiEnergy.CoreEarth:
                    return Dictionary.coreearth_personal_development;

                case ENineStarKiEnergy.Heaven:
                    return Dictionary.heaven_personal_development;

                case ENineStarKiEnergy.Lake:
                    return Dictionary.lake_personal_development;

                case ENineStarKiEnergy.Mountain:
                    return Dictionary.mountain_personal_development;

                case ENineStarKiEnergy.Fire:
                    return Dictionary.fire_personal_development;
            }

            return string.Empty;
        }
    }
}