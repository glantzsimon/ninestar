using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Models;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.ViewModels;
using System;
using System.Threading.Tasks;

namespace K9.WebApplication.Services
{
    public class ReportsService : BaseService, IReportsService
    {
        private readonly IRepository<Promotion> _promotionsRepository;
        private readonly IRepository<UserPromotion> _userPromotionsRepository;
        private readonly IRepository<UserMembership> _userMembershipsRepository;
        private readonly IRepository<MembershipOption> _membershipOptionsRepository;
        private readonly IContactService _contactService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailQueueService _emailQueueService;
        private readonly IAccountService _accountService;
        private readonly IAstronomyService _astronomyService;
        private readonly INineStarKiService _nineStarKiService;
        private readonly IAIService _aiService;

        public ReportsService(INineStarKiBasePackage my, IRepository<Promotion> promotionsRepository, IRepository<UserPromotion> userPromotionsRepository, IRepository<UserMembership> userMembershipsRepository, IRepository<UserOTP> userOtpRepository, IRepository<MembershipOption> membershipOptionsRepository, IContactService contactService, IEmailTemplateService emailTemplateService, IEmailQueueService emailQueueService, IAccountService accountService,
            IAstronomyService astronomyService, INineStarKiService nineStarKiService, IAIService aiService)
            : base(my)
        {
            _promotionsRepository = promotionsRepository;
            _userPromotionsRepository = userPromotionsRepository;
            _userMembershipsRepository = userMembershipsRepository;
            _membershipOptionsRepository = membershipOptionsRepository;
            _contactService = contactService;
            _emailTemplateService = emailTemplateService;
            _emailQueueService = emailQueueService;
            _accountService = accountService;
            _astronomyService = astronomyService;
            _nineStarKiService = nineStarKiService;
            _aiService = aiService;
        }

        public async Task<string> GetYearlyReport(int userId)
        {
            var myAccount = _accountService.GetAccount(userId);
            return await GetYearlyReport(myAccount);
        }

        public async Task<string> GetYearlyReport(Guid userId)
        {
            var myAccount = _accountService.GetAccount(userId);
            return await GetYearlyReport(myAccount);
        }

        private async Task<string> GetYearlyReport(MyAccountViewModel myAccount)
        {
            var personModel = new PersonModel
            {
                Name = myAccount.User.FullName,
                DateOfBirth = myAccount.User.BirthDate.Add(myAccount.UserInfo.TimeOfBirth),
                Gender = myAccount.User.Gender,
                TimeOfBirth = myAccount.UserInfo.TimeOfBirth,
                BirthTimeZoneId = myAccount.UserInfo.BirthTimeZoneId
            };

            var now = new DateTime(DateTime.UtcNow.Year, 2, 5);
            var lichun = _astronomyService.GetLichun(now, personModel.BirthTimeZoneId);

            var nineStarKiModel = _nineStarKiService.CalculateNineStarKiProfile(personModel, false, false, now,
                ECalculationMethod.Chinese, true, true,
                personModel.BirthTimeZoneId, EHousesDisplay.SolarHouse, false, false, EDisplayDataForPeriod.SelectedDate);

            var plannerData = _nineStarKiService.GetPlannerData(personModel.DateOfBirth.Date, personModel.BirthTimeZoneId,
                personModel.TimeOfBirth, personModel.Gender, now, nineStarKiModel.UserTimeZoneId,
                nineStarKiModel.CalculationMethod, nineStarKiModel.DisplayDataForPeriod, nineStarKiModel.HousesDisplay,
                nineStarKiModel.InvertDailyAndHourlyKiForSouthernHemisphere,
                nineStarKiModel.InvertDailyAndHourlyCycleKiForSouthernHemisphere,
                EPlannerView.Year, EScopeDisplay.PersonalKi, EPlannerNavigationDirection.None, nineStarKiModel);

            var report = await _aiService.GetYearlyReport(new YearlyReportViewModel
            {
                NineStarKiModel = nineStarKiModel,
                YearlyPlannerModel = plannerData
            });

            return report;
        }
    }
}