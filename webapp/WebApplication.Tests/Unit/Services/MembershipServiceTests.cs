using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Config;
using K9.WebApplication.Services;
using K9.WebApplication.Services.Stripe;
using Moq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{

    public class MembershipModelserviceTests
    {
        private readonly Mock<IRepository<User>> _usersRepository = new Mock<IRepository<User>>();
        private readonly Mock<IRepository<UserMembership>> _userMembershipRepository = new Mock<IRepository<UserMembership>>();
        private readonly Mock<IRepository<MembershipOption>> _membershipOptionRepository = new Mock<IRepository<MembershipOption>>();
        private readonly Mock<IRepository<UserProfileReading>> _userProfileReadingRepository = new Mock<IRepository<UserProfileReading>>();
        private readonly Mock<IRepository<UserRelationshipCompatibilityReading>> _userRelationshipCompatibilityReadingsRepository = new Mock<IRepository<UserRelationshipCompatibilityReading>>();
        private readonly Mock<IRepository<UserCreditPack>> _userCreditPackRepository = new Mock<IRepository<UserCreditPack>>();
        private readonly Mock<ILogger> _logger = new Mock<ILogger>();
        private readonly Mock<IMailer> _mailer = new Mock<IMailer>();
        private readonly Mock<IOptions<WebsiteConfiguration>> _config = new Mock<IOptions<WebsiteConfiguration>>();
        private readonly Mock<IAuthentication> _authentication = new Mock<IAuthentication>();
        private readonly Mock<IOptions<StripeConfiguration>> _stripeConfig = new Mock<IOptions<StripeConfiguration>>();
        private readonly Mock<IStripeService> _stripeService = new Mock<IStripeService>();
        private readonly Mock<IContactService> _contactService = new Mock<IContactService>();
        private MembershipService _Membershipservice;

        private readonly int _userId = 1;

        private readonly MembershipOption _standardMonthlyMembership = new MembershipOption
        {
            Id = 1,
            Name = "Monthly Standard Membership",
            SubscriptionType = MembershipOption.ESubscriptionType.MonthlyStandard,
            Price = 10
        };

        private readonly MembershipOption _standardYearlyMembership = new MembershipOption
        {
            Id = 2,
            Name = "Annual Standard Membership",
            SubscriptionType = MembershipOption.ESubscriptionType.AnnualStandard,
            Price = 90
        };

        private readonly MembershipOption _platinumMonthlyMembership = new MembershipOption
        {
            Id = 3,
            Name = "Monthly Platinum Membership",
            SubscriptionType = MembershipOption.ESubscriptionType.MonthlyPlatinum,
            Price = 20
        };

        private readonly MembershipOption _platinumYearlyMembership = new MembershipOption
        {
            Id = 4,
            Name = "Yearly Platinum Membership",
            SubscriptionType = MembershipOption.ESubscriptionType.AnnualPlatinum,
            Price = 180
        };

        public MembershipModelserviceTests()
        {
            var membershipOptions = new List<MembershipOption>
            {
                _standardMonthlyMembership,
                _standardYearlyMembership,
                _platinumMonthlyMembership,
                _platinumYearlyMembership
            };

            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
            );

            _config.SetupGet(_ => _.Value).Returns(new WebsiteConfiguration
            {
                CompanyLogoUrl = "http://local",
                CompanyName = "Glantz Consulting",
                SupportEmailAddress = "support@gc.com"
            });

            _stripeConfig.SetupGet(_ => _.Value).Returns(new StripeConfiguration
            {
                SecretKey = "sk_12348765",
                PublishableKey = "pk_09872345"
            });

            _membershipOptionRepository.Setup(_ => _.List()).Returns(membershipOptions);
            _membershipOptionRepository.Setup(_ => _.Find(_standardMonthlyMembership.Id)).Returns(_standardMonthlyMembership);
            _membershipOptionRepository.Setup(_ => _.Find(_standardYearlyMembership.Id)).Returns(_standardYearlyMembership);
            _membershipOptionRepository.Setup(_ => _.Find(_platinumMonthlyMembership.Id)).Returns(_platinumMonthlyMembership);
            _membershipOptionRepository.Setup(_ => _.Find(_platinumYearlyMembership.Id)).Returns(_platinumYearlyMembership);

            _Membershipservice = new MembershipService(
                _logger.Object,
                _authentication.Object,
                _membershipOptionRepository.Object,
                _userMembershipRepository.Object,
                _userProfileReadingRepository.Object,
                _userRelationshipCompatibilityReadingsRepository.Object,
                _userCreditPackRepository.Object,
                _usersRepository.Object,
                _contactService.Object,
                _mailer.Object,
                _config.Object,
                new Mock<IRepository<PromoCode>>().Object);
        }

        [Fact]
        public void GetMembershipModel_StandardMonthly_CanUpgradeThree()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var userMembershipModels = new List<UserMembership>
            {
                new UserMembership
                {
                    UserId = _userId,
                    MembershipOptionId = _standardMonthlyMembership.Id,
                    MembershipOption = _standardMonthlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddMonths(1)
                }
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMembershipModels.AsQueryable());

            var model = _Membershipservice.GetMembershipViewModel();

            Assert.Equal(1, _Membershipservice.GetActiveUserMemberships().Count);
            Assert.Equal(userMembershipModels.First(), _Membershipservice.GetActiveUserMembership());
            Assert.Equal(0, model.MembershipModels.Count(_ => _.IsSelected));
            Assert.Equal(3, model.MembershipModels.Count(_ => _.IsUpgrade));
            Assert.Equal(1, model.MembershipModels.Count(_ => _.IsSubscribed));
            Assert.Equal(3, model.MembershipModels.Count(_ => _.IsSelectable));
        }

        [Fact]
        public void GetMembershipModel_StandardYearly_CanUpgradeTwo()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var userMembershipModels = new List<UserMembership>
            {
                new UserMembership
                {
                    UserId = _userId,
                    MembershipOptionId = _standardYearlyMembership.Id,
                    MembershipOption = _standardYearlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddYears(1)
                }
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMembershipModels.AsQueryable());

            var model = _Membershipservice.GetMembershipViewModel();

            Assert.Equal(1, _Membershipservice.GetActiveUserMemberships().Count);
            Assert.Equal(userMembershipModels.First(), _Membershipservice.GetActiveUserMembership());
            Assert.Equal(0, model.MembershipModels.Count(_ => _.IsSelected));
            Assert.Equal(1, model.MembershipModels.Count(_ => _.IsUpgrade));
            Assert.Equal(1, model.MembershipModels.Count(_ => _.IsSubscribed));
            Assert.Equal(1, model.MembershipModels.Count(_ => _.IsSelectable));
        }

        [Fact]
        public void GetMembershipModel_PlatinumMonthly_CanUpgradeOne()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var userMembershipModels = new List<UserMembership>
            {
                new UserMembership
                {
                    UserId = _userId,
                    MembershipOptionId = _platinumMonthlyMembership.Id,
                    MembershipOption = _platinumMonthlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddMonths(1)
                }
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMembershipModels.AsQueryable());

            var model = _Membershipservice.GetMembershipViewModel();

            Assert.Equal(1, _Membershipservice.GetActiveUserMemberships().Count);
            Assert.Equal(userMembershipModels.First(), _Membershipservice.GetActiveUserMembership());
            Assert.Equal(0, model.MembershipModels.Count(_ => _.IsSelected));
            Assert.Equal(1, model.MembershipModels.Count(_ => _.IsUpgrade));
            Assert.Equal(1, model.MembershipModels.Count(_ => _.IsSubscribed));
            Assert.Equal(1, model.MembershipModels.Count(_ => _.IsSelectable));
        }

        [Fact]
        public void GetMembershipModel_PlatinumYearly_CanUpgradeNone()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var userMembershipModels = new List<UserMembership>
            {
                new UserMembership
                {
                    Id = 7,
                    UserId = _userId,
                    MembershipOptionId = _platinumYearlyMembership.Id,
                    MembershipOption = _platinumYearlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddYears(1),
                }
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMembershipModels.AsQueryable());

            var model = _Membershipservice.GetMembershipViewModel();

            Assert.Equal(1, _Membershipservice.GetActiveUserMemberships().Count);
            Assert.Equal(userMembershipModels.First(), _Membershipservice.GetActiveUserMembership());
            Assert.Equal(0, model.MembershipModels.Count(_ => _.IsSelected));
            Assert.Equal(0, model.MembershipModels.Count(_ => _.IsUpgrade));
            Assert.Equal(1, model.MembershipModels.Count(_ => _.IsSubscribed));
            Assert.Equal(0, model.MembershipModels.Count(_ => _.IsSelectable));
            Assert.Equal(7, model.MembershipModels.First().ActiveUserMembershipId);
        }

        [Fact]
        public void GetMembershipModel_CanUpgradeOne_AndSwitchOne()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var scheduledStartsOn = startsOn.AddMonths(1).AddDays(5);
            var scheduledUserMembership = new UserMembership
            {
                Id = 8,
                UserId = _userId,
                MembershipOptionId = _standardMonthlyMembership.Id,
                MembershipOption = _standardMonthlyMembership,
                StartsOn = scheduledStartsOn,
                EndsOn = scheduledStartsOn.AddMonths(1),
                IsAutoRenew = true
            };
            var userMembershipModels = new List<UserMembership>
            {
                new UserMembership
                {
                    Id = 7,
                    UserId = _userId,
                    MembershipOptionId = _platinumMonthlyMembership.Id,
                    MembershipOption = _platinumMonthlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddMonths(1)
                },
                scheduledUserMembership
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMembershipModels.AsQueryable());

            var model = _Membershipservice.GetMembershipViewModel();
            
            Assert.Equal(1, _Membershipservice.GetActiveUserMemberships().Count);
            Assert.Equal(userMembershipModels.First(), _Membershipservice.GetActiveUserMembership());
            Assert.Equal(0, model.MembershipModels.Count(_ => _.IsSelected));
            Assert.Equal(1, model.MembershipModels.Count(_ => _.IsUpgrade));
            Assert.Equal(1, model.MembershipModels.Count(_ => _.IsSubscribed));
            Assert.Equal(1, model.MembershipModels.Count(_ => _.IsSelectable));
            Assert.Equal(7, model.MembershipModels.First().ActiveUserMembershipId);
        }

        [Fact]
        public void GetSwitchMembershipModel_ShouldThrowError_NoSubscriptions()
        {
            AuthenticateUser();

            var ex = Assert.Throws<Exception>(() => _Membershipservice.GetSwitchMembershipModel(1));
            Assert.Equal(Globalisation.Dictionary.SwitchMembershipErrorNotSubscribed, ex.Message);
        }

        [Fact]
        public void GetSwitchMembershipModel_ShouldThrowError_SameSubscriptionId()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var userMembershipModels = new List<UserMembership>
            {
                new UserMembership
                {
                    Id = 7,
                    UserId = _userId,
                    MembershipOptionId = _standardMonthlyMembership.Id,
                    MembershipOption = _standardMonthlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddMonths(1)
                }
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMembershipModels.AsQueryable());


            var ex = Assert.Throws<Exception>(() => _Membershipservice.GetSwitchMembershipModel(_standardMonthlyMembership.Id));
            Assert.Equal(Globalisation.Dictionary.SwitchMembershipErrorAlreadySubscribed, ex.Message);
        }

        [Fact]
        public void GetSwitchMembershipModel_ShouldThrowError_ScheduledIsSameSubscriptionId()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var scheduledStartsOn = startsOn.AddMonths(1).AddDays(1);
            var scheduledUserMembership = new UserMembership
            {
                Id = 8,
                UserId = _userId,
                MembershipOptionId = _standardYearlyMembership.Id,
                MembershipOption = _standardYearlyMembership,
                StartsOn = scheduledStartsOn,
                EndsOn = scheduledStartsOn.AddMonths(1),
                IsAutoRenew = true
            };
            var userMembershipModels = new List<UserMembership>
            {
                new UserMembership
                {
                    Id = 7,
                    UserId = _userId,
                    MembershipOptionId = _platinumMonthlyMembership.Id,
                    MembershipOption = _platinumMonthlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddMonths(1)
                },
                scheduledUserMembership
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMembershipModels.AsQueryable());


            var ex = Assert.Throws<Exception>(() => _Membershipservice.GetSwitchMembershipModel(_standardYearlyMembership.Id));
            Assert.Equal(Globalisation.Dictionary.CannotSwitchMembershipError, ex.Message);
        }

        [Fact]
        public void GetSwitchMembershipModel_IsUpgrade()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var userMembershipModels = new List<UserMembership>
            {
                new UserMembership
                {
                    Id = 7,
                    UserId = _userId,
                    MembershipOptionId = _standardMonthlyMembership.Id,
                    MembershipOption = _standardMonthlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddMonths(1)
                }
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMembershipModels.AsQueryable());


            var result = _Membershipservice.GetSwitchMembershipModel(_standardYearlyMembership.Id);
            Assert.True(result.IsUpgrade);
        }
        
        [Fact]
        public void GetPurchaseMembershipModel_ShouldThrowError_IfAlreadySubscribed()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var userMembershipModels = new List<UserMembership>
            {
                new UserMembership
                {
                    Id = 7,
                    UserId = _userId,
                    MembershipOptionId = _standardYearlyMembership.Id,
                    MembershipOption = _standardYearlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddMonths(1)
                }
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMembershipModels.AsQueryable());

            var ex = Assert.Throws<Exception>(() => _Membershipservice.GetPurchaseMembershipModel(_standardYearlyMembership.Id));
            Assert.Equal(Globalisation.Dictionary.PurchaseMembershipErrorAlreadySubscribed, ex.Message);
        }

        [Fact]
        public void GetPurchaseMembershipModel_ShouldThrowError_IfSubscriptionsAlreadyExist()
        {
            AuthenticateUser();

            var startsOn = DateTime.Today.AddDays(-7);
            var userMembershipModels = new List<UserMembership>
            {
                new UserMembership
                {
                    Id = 7,
                    UserId = _userId,
                    MembershipOptionId = _standardYearlyMembership.Id,
                    MembershipOption = _standardYearlyMembership,
                    StartsOn = startsOn,
                    EndsOn = startsOn.AddMonths(1)
                }
            };

            _userMembershipRepository.Setup(_ => _.Find(It.IsAny<System.Linq.Expressions.Expression<Func<UserMembership, bool>>>()))
                .Returns(userMembershipModels.AsQueryable());

            var ex = Assert.Throws<Exception>(() => _Membershipservice.GetPurchaseMembershipModel(_platinumYearlyMembership.Id));
            Assert.Equal(Globalisation.Dictionary.PurchaseMembershipErrorAlreadySubscribedToAnother, ex.Message);
        }

        private void AuthenticateUser()
        {
            _authentication.SetupGet(_ => _.IsAuthenticated).Returns(true);
            _authentication.SetupGet(_ => _.CurrentUserId).Returns(_userId);
        }
    }
}
