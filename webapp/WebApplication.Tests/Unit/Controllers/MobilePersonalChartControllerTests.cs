using K9.Base.DataAccessLayer.Attributes;
using K9.SharedLibrary.Extensions;
using K9.Base.DataAccessLayer.Enums;
using K9.Base.DataAccessLayer.Models;
using K9.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Constants;
using K9.WebApplication.Controllers;
using K9.WebApplication.Enums;
using K9.WebApplication.Helpers;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels.MobileApi;
using Moq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Controllers
{
    public class MobilePersonalChartControllerTests
    {
        [Theory]
        [InlineData(false, 42)]
        [InlineData(true, 0)]
        public void AnonymousOrUnresolvedIdentityCannotLoadPrivateData(bool authenticated, int userId)
        {
            var fixture = new Fixture(authenticated, userId);
            var json = fixture.Invoke();
            fixture.AssertError(json, 401, "authentication_required");
            fixture.Users.Verify(e => e.Find(It.IsAny<int>()), Times.Never);
            fixture.Memberships.Verify(e => e.GetActiveUserMembership(It.IsAny<int>()), Times.Never);
            fixture.Repository.Verify(e => e.Find(It.IsAny<Expression<Func<UserInfo, bool>>>()), Times.Never);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void FreeOrExpiredMembershipCannotReceiveFullChart(bool expired)
        {
            var fixture = new Fixture();
            fixture.Membership.MembershipOption.SubscriptionType = expired
                ? MembershipOption.ESubscriptionType.WeeklyPlatinum : MembershipOption.ESubscriptionType.Free;
            if (expired)
                fixture.Membership.EndsOn = DateTime.Today.AddDays(-1);
            fixture.AssertError(fixture.Invoke(), 403, "membership_required");
            fixture.Users.Verify(e => e.Find(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void MissingBirthDetailsDoNotCreateUserInfoOrCalculate()
        {
            var fixture = new Fixture();
            fixture.Info.Clear();
            fixture.AssertError(fixture.Invoke(), 409, "birth_details_required");
            fixture.Users.Verify(e => e.GetOrCreateUserInfo(It.IsAny<int>()), Times.Never);
        }

        [Theory]
        [InlineData("", "timezone_required")]
        [InlineData("Not/A_Real_Zone", "invalid_timezone")]
        public void MissingOrInvalidTimezoneDoesNotSilentlyBecomeGmt(string zone, string code)
        {
            var fixture = new Fixture();
            fixture.Info[0].BirthTimeZoneId = zone;
            fixture.AssertError(fixture.Invoke(), 409, code);
        }

        [Fact]
        public void SavedOptionsAndBirthTimeAreForwardedExactlyOnceAndResponseIsAnAllowList()
        {
            var fixture = new Fixture();
            var info = fixture.Info[0];
            // Simulate a stored birthday that already contains a time: normalise before adding the saved time.
            fixture.User.BirthDate = fixture.User.BirthDate.AddHours(4);
            fixture.Users.Setup(e => e.GetUserPreference(42, SessionConstants.UserCalculationMethod, info.CalculationMethod))
                .Returns(ECalculationMethod.Traditional);
            fixture.Users.Setup(e => e.GetUserPreference(42, SessionConstants.DefaultCalculatorType, info.CalculatorType))
                .Returns(ECalculatorType.Advanced);
            fixture.Users.Setup(e => e.GetUserPreference(42, SessionConstants.InvertDailyAndHourlyKiForSouthernHemisphere, false)).Returns(true);
            fixture.Users.Setup(e => e.GetUserPreference(42, SessionConstants.InvertDailyAndHourlyCycleKiForSouthernHemisphere, false)).Returns(true);
            var profile = fixture.CreateProfile(fixture.User.BirthDate.Date);
            fixture.Profiles.Setup(e => e.CalculateNineStarKiProfile(
                It.Is<PersonModel>(p => p.DateOfBirth == new DateTime(1979, 6, 16, 8, 15, 0) &&
                    p.DateOfBirth.Kind == DateTimeKind.Unspecified && p.TimeOfBirth == info.TimeOfBirth &&
                    p.BirthTimeZoneId == "UTC" && p.Gender == EGender.Male),
                false, true, new DateTime(2026, 10, 16, 12, 0, 0), ECalculationMethod.Traditional,
                ECalculatorType.Advanced, false, false, "UTC", EHousesDisplay.SolarHouse,
                true, true, EDisplayDataForPeriod.SelectedDate, false)).Returns(profile);

            var json = fixture.Invoke();
            Assert.True((bool)json["success"]);
            Assert.Equal(200, fixture.Response.Object.StatusCode);
            Assert.Equal("1979-06-16", (string)json["data"]["person"]["dateOfBirth"]);
            Assert.Equal("08:15:00", (string)json["data"]["person"]["timeOfBirth"]);
            Assert.Equal("Traditional", (string)json["data"]["settings"]["calculationMethod"]);
            Assert.Equal("Advanced", (string)json["data"]["settings"]["calculatorType"]);
            Assert.Equal(profile.MainEnergy.IntellectualQualities, (string)json["data"]["sections"]["intellectualQualities"]["body"]);
            Assert.Equal(profile.MainEnergy.EnergyNumber, (int)json["data"]["energies"]["core"]["number"]);
            Assert.Null(json["data"]["person"]["emailAddress"]);
            Assert.Null(json["data"]["user"]);
            Assert.Null(json["data"]["membership"]);
            Assert.Null(json["data"]["plannerViewModel"]);
            fixture.Profiles.VerifyAll();
            fixture.Cache.Verify(e => e.SetNoStore(), Times.Once);

            // Every request rechecks entitlement, even when calculation data is cached.
            fixture.Membership.IsDeactivated = true;
            fixture.AssertError(fixture.Invoke(), 403, "membership_required");
            fixture.Profiles.Verify(e => e.CalculateNineStarKiProfile(It.IsAny<PersonModel>(),
                It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<DateTime?>(), It.IsAny<ECalculationMethod>(),
                It.IsAny<ECalculatorType>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(),
                It.IsAny<EHousesDisplay>(), It.IsAny<bool>(), It.IsAny<bool>(),
                It.IsAny<EDisplayDataForPeriod>(), It.IsAny<bool>()), Times.Once);
            fixture.Users.Verify(e => e.UpdateUserPreference(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<object>()), Times.Never);
        }

        [Fact]
        public void MapperPreservesFullTextAndOmitsRelationshipSectionsForChildren()
        {
            var fixture = new Fixture();
            var child = fixture.CreateProfile(DateTime.Today.AddYears(-10));
            var result = MobilePersonalChartMapper.Map(child, false);
            Assert.False(result.Person.IsSixteenOrOver);
            Assert.False(result.Help.ContainsKey("relationships"));
            Assert.Equal(K9.Globalisation.Dictionary.MainEnergyDescription, result.Help["core"].HtmlBody);
            Assert.False(result.Sections.ContainsKey("relationships"));
            Assert.False(result.Sections.ContainsKey("relationshipsStraight"));
            Assert.False(result.Sections.ContainsKey("relationshipsLgbtq"));
            Assert.Equal(child.CharacterEnergy.ChildDescription, result.Sections["childNature"].Body);
            Assert.Equal(child.CharacterEnergy.ChildStressDescription, result.Sections["childStressResponse"].Body);
        }

        [Fact]
        public void FreeProfileProjectionContainsOnlyTheGrantedSections()
        {
            var fixture = new Fixture();
            var profile = fixture.CreateProfile(new DateTime(1979, 6, 16));
            var json = JObject.FromObject(MobilePersonalChartMapper.FreeProfile(profile));
            Assert.Equal(K9.Globalisation.Dictionary.MainEnergyDescription, (string)json["Help"]["core"]["HtmlBody"]);
            Assert.Null(json["Help"]["adultChildRelationship"]);
            var sections = Assert.IsType<JObject>(json["Sections"]);
            Assert.Equal(new[] { "Core", "Character", "IntellectualQualities" },
                System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Select(sections.Properties(), p => p.Name)));
            var energies = Assert.IsType<JObject>(json["Energies"]);
            Assert.Equal(new[] { "character", "core", "dayStar", "epoch", "generation", "hour", "socialExpression" },
                energies.Properties().Select(e => e.Name).OrderBy(e => e).ToArray());
            var metadataFields = new[] { "Colour", "Direction", "FamilyMember", "InternalRepresentation",
                "MostChallengingCombinations", "MostHarmoniousCombinations", "Number", "Physiognomy", "PrimaryAttributes", "Season" };
            foreach (var entry in energies.Properties())
            {
                var metadata = Assert.IsType<JObject>(entry.Value);
                // Exact allowlist: no paid descriptions can enter the free DTO.
                Assert.Equal(metadataFields, metadata.Properties().Select(e => e.Name).OrderBy(e => e).ToArray());
            }
            Assert.Equal((int)json["Core"], (int)energies["core"]["Number"]);
            Assert.Equal((int)json["Character"], (int)energies["character"]["Number"]);
            Assert.Equal((int)json["SocialExpression"], (int)energies["socialExpression"]["Number"]);
            Assert.Equal(profile.MainEnergy.SeasonDescription, (string)energies["core"]["Season"]);
            Assert.Equal(profile.MainEnergy.MetaData.GetInternalRepresentation(), (string)energies["core"]["InternalRepresentation"]);
            Assert.Null(json["StrongYinOrgan"]);
            Assert.Equal(profile.MainEnergy.EnergyDescription, (string)sections["Core"]["Body"]);
            Assert.Equal(profile.CharacterEnergy.EnergyDescription, (string)sections["Character"]["Body"]);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        public void FreeAndFullEnergyMetadataMatchEveryWebsiteHeaderRow(int number)
        {
            var energy = new NineStarKiEnergy((ENineStarKiEnergy)number);
            var metadata = energy.MetaData;
            var overview = MobilePersonalChartMapper.EnergyOverview(energy);
            var full = MobilePersonalChartMapper.Energy(energy);
            foreach (var value in new MobileChartEnergyOverview[] { overview, full })
            {
                Assert.Equal(number, value.Number);
                Assert.Equal(metadata.GetColour(), value.Colour);
                Assert.Equal(metadata.GetDirection(), value.Direction);
                Assert.Equal(metadata.GetFamilyMember(), value.FamilyMember);
                Assert.Equal(energy.SeasonDescription, value.Season);
                Assert.Equal(metadata.GetInternalRepresentation(), value.InternalRepresentation);
                Assert.Equal(metadata.GetPhysiognomy(), value.Physiognomy);
                Assert.Equal(metadata.GetPrimaryAttributes(), value.PrimaryAttributes);
                Assert.Equal(metadata.GetMostHarmoniousCombinationsString(), value.MostHarmoniousCombinations);
                Assert.Equal(metadata.GetMostChallengingCombinationsString(), value.MostChallengingCombinations);
            }
            Assert.Equal(energy.EnergyDescription, full.Description);
            Assert.Equal(energy.ElementDescription, full.ElementDescription);
            Assert.Equal(energy.ModalityDescription, full.ModalityDescription);
            Assert.Equal(energy.TrigramDescription, full.TrigramDescription);
            Assert.Null(JObject.FromObject(overview)["Description"]);
            Assert.Null(JObject.FromObject(overview)["ElementDescription"]);
        }

        [Fact]
        public void MissingEnergyHasNoOverviewOrFullProjection()
        {
            Assert.Null(MobilePersonalChartMapper.EnergyOverview(null));
            Assert.Null(MobilePersonalChartMapper.Energy(null));
        }

        [Fact]
        public void EmbeddedProfilesCanOmitRepeatedChartHelpWithoutChangingReadings()
        {
            var fixture = new Fixture();
            var profile = fixture.CreateProfile(new DateTime(1979, 6, 16));
            var result = MobilePersonalChartMapper.Map(profile, false, includeHelp: false);
            Assert.Null(result.Help);
            Assert.Equal(profile.MainEnergy.EnergyDescription, result.Sections["core"].Body);
            Assert.Equal(profile.MainEnergy.EnergyNumber, result.Energies["core"].Number);
        }

        [Fact]
        public void HealthPanelsMatchWebsiteResourcesAndCalculatedOrgans()
        {
            var fixture = new Fixture();
            var profile = fixture.CreateProfile(new DateTime(1979, 6, 16));
            var result = MobilePersonalChartMapper.Map(profile, false);
            Assert.Equal(K9.Globalisation.Dictionary.StrongOrgans, result.Health.StrongOrgans.Title);
            Assert.Equal(K9.Globalisation.Dictionary.StrongOrgansDescription, result.Health.StrongOrgans.HtmlBody);
            Assert.Equal(K9.Globalisation.Dictionary.WeakOrgans, result.Health.WeakOrgans.Title);
            Assert.Equal(K9.Globalisation.Dictionary.WeakOrgansDescription, result.Health.WeakOrgans.HtmlBody);
            Assert.Equal(profile.MainEnergy.YinYangExpansionTitle, result.Energies["core"].YinYangExpansionTitle);
            Assert.Equal(profile.MainEnergy.YinYangExpansionDescription, result.Energies["core"].YinYangExpansionDescription);

            var expected = new[] { profile.StrongYinOrgans.Value, profile.StrongYangOrgans.Value }
                .Concat(profile.WeakYinOrgans).Concat(profile.WeakYangOrgans).ToArray();
            var actual = result.Health.StrongOrgans.YinOrgans.Concat(result.Health.StrongOrgans.YangOrgans)
                .Concat(result.Health.WeakOrgans.YinOrgans).Concat(result.Health.WeakOrgans.YangOrgans).ToArray();
            Assert.Equal(expected.Select(e => e.ToString()), actual.Select(e => e.Value));
            Assert.Equal(expected.Select(e => e.GetAttribute<EnumDescriptionAttribute>().GetDescription()),
                actual.Select(e => e.Label));
            Assert.Equal(result.StrongYinOrganArtworkId, result.Health.StrongOrgans.YinOrgans.Single().ArtworkId);
            Assert.Equal(result.WeakYangOrganArtworkIds, result.Health.WeakOrgans.YangOrgans.Select(e => e.ArtworkId).ToArray());
            Assert.Null(JObject.FromObject(MobilePersonalChartMapper.FreeProfile(profile))["Health"]);
        }

        [Fact]
        public void CharacterNavigationTitlesMatchWebsiteWithoutChangingReadingsOrHelp()
        {
            var profile = new Fixture().CreateProfile(new DateTime(1979, 6, 16));
            var result = MobilePersonalChartMapper.Map(profile, false);
            Assert.Equal(profile.CharacterEnergy.DescriptiveTitle, result.Sections["character"].Title);
            Assert.Equal(K9.Globalisation.Dictionary.ChildNature, result.Sections["childNature"].Title);
            Assert.Equal(K9.Globalisation.Dictionary.AdultChildRelationshipTitle, result.Sections["adultChildRelationship"].Title);
            Assert.Equal(K9.Globalisation.Dictionary.AboutAdultChildRelationship, result.Help["adultChildRelationship"].Title);
            foreach (var key in new[] { "stressResponse", "natalHouseStressResponse", "childStressResponse" })
                Assert.Equal(K9.Globalisation.Dictionary.StressResponse, result.Sections[key].Title);
            Assert.Equal(profile.CharacterEnergy.ChildDescription, result.Sections["childNature"].Body);
            Assert.Equal(profile.CharacterEnergy.EnergyDescription, result.Sections["character"].Body);
            Assert.Equal(profile.CharacterEnergy.CharacterEnergySummary, result.Sections["character"].Summary);
            Assert.Equal(K9.Globalisation.Dictionary.AboutCharacterNature, result.Help["character"].Title);
            Assert.Equal(profile.StressResponseDetails, result.Sections["stressResponse"].Body);
            Assert.Equal(profile.StressResponseFromNatalHouseDetails, result.Sections["natalHouseStressResponse"].Body);
            Assert.Equal(profile.CharacterEnergy.ChildStressDescription, result.Sections["childStressResponse"].Body);

            var serializer = Newtonsoft.Json.JsonSerializer.Create(new Newtonsoft.Json.JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
            var json = JObject.FromObject(result, serializer);
            Assert.Equal(K9.Globalisation.Dictionary.ChildNature, (string)json["sections"]["childNature"]["title"]);
            Assert.NotNull(json["sections"]["character"]["body"]);
            Assert.NotNull(json["sections"]["character"]["summary"]);
            var embedded = MobilePersonalChartMapper.Map(profile, false, includeHelp: false);
            Assert.Null(embedded.Help);
            Assert.Equal(result.Sections["childNature"].Title, embedded.Sections["childNature"].Title);

            var free = JObject.FromObject(MobilePersonalChartMapper.FreeProfile(profile), serializer);
            Assert.Equal(profile.CharacterEnergy.DescriptiveTitle, (string)free["sections"]["character"]["title"]);
            Assert.Null(free["sections"]["childNature"]);
            Assert.Null(free["sections"]["stressResponse"]);
            Assert.Null(free["sections"]["adultChildRelationship"]);
        }

        private sealed class TestController : MobilePersonalChartController
        {
            private readonly int _userId;
            public TestController(INineStarKiPackage package, INineStarKiService profiles, int userId)
                : base(package, profiles) { _userId = userId; }
            protected override int ResolveCurrentUserId() => _userId;
            protected override DateTime GetUtcNow() => new DateTime(2026, 10, 16, 12, 0, 0, DateTimeKind.Utc);
        }

        private sealed class Fixture
        {
            public readonly Mock<IUserService> Users = new Mock<IUserService>();
            public readonly Mock<IMembershipService> Memberships = new Mock<IMembershipService>();
            public readonly Mock<INineStarKiService> Profiles = new Mock<INineStarKiService>(MockBehavior.Strict);
            public readonly Mock<IRepository<UserInfo>> Repository = new Mock<IRepository<UserInfo>>();
            public readonly Mock<HttpResponseBase> Response = new Mock<HttpResponseBase>();
            public readonly Mock<HttpCachePolicyBase> Cache = new Mock<HttpCachePolicyBase>();
            public readonly List<UserInfo> Info = new List<UserInfo>
            {
                new UserInfo { UserId = 42, BirthTimeZoneId = "UTC", TimeOfBirth = new TimeSpan(8, 15, 0) }
            };
            public readonly User User = new User { Id = 42, BirthDate = new DateTime(1979, 6, 16), Gender = EGender.Male };
            public readonly UserMembership Membership = new UserMembership
            {
                UserId = 42, StartsOn = DateTime.Today.AddDays(-1), EndsOn = DateTime.Today.AddDays(1),
                MembershipOption = new MembershipOption { SubscriptionType = MembershipOption.ESubscriptionType.WeeklyPlatinum }
            };
            private readonly TestController _controller;

            public Fixture(bool authenticated = true, int userId = 42)
            {
                Users.Setup(e => e.Find(42)).Returns(User);
                Memberships.Setup(e => e.GetActiveUserMembership(42)).Returns(Membership);
                Repository.Setup(e => e.Find(It.IsAny<Expression<Func<UserInfo, bool>>>())).Returns(Info);
                PreferenceFallback<string>();
                PreferenceFallback<bool>();
                PreferenceFallback<ECalculationMethod>();
                PreferenceFallback<ECalculatorType>();
                PreferenceFallback<EHousesDisplay>();
                var package = new Mock<INineStarKiPackage>();
                package.SetupGet(e => e.UserService).Returns(Users.Object);
                package.SetupGet(e => e.MembershipService).Returns(Memberships.Object);
                package.SetupGet(e => e.UserInfosRepository).Returns(Repository.Object);
                var request = new Mock<HttpRequestBase>();
                request.SetupGet(e => e.IsAuthenticated).Returns(authenticated);
                Response.SetupProperty(e => e.StatusCode);
                Response.SetupProperty(e => e.SuppressFormsAuthenticationRedirect);
                Response.SetupProperty(e => e.TrySkipIisCustomErrors);
                Response.SetupGet(e => e.Cache).Returns(Cache.Object);
                var context = new Mock<HttpContextBase>();
                context.SetupGet(e => e.Request).Returns(request.Object);
                context.SetupGet(e => e.Response).Returns(Response.Object);
                _controller = new TestController(package.Object, Profiles.Object, userId);
                _controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), _controller);
            }

            private void PreferenceFallback<T>()
            {
                Users.Setup(e => e.GetUserPreference(42, It.IsAny<string>(), It.IsAny<T>()))
                    .Returns((int id, string key, T fallback) => fallback);
            }

            public JObject Invoke()
            {
                var result = Assert.IsType<ContentResult>(_controller.MyChart());
                Assert.Equal("application/json", result.ContentType);
                Assert.True(Response.Object.SuppressFormsAuthenticationRedirect);
                Assert.True(Response.Object.TrySkipIisCustomErrors);
                return JObject.Parse(result.Content);
            }

            public void AssertError(JObject json, int status, string code)
            {
                Assert.Equal(status, Response.Object.StatusCode);
                Assert.False((bool)json["success"]);
                Assert.Equal(JTokenType.Null, json["data"].Type);
                Assert.Equal(code, (string)json["error"]["code"]);
            }

            public NineStarKiModel CreateProfile(DateTime birthDate)
            {
                return new NineStarKiModel(new PersonModel
                {
                    DateOfBirth = birthDate.Add(Info[0].TimeOfBirth), TimeOfBirth = Info[0].TimeOfBirth,
                    BirthTimeZoneId = "UTC", Gender = EGender.Male
                }, 5, 5, 3, 1, 1, 5, 5, 5, 5, 5, 5, 5,
                    new (int DailyKi, int? InvertedDailyKi)[] { (5, null), (5, null) }, 5,
                    selectedDate: new DateTime(2026, 10, 16, 12, 0, 0),
                    calculationMethod: ECalculationMethod.Traditional, calculatorType: ECalculatorType.Advanced,
                    housesDisplay: EHousesDisplay.SolarHouse, userTimeZoneId: "UTC",
                    invertDailyAndHourlyKiForSouthernHemisphere: true, invertDailyAndHourlyCycleKiForSouthernHemisphere: true);
            }
        }
    }
}
