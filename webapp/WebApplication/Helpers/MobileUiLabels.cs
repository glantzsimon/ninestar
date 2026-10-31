using K9.Globalisation;
using K9.WebApplication.Enums;
using System.Globalization;
using BaseDictionary = K9.Base.Globalisation.Dictionary;

namespace K9.WebApplication.Helpers
{
    // Explicit public UI allowlist. Never enumerate Dictionary: it also contains paid reading text.
    public static class MobileUiLabels
    {
        public static object Map() => new
        {
            Version = 1,
            Culture = (Dictionary.Culture ?? CultureInfo.CurrentUICulture).Name,
            Shared = new
            {
                DateTime = DateTimeUi(),
                Feedback = Feedback(),
                ChartUi = ChartUi(),
                Navigation = NavigationUi(),
                Actions = new
                {
                    Calculate = Dictionary.Calculate,
                    EditCalculation = Dictionary.EditCalculation,
                    Save = Dictionary.Save,
                    Cancel = Dictionary.Cancel,
                    Back = Dictionary.Back,
                    MoreDetails = Dictionary.MoreDetails,
                    ViewFullText = Dictionary.FullTextView,
                    ViewSummary = Dictionary.SummaryView,
                    Generate = Dictionary.Generate,
                    GenerateHexagram = Dictionary.GenerateHexagram,
                    Yes = BaseDictionary.Yes,
                    No = BaseDictionary.No,
                    SignIn = Dictionary.SignIn,
                    SignOut = BaseDictionary.LogOut,
                    CreateAccount = Dictionary.CreateAccount,
                    Update = BaseDictionary.Update,
                    UpgradeMembership = Dictionary.UpgradeMembershipText
                },
                Calculator = new
                {
                    Ui = CalculatorUi(),
                    Validation = CalculatorValidation(),
                    Groups = new
                    {
                        BirthDetails = Dictionary.BirthDetails,
                        AdvancedPreferences = Dictionary.AdvancedCalculationPreferences,
                        SouthernHemisphere = Dictionary.SouthernHemisphereOptions
                    },
                    Fields = new
                    {
                        Name = BaseDictionary.NameLabel,
                        OptionalName = Dictionary.OptionalNameLabel,
                        DateOfBirth = Dictionary.DateOfBirthLabel,
                        TimeOfBirth = Dictionary.TimeOfBirthLabel,
                        TimeOfBirthKnown = Dictionary.TimeOfBirthKnownLabel,
                        BirthTimeZone = Dictionary.BirthTimeZone,
                        CurrentTimeZone = Dictionary.CurrentTimeZone,
                        Gender = Dictionary.GenderLabel,
                        CalculationMethod = Dictionary.CalculationMethodLabel,
                        HousesDisplay = Dictionary.HousesDisplayLabel,
                        DisplayDataFor = Dictionary.DisplayDataForLabel,
                        SelectedDate = Dictionary.SelectedDateLabel,
                        SelectedTime = Dictionary.SelectedTimeLabel,
                        InvertNatalKi = Dictionary.InvertDailyAndHourlyKiForSouthernHemisphereLabel,
                        InvertCycleKi = Dictionary.InvertDailyAndHourlyCycleKiForSouthernHemisphereLabel
                    },
                    Help = new
                    {
                        CurrentTimeZone = Dictionary.CurrentTimeZoneHelp
                    },
                    DisplayDataForPeriods = MobileMetadataHelper.EnumOptions(typeof(EDisplayDataForPeriod))
                }
            },
            Navigation = new
            {
                YourChart = Dictionary.YourChart,
                PersonalChart = Dictionary.PersonalChart,
                Predictions = Dictionary.Predictions,
                Compatibility = Dictionary.Compatibility,
                Iching = Dictionary.IChing,
                KarmicNumerology = Dictionary.KarmicNumerology,
                Calendar = Dictionary.Calendar,
                Biorhythms = Dictionary.BiorhythmsLabel,
                AiAstrologer = Dictionary.AIAstrologer,
                KnowledgeBase = Dictionary.KnowledgeBase,
                Articles = Dictionary.Articles,
                Account = BaseDictionary.MyAccountTitle
            },
            Screens = new
            {
                Explore = ExploreUi(),
                YourChart = new
                {
                    Ui = YourChartUi(),
                    Title = Dictionary.YourChart,
                    PersonalChart = Dictionary.YourPersonalChart,
                    Predictions = Dictionary.YourPredictions,
                    Calendar = Dictionary.YourCalendar
                },
                PersonalChart = new
                {
                    Titles = new
                    {
                        Page = Dictionary.YourPersonalChart,
                        Calculator = Dictionary.PersonalChartCalculator,
                        Hub = Dictionary.YourChart
                    },
                    Groups = new
                    {
                        Core = Dictionary.CoreEnergies,
                        Extended = Dictionary.ExtendedProfile,
                        Summary = Dictionary.SummaryLabel
                    },
                    Menu = new
                    {
                        Overview = Dictionary.OverviewLabel,
                        Essence = Dictionary.Essence,
                        IntellectualQualities = Dictionary.IntellectualQualities,
                        InterpersonalQualities = Dictionary.InterpersonalQualities,
                        EmotionalLandscape = Dictionary.EmotionalLandscape,
                        Relationships = Dictionary.Relationships,
                        Career = Dictionary.Career,
                        Finances = Dictionary.Finances,
                        Health = Dictionary.HealthLabel,
                        Spirituality = Dictionary.Spirituality,
                        CommonHealthChallenges = Dictionary.Illnesses,
                        ChildNature = Dictionary.ChildNature,
                        AdultChildRelationship = Dictionary.AdultChildRelationshipTitle,
                        StressResponse = Dictionary.StressResponse
                    },
                    Facts = new
                    {
                        Energy = Dictionary.Energy,
                        YinYang = Dictionary.YinYangLabel,
                        Element = Dictionary.Element,
                        Group = Dictionary.ModalityLabel,
                        Trigram = Dictionary.Trigram,
                        Colour = Dictionary.Colour,
                        Direction = Dictionary.Direction,
                        FamilyMember = Dictionary.FamilyMember,
                        Season = Dictionary.Season,
                        InternalRepresentation = Dictionary.InternalRepresentation,
                        Physiognomy = Dictionary.Physiognomy,
                        PrimaryAttributes = Dictionary.PrimaryAttributes,
                        MostHarmoniousCombinations = Dictionary.MostHarmoniousCombinations,
                        MostChallengingCombinations = Dictionary.MostChallengingCombinations
                    },
                    Roles = MobilePersonalChartMapper.RoleLabels()
                },
                Predictions = new { Ui = PredictionsUi(), CalculatorUi = PredictionCalculatorUi(), Titles = new { Page = Dictionary.YourPredictions, Calculator = Dictionary.PredictionsCalculator }, Labels = MobileCalculationMapper.PredictionLabels() },
                Compatibility = new { Labels = MobileCalculationMapper.CompatibilityLabels(false), Ui = CompatibilityUi() },
                Iching = new
                {
                    Ui = IChingUi(),
                    Title = Dictionary.IChing,
                    Calculator = Dictionary.IChingHexagramGenerator,
                    Consult = Dictionary.ConsultTheIChing,
                    Generate = Dictionary.GenerateHexagram,
                    GenerateShort = Dictionary.Generate,
                    YourHexagram = Dictionary.YourHexagram,
                    MainHexagram = Dictionary.MainHexagram,
                    TransformedHexagram = Dictionary.TransformedHexagram,
                    DetailedInterpretation = Dictionary.DetailedInterpretation,
                    HeadingFormat = Dictionary.HexagramHeadingFormat
                },
                KarmicNumerology = new
                {
                    Title = Dictionary.KarmicNumerology,
                    Calculator = Dictionary.KarmicNumerologyCalculator,
                    Blueprint = Dictionary.MainKarmicBlueprint,
                    Evolution = Dictionary.KarmicEvolution,
                    TwelveYearOutlook = Dictionary.TwelveYearOutlook,
                    TwelveMonthOutlook = Dictionary.TwelveMonthOutlook,
                    DailyInfluences = Dictionary.DailyInfluences,
                    DailyKarmicInfluences = Dictionary.DailyKarmicInfluences,
                    Emergence = Dictionary.Emergence,
                    Actualisation = Dictionary.Actualisation,
                    Mastery = Dictionary.Mastery,
                    EmergenceYears = Dictionary.EmergenceYears,
                    ActualisationYears = Dictionary.ActualisationYears,
                    MasteryYears = Dictionary.MasteryYears
                },
                Membership = new
                {
                    Title = Dictionary.MembershipLabel,
                    OptionsTitle = Dictionary.MembershipOptions,
                    WhyUpgrade = Dictionary.WhyUpgrade,
                    WhyUpgradeDescription = Dictionary.why_upgrade,
                    UpgradePromptTitle = Dictionary.WhyUpgrade,
                    UpgradePromptDescription = Dictionary.why_upgrade,
                    BenefitsTitle = Dictionary.WhatDoIGet,
                    BenefitsIntroductionHtml = Dictionary.what_you_get,
                    ChoosePlan = Dictionary.ChooseYourPlan,
                    Benefits = new
                    {
                        PersonalChartReadings = Dictionary.PersonalChartReadings,
                        PersonalChartDescriptionHtml = Dictionary.PersonalChartDescription,
                        CompatibilityReadings = Dictionary.CompatibilityReadings,
                        CompatibilityDescriptionHtml = Dictionary.CompatibilityDescription,
                        YearlyAndMonthlyCycles = Dictionary.YearlyAndMonthlyCycles,
                        OtherCycles = Dictionary.OtherCyclesTitle,
                        OtherCyclesDescriptionHtml = Dictionary.OtherCyclesDescription,
                        GlobalKi = Dictionary.GlobalKiTitle,
                        GlobalKiDescriptionHtml = Dictionary.GlobalKiDescription,
                        Directionology = Dictionary.Directionology,
                        DirectionologyDescriptionHtml = Dictionary.DirectionologyDetails,
                        InteractiveCalendar = Dictionary.InteractiveCalendar,
                        InteractiveCalendarDescriptionHtml = Dictionary.InteractiveCalendarDescription,
                        ConfigurableCalculators = Dictionary.ConfigurationTitle,
                        ConfigurableCalculatorsDescriptionHtml = Dictionary.ConfigurationsDescription,
                        AstronomicalPrecision = Dictionary.AstronomicalPrecision,
                        AstronomicalPrecisionDescriptionHtml = Dictionary.AstronomicalPrecisionDetails,
                        Biorhythms = Dictionary.NineStarKiBiorhythmsLabel,
                        IChing = Dictionary.IChingHexagramGenerator,
                        Numerology = Dictionary.AccessToKarmicNumerology,
                        AiAstrologer = Dictionary.AccessToOurGPTMembershipItem,
                        KnowledgeBase = Dictionary.KnowledgeBase,
                        ComplimentaryConsultation = Dictionary.FreeConsultationOfferText
                    },
                    Actions = new
                    {
                        Select = Dictionary.Select,
                        Unlock = Dictionary.Unlock,
                        Upgrade = Dictionary.Upgrade,
                        Unavailable = Dictionary.Unavailable,
                        UpgradeMembership = Dictionary.UpgradeMembershipText
                    }
                },
                Biorhythms = new
                {
                    Title = Dictionary.BiorhythmsLabel,
                    Overview = Dictionary.BiorhythmsOverview,
                    Details = Dictionary.BiorhythmsIndepthAnalysis,
                    MonthlyPlanner = Dictionary.BiorhythmsMonthlyPlanner,
                    Physical = Dictionary.PhysicalBiorhythm,
                    Emotional = Dictionary.EmotionalBiorhythm,
                    Intellectual = Dictionary.IntellectualBiorhythm
                },
                Calendar = new { Labels = MobileCalculationMapper.CalendarLabels(), Ui = CalendarUi() },
                Account = new
                {
                    Fields = new
                    {
                        FirstName = BaseDictionary.FirstNameLabel,
                        LastName = BaseDictionary.LastNameLabel,
                        PhoneNumber = BaseDictionary.PhoneNumberLabel,
                        OptionalPhoneNumber = Dictionary.MobileAccountOptionalPhoneNumber
                    },
                    Actions = new
                    {
                        SaveChanges = Dictionary.MobileAccountSaveChanges,
                        DiscardChanges = Dictionary.MobileAccountDiscardChanges,
                        CheckSavedDetails = Dictionary.MobileAccountCheckSavedDetails,
                        TrySignOutAgain = Dictionary.MobileAccountTrySignOutAgain
                    },
                    MobileOnly = Dictionary.MobileAccountMobileOnly,
                    Validation = AccountValidation(),
                    Login = LoginUi(),
                    Title = BaseDictionary.MyAccountTitle,
                    PersonalDetails = Dictionary.PersonalDetails,
                    CalculationPreferences = Dictionary.CalculationPreferences,
                    Email = Dictionary.EmailAddressLabel,
                    UsernameOrEmail = Dictionary.UserNameOrEmailAddress,
                    Password = BaseDictionary.PasswordLabel,
                    AccountNumber = Dictionary.AccountNumberLabel,
                    Membership = Dictionary.MembershipLabel,
                    CurrentMembership = Dictionary.YourCurrentMembership,
                    ChangePassword = BaseDictionary.ChangePassword,
                    DeleteAccount = BaseDictionary.DeleteMyAccount,
                    SignInWithGoogle = Dictionary.SignInWithGoogle
                },
                KnowledgeBase = new { Title = Dictionary.KnowledgeBase, Ui = KnowledgeUi() }, Articles = new { Title = Dictionary.Articles }, AiAstrologer = new { Title = Dictionary.AIAstrologer }
            }
        };

        private static object KnowledgeUi() => new
        {
            Back = Dictionary.MobileUiKnowledgeBack,
            Loading = Dictionary.MobileUiKnowledgeLoading,
            Unavailable = Dictionary.MobileUiKnowledgeUnavailable
        };

        private static object ChartUi() => new
        {
            ExternalLinkError = Dictionary.MobileUiExternalLinkError,
            LoadingPersonalChart = Dictionary.MobileUiLoadingPersonalChart,
            ReadMore = Dictionary.MobileUiReadMore,
            ShowLess = Dictionary.MobileUiShowLess,
            BackToMenu = Dictionary.MobileUiBackToMenu,
            Close = Dictionary.MobileUiClose,
            MenuHint = Dictionary.MobileUiMenuHint,
            EnergyDetailsHint = Dictionary.MobileUiEnergyDetailsHint,
            ChartGuidance = Dictionary.MobileUiChartGuidance,
            ExtendedUnavailable = Dictionary.MobileUiExtendedUnavailable,
            AccessNotice = Dictionary.MobileUiAccessNotice
        };

        private static object IChingUi() => new
        {
            Unavailable = Dictionary.MobileUiIchingUnavailable
        };

        private static object Feedback() => new
        {
            ErrorTitle = Dictionary.MobileUiFeedbackErrorTitle,
            PreviewError = Dictionary.MobileUiFeedbackPreviewError
        };

        private static object DateTimeUi() => new
        {
            ChooseDate = Dictionary.MobileUiDateTimeChooseDate,
            ChooseTime = Dictionary.MobileUiDateTimeChooseTime,
            Done = Dictionary.MobileUiDateTimeDone,
            Cancel = Dictionary.Cancel,
            Clear = Dictionary.MobileUiDateTimeClear
        };

        private static object AccountValidation() => new
        {
            FirstName = Dictionary.MobileUiAccountValidationFirstName,
            LastName = Dictionary.MobileUiAccountValidationLastName,
            FullName = Dictionary.MobileUiAccountValidationFullName,
            EmailAddress = Dictionary.MobileUiAccountValidationEmailAddress,
            PhoneNumber = Dictionary.MobileUiAccountValidationPhoneNumber,
            DateFormat = Dictionary.MobileUiAccountValidationDateFormat,
            DateRange = Dictionary.MobileUiAccountValidationDateRange,
            TimeKnown = Dictionary.MobileUiAccountValidationTimeKnown,
            Time = Dictionary.MobileUiAccountValidationTime,
            Gender = Dictionary.MobileUiAccountValidationGender,
            BirthTimeZoneId = Dictionary.MobileUiAccountValidationBirthTimeZoneId,
            UserTimeZoneId = Dictionary.MobileUiAccountValidationUserTimeZoneId,
            CalculationMethod = Dictionary.MobileUiAccountValidationCalculationMethod,
            HousesDisplay = Dictionary.MobileUiAccountValidationHousesDisplay,
            SupportedMode = Dictionary.MobileUiAccountValidationSupportedMode,
            Hemisphere = Dictionary.MobileUiAccountValidationHemisphere
        };

        private static object YourChartUi() => new
        {
            Back = Dictionary.MobileUiMyChartBack
        };

        private static object CalendarUi() => new
        {
            Title = Dictionary.MobileUiCalendarTitle,
            Loading = Dictionary.MobileUiCalendarLoading,
            Previous = Dictionary.MobileUiCalendarPrevious,
            Next = Dictionary.MobileUiCalendarNext,
            Today = Dictionary.Today,
            Expand = Dictionary.MobileUiCalendarExpand,
            Copy = Dictionary.MobileUiCalendarCopy,
            Link = Dictionary.MobileUiCalendarLink,
            Copied = Dictionary.MobileUiCalendarCopied,
            CopyFailed = Dictionary.MobileUiCalendarCopyFailed,
            Disable = Dictionary.MobileUiCalendarDisable,
            Disabled = Dictionary.MobileUiCalendarDisabled,
            Retry = Dictionary.MobileUiCalendarRetry,
            RevokeTitle = Dictionary.MobileUiCalendarRevokeTitle,
            RevokePrompt = Dictionary.MobileUiCalendarRevokePrompt,
            Cancel = Dictionary.Cancel,
            Account = Dictionary.MobileUiCalendarAccount,
            SignIn = Dictionary.MobileUiCalendarSignIn,
            SignInRequired = Dictionary.MobileUiCalendarSignInRequired,
            MobileOnly = Dictionary.MobileUiCalendarMobileOnly,
            Unavailable = Dictionary.MobileUiCalendarUnavailable,
            Membership_required = Dictionary.MobileUiCalendarMembershipRequired,
            Calendar_details_required = Dictionary.MobileUiCalendarCalendarDetailsRequired,
            Request_timeout = Dictionary.MobileUiCalendarRequestTimeout,
            Network_error = Dictionary.MobileUiCalendarNetworkError,
            SubscriptionUnavailable = Dictionary.MobileUiCalendarSubscriptionUnavailable
        };

        private static object PredictionsUi() => new
        {
            HousesHeading = Dictionary.HousesYouCurrentlyOccupy,
            AdditionalUnavailable = Dictionary.MobileUiPredictionsAdditionalUnavailable,
            Loading = Dictionary.MobileUiPredictionsLoading,
            Retry = Dictionary.MobileUiCalendarRetry,
            SignIn = Dictionary.MobileUiCalendarSignIn,
            SignInRequired = Dictionary.MobileUiPredictionsSignInRequired,
            Account = Dictionary.MobileUiCalendarAccount,
            Unavailable = Dictionary.MobileUiPredictionsUnavailable,
            Birth_details_required = Dictionary.MobileUiPredictionsBirthDetailsRequired,
            Invalid_calculation_settings = Dictionary.MobileUiPredictionsInvalidCalculationSettings,
            Request_timeout = Dictionary.MobileUiCalendarRequestTimeout,
            Network_error = Dictionary.MobileUiCalendarNetworkError,
            MobileOnly = Dictionary.MobileUiPredictionsMobileOnly
        };

        private static object CalculatorUi() => new
        {
            MissingPreferences = Dictionary.MobileUiCalculatorMissingPreferences,
            Back = Dictionary.MobileUiCalculatorBack,
            Loading = Dictionary.MobileUiCalculatorLoading,
            Retry = Dictionary.MobileUiCalendarRetry,
            SignIn = Dictionary.MobileUiCalendarSignIn,
            SignInRequired = Dictionary.MobileUiCalculatorSignInRequired,
            MobileOnly = Dictionary.MobileUiCalculatorMobileOnly
        };

        private static object CalculatorValidation() => new
        {
            InvalidName = Dictionary.MobileUiCalculatorErrorsInvalidName,
            InvalidDate = Dictionary.MobileUiCalculatorErrorsInvalidDate,
            InvalidTime = Dictionary.MobileUiCalculatorErrorsInvalidTime,
            InvalidTimeKnown = Dictionary.MobileUiCalculatorErrorsInvalidTimeKnown,
            InvalidPreferences = Dictionary.MobileUiCalculatorErrorsInvalidPreferences,
            InvalidSelectedDate = Dictionary.MobileUiCalculatorErrorsInvalidSelectedDate,
            Membership_required = Dictionary.MobileUiCalculatorErrorsMembershipRequired,
            Authentication_required = Dictionary.MobileUiCalculatorErrorsAuthenticationRequired,
            Invalid_csrf_token = Dictionary.MobileUiCalculatorErrorsInvalidCsrfToken,
            Invalid_calculation_request = Dictionary.MobileUiCalculatorErrorsInvalidCalculationRequest,
            Request_timeout = Dictionary.MobileUiCalculatorErrorsRequestTimeout,
            Network_error = Dictionary.MobileUiCalendarNetworkError,
            Unavailable = Dictionary.MobileUiCalculatorErrorsUnavailable
        };

        private static object PredictionCalculatorUi() => new
        {
            SignInRequired = Dictionary.MobileUiPredictionCalculatorSignInRequired,
            Membership_required = Dictionary.MobileUiPredictionCalculatorMembershipRequired
        };

        private static object CompatibilityUi() => new
        {
            Title = Dictionary.CompatibilityTitle,
            HideSexualChemistry = Dictionary.DoNotDisplaySexualityLabel,
            SignInRequired = Dictionary.MobileUiCompatibilitySignInRequired,
            Membership_required = Dictionary.MobileUiCompatibilityMembershipRequired,
            Unavailable = Dictionary.MobileUiCompatibilityUnavailable
        };

        private static object NavigationUi() => new
        {
            Explore = Dictionary.MobileUiNavigationExplore,
            Chart = Dictionary.MyChart,
            Predictions = Dictionary.MobileUiNavigationPredictions,
            Account = Dictionary.MobileUiNavigationAccount
        };

        private static object ExploreUi() => new
        {
            Introduction = Dictionary.MobileUiExploreIntroduction,
            Readings = Dictionary.MobileUiExploreReadings,
            Ask = Dictionary.MobileUiExploreAsk,
            Learn = Dictionary.Learn,
            Unavailable = Dictionary.MobileUiExploreUnavailable
        };

        private static object LoginUi() => new
        {
            Username = Dictionary.UserNameOrEmailAddress,
            Password = BaseDictionary.PasswordLabel,
            Submit = Dictionary.SignIn,
            StaySignedIn = Dictionary.MobileUiLoginStaySignedIn
        };
    }
}
