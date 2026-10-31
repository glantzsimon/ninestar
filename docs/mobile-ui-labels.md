# Mobile UI label catalogue

Public endpoint: `GET /api/mobile/v1/metadata/labels`. Response envelope remains `{success,data,error}`. Read before rendering the first calculator; no login, database query, profile calculation, credit consumption or membership is required.

`MobileUiLabels.Map` is an explicit, strongly typed allowlist. Do not enumerate Dictionary or expose its resource manager: Dictionary also contains paid descriptions, email templates and internal content. The catalogue includes website-backed headings plus the current mobile `uiCopy` inventory. It is not an automatic export of every resource, admin-only UI or all personalised reading headings. Future UI controls must be registered here with a Dictionary resource. Existing per-reading labels/help remain compatible.

## Organisation

- `data.version`: schema version 1 (integer).
- `data.culture`: `(Dictionary.Culture ?? CultureInfo.CurrentUICulture).Name`, the effective resource/UI culture of this request.
- `data.shared.actions`: common buttons/Yes/No.
- `data.shared.calculator`: `groups`, `fields`, `help`, `displayDataForPeriods`, `ui`, `validation`.
- `data.shared.dateTime`, `feedback`, `chartUi`, `navigation`: shared existing mobile controls/status wording.
- `data.navigation`: feature names, separate from page titles and About explanations.
- `data.screens`: `yourChart`, `explore`, `personalChart`, `predictions`, `compatibility`, `iching`, `karmicNumerology`, `biorhythms`, `calendar`, `account`, `knowledgeBase`, `articles`, `aiAstrologer`.
- Personal chart: `titles`, `groups`, `menu`, `facts`, `roles`. Prediction labels retain the existing `labels` structure and gain `titles`, `ui`, `calculatorUi`. Compatibility uses `labels` from the existing Compatibility label mapper and `ui` for form/status wording. Calendar uses `labels` from CalendarLabels and `ui` for the existing mobile messages. Account includes fixed field/actions, `fields` (firstName/lastName/phoneNumber/optionalPhoneNumber), `actions` (saveChanges/discardChanges/checkSavedDetails/trySignOutAgain), `mobileOnly`, `login` and `validation`. First/last/phone labels reuse the base Dictionary FirstNameLabel/LastNameLabel/PhoneNumberLabel; optional-phone and remaining Account actions preserve the current Account component wording in dedicated MobileAccount resources. Other `ui` blocks contain existing mobile interface wording, not authored readings.

The catalogue exposes all 173 leaf entries in the audited `ninestar-mobile/src/lib/ui-copy.ts` snapshot, with the migration table below identifying their destination. It also exposes the requested website-only calculator, menu/fact and feature labels. Website wording takes precedence for shared fields (including casing); remaining already-visible mobile wording was moved unchanged into `MobileUi...` Dictionary resources. No new promotional message or interface control was added; the localized existing membership action label is available to free calculator prompts. Mobile component wiring and removal of its local literals belong to ninestar-mobile, not this backend change.

## Culture, caching and missing values

The endpoint uses the existing request/thread UI culture (and existing Dictionary culture override, if set); .NET ResourceManager applies its normal culture/parent/neutral fallback. Neutral resources are English. This does not promise a complete translation of every screen. No new culture query parameter, Accept-Language negotiation or account-language write is introduced. Use the same existing server language/session configuration for readings and metadata, and treat the returned `culture` as authoritative. An invariant server/test culture may report an empty string.

The endpoint retains the mobile API's `no-store` response policy; it does not borrow the artwork catalogue's public cache exception. A native bootstrap/last-known catalogue can be stored separately by server/environment, culture and version. Never replace a working same-culture catalogue with a partial/failed response, mix environments or infer labels from enum keys. For offline/login failure UI, ship a snapshot of these approved Dictionary values and refresh from the API when available. Optional unavailable controls can be hidden; required controls should retain an approved bootstrap label or wait for metadata. Never title-case translated values, strip About, concatenate translated words or display protocol keys as labels.

## Calculator/date contract

- `shared.calculator.fields.currentTimeZone` → new `Dictionary.CurrentTimeZone` = Current Time Zone. This is deliberately distinct from the website's older `UserTimeZone` = User Time Zone.
- `shared.calculator.help.currentTimeZone` → new `Dictionary.CurrentTimeZoneHelp` = The timezone of your current location. It describes the selected timezone; no GPS/location detection or permission is implied.
- Groups Birth Details and Advanced Calculation Preferences are new Dictionary entries for the existing mobile headings.
- `fields.displayDataFor` → `DisplayDataForLabel`; `displayDataForPeriods` uses existing `EDisplayDataForPeriod` metadata: Now / 0 / Dictionary.Now and SelectedDate / 1 / Dictionary.SelectedDateAndTime.
- The same choices are added as `data.displayDataForPeriods` on `GET metadata/options` for existing metadata consumers.
- Now sends `selectedDateTime:null`. SelectedDate sends a local wall-clock timestamp interpreted in `userTimeZoneId`, preserving existing API request semantics. No new request enum/property or calculation mode is introduced. Mobile preserves both drafts when switching, but hides/ignores date controls and their validation for Now.
- The two inversion fields use the website's two distinct resources, even though their current English values both say Invert Daily Ki. Do not invent longer device-side labels. Displaying options never grants reading access.
- Graphical/Magic Square are presentation choices from the existing Prediction labels/options. Do not restore a Simple/Advanced selector merely because legacy metadata still supports CalculatorType.

## Additional display fields

`GET account/me` adds `data.membership.displayName`, resolved from `MembershipOption.SubscriptionTypeNameLocal` (FreeMembership for a free option); no membership yields the existing null membership object. It is private account data, not part of the public label catalogue. Membership/authentication rules and entitlement responses are unchanged.

I Ching result objects add `heading`, formatted with `Dictionary.HexagramHeadingFormat` = `Hexagram {0}: {1}` and the existing number/name. Use this server-formatted string; do not construct the prefix on device. Page/panel labels remain in `screens.iching`. The existing `iching/help` introduction stays separate, shown above the form and hidden on results. No cast or public/CSRF behaviour changes.

## Fixed field/resource mappings

All paths below are under `data`. `Base.` denotes `K9.Base.Globalisation.Dictionary`; other entries denote `K9.Globalisation.Dictionary`.

| Field | Dictionary resource |
| --- | --- |
| `shared.actions.calculate` | `Calculate` |
| `shared.actions.editCalculation` | `EditCalculation` |
| `shared.actions.save` | `Save` |
| `shared.actions.cancel` | `Cancel` |
| `shared.actions.back` | `Back` |
| `shared.actions.moreDetails` | `MoreDetails` |
| `shared.actions.viewFullText` | `FullTextView` |
| `shared.actions.viewSummary` | `SummaryView` |
| `shared.actions.generate` | `Generate` |
| `shared.actions.generateHexagram` | `GenerateHexagram` |
| `shared.actions.yes` | `Base.Yes` |
| `shared.actions.no` | `Base.No` |
| `shared.actions.signIn` | `SignIn` |
| `shared.actions.signOut` | `Base.LogOut` |
| `shared.actions.createAccount` | `CreateAccount` |
| `shared.actions.update` | `Base.Update` |
| `shared.actions.upgradeMembership` | `UpgradeMembershipText` |
| `shared.calculator.groups.birthDetails` | `BirthDetails` |
| `shared.calculator.groups.advancedPreferences` | `AdvancedCalculationPreferences` |
| `shared.calculator.groups.southernHemisphere` | `SouthernHemisphereOptions` |
| `shared.calculator.fields.name` | `Base.NameLabel` |
| `shared.calculator.fields.optionalName` | `OptionalNameLabel` |
| `shared.calculator.fields.dateOfBirth` | `DateOfBirthLabel` |
| `shared.calculator.fields.timeOfBirth` | `TimeOfBirthLabel` |
| `shared.calculator.fields.timeOfBirthKnown` | `TimeOfBirthKnownLabel` |
| `shared.calculator.fields.birthTimeZone` | `BirthTimeZone` |
| `shared.calculator.fields.currentTimeZone` | `CurrentTimeZone` |
| `shared.calculator.fields.gender` | `GenderLabel` |
| `shared.calculator.fields.calculationMethod` | `CalculationMethodLabel` |
| `shared.calculator.fields.housesDisplay` | `HousesDisplayLabel` |
| `shared.calculator.fields.displayDataFor` | `DisplayDataForLabel` |
| `shared.calculator.fields.selectedDate` | `SelectedDateLabel` |
| `shared.calculator.fields.selectedTime` | `SelectedTimeLabel` |
| `shared.calculator.fields.invertNatalKi` | `InvertDailyAndHourlyKiForSouthernHemisphereLabel` |
| `shared.calculator.fields.invertCycleKi` | `InvertDailyAndHourlyCycleKiForSouthernHemisphereLabel` |
| `shared.calculator.help.currentTimeZone` | `CurrentTimeZoneHelp` |
| `navigation.yourChart` | `YourChart` |
| `navigation.personalChart` | `PersonalChart` |
| `navigation.predictions` | `Predictions` |
| `navigation.compatibility` | `Compatibility` |
| `navigation.iching` | `IChing` |
| `navigation.karmicNumerology` | `KarmicNumerology` |
| `navigation.calendar` | `Calendar` |
| `navigation.biorhythms` | `BiorhythmsLabel` |
| `navigation.aiAstrologer` | `AIAstrologer` |
| `navigation.knowledgeBase` | `KnowledgeBase` |
| `navigation.articles` | `Articles` |
| `navigation.account` | `Base.MyAccountTitle` |
| `screens.personalChart.titles.page` | `YourPersonalChart` |
| `screens.personalChart.titles.calculator` | `PersonalChartCalculator` |
| `screens.personalChart.titles.hub` | `YourChart` |
| `screens.personalChart.groups.core` | `CoreEnergies` |
| `screens.personalChart.groups.extended` | `ExtendedProfile` |
| `screens.personalChart.groups.summary` | `SummaryLabel` |
| `screens.personalChart.menu.overview` | `OverviewLabel` |
| `screens.personalChart.menu.essence` | `Essence` |
| `screens.personalChart.menu.intellectualQualities` | `IntellectualQualities` |
| `screens.personalChart.menu.interpersonalQualities` | `InterpersonalQualities` |
| `screens.personalChart.menu.emotionalLandscape` | `EmotionalLandscape` |
| `screens.personalChart.menu.relationships` | `Relationships` |
| `screens.personalChart.menu.career` | `Career` |
| `screens.personalChart.menu.finances` | `Finances` |
| `screens.personalChart.menu.health` | `HealthLabel` |
| `screens.personalChart.menu.spirituality` | `Spirituality` |
| `screens.personalChart.menu.commonHealthChallenges` | `Illnesses` |
| `screens.personalChart.menu.childNature` | `ChildNature` |
| `screens.personalChart.menu.adultChildRelationship` | `AdultChildRelationshipTitle` |
| `screens.personalChart.menu.stressResponse` | `StressResponse` |
| `screens.personalChart.facts.energy` | `Energy` |
| `screens.personalChart.facts.yinYang` | `YinYangLabel` |
| `screens.personalChart.facts.element` | `Element` |
| `screens.personalChart.facts.group` | `ModalityLabel` |
| `screens.personalChart.facts.trigram` | `Trigram` |
| `screens.personalChart.facts.colour` | `Colour` |
| `screens.personalChart.facts.direction` | `Direction` |
| `screens.personalChart.facts.familyMember` | `FamilyMember` |
| `screens.personalChart.facts.season` | `Season` |
| `screens.personalChart.facts.internalRepresentation` | `InternalRepresentation` |
| `screens.personalChart.facts.physiognomy` | `Physiognomy` |
| `screens.personalChart.facts.primaryAttributes` | `PrimaryAttributes` |
| `screens.personalChart.facts.mostHarmoniousCombinations` | `MostHarmoniousCombinations` |
| `screens.personalChart.facts.mostChallengingCombinations` | `MostChallengingCombinations` |
| `screens.predictions.titles.page` | `YourPredictions` |
| `screens.predictions.titles.calculator` | `PredictionsCalculator` |
| `screens.iching.title` | `IChing` |
| `screens.iching.calculator` | `IChingHexagramGenerator` |
| `screens.iching.consult` | `ConsultTheIChing` |
| `screens.iching.generate` | `GenerateHexagram` |
| `screens.iching.generateShort` | `Generate` |
| `screens.iching.yourHexagram` | `YourHexagram` |
| `screens.iching.mainHexagram` | `MainHexagram` |
| `screens.iching.transformedHexagram` | `TransformedHexagram` |
| `screens.iching.detailedInterpretation` | `DetailedInterpretation` |
| `screens.iching.headingFormat` | `HexagramHeadingFormat` |
| `screens.karmicNumerology.title` | `KarmicNumerology` |
| `screens.karmicNumerology.calculator` | `KarmicNumerologyCalculator` |
| `screens.karmicNumerology.blueprint` | `MainKarmicBlueprint` |
| `screens.karmicNumerology.evolution` | `KarmicEvolution` |
| `screens.karmicNumerology.twelveYearOutlook` | `TwelveYearOutlook` |
| `screens.karmicNumerology.twelveMonthOutlook` | `TwelveMonthOutlook` |
| `screens.karmicNumerology.dailyInfluences` | `DailyInfluences` |
| `screens.karmicNumerology.dailyKarmicInfluences` | `DailyKarmicInfluences` |
| `screens.karmicNumerology.emergence` | `Emergence` |
| `screens.karmicNumerology.actualisation` | `Actualisation` |
| `screens.karmicNumerology.mastery` | `Mastery` |
| `screens.karmicNumerology.emergenceYears` | `EmergenceYears` |
| `screens.karmicNumerology.actualisationYears` | `ActualisationYears` |
| `screens.karmicNumerology.masteryYears` | `MasteryYears` |
| `screens.biorhythms.title` | `BiorhythmsLabel` |
| `screens.biorhythms.overview` | `BiorhythmsOverview` |
| `screens.biorhythms.details` | `BiorhythmsIndepthAnalysis` |
| `screens.biorhythms.monthlyPlanner` | `BiorhythmsMonthlyPlanner` |
| `screens.biorhythms.physical` | `PhysicalBiorhythm` |
| `screens.biorhythms.emotional` | `EmotionalBiorhythm` |
| `screens.biorhythms.intellectual` | `IntellectualBiorhythm` |
| `screens.account.title` | `Base.MyAccountTitle` |
| `screens.account.personalDetails` | `PersonalDetails` |
| `screens.account.calculationPreferences` | `CalculationPreferences` |
| `screens.account.email` | `EmailAddressLabel` |
| `screens.account.usernameOrEmail` | `UserNameOrEmailAddress` |
| `screens.account.password` | `Base.PasswordLabel` |
| `screens.account.accountNumber` | `AccountNumberLabel` |
| `screens.account.membership` | `MembershipLabel` |
| `screens.account.currentMembership` | `YourCurrentMembership` |
| `screens.account.changePassword` | `Base.ChangePassword` |
| `screens.account.deleteAccount` | `Base.DeleteMyAccount` |
| `screens.account.signInWithGoogle` | `SignInWithGoogle` |
| `screens.knowledgeBase.title` | `KnowledgeBase` |
| `screens.articles.title` | `Articles` |
| `screens.aiAstrologer.title` | `AIAstrologer` |
| `screens.yourChart.title` | `YourChart` |
| `screens.yourChart.personalChart` | `YourPersonalChart` |
| `screens.yourChart.predictions` | `YourPredictions` |
| `screens.yourChart.calendar` | `YourCalendar` |

`screens.personalChart.roles` reuses MobilePersonalChartMapper.RoleLabels; `screens.predictions.labels` reuses PredictionLabels; `screens.compatibility.labels` reuses CompatibilityLabels; `screens.calendar.labels` reuses CalendarLabels. Their exact existing mappings remain documented in mobile-api.md. They contain labels only, not readings or calculated scores. The Compatibility catalogue contains the complete label vocabulary; a result's privacy/access-controlled content still decides which sections to render.

## Membership options screen copy

`data.screens.membership` is an additive, Dictionary-backed block in the existing `GET /api/mobile/v1/metadata/labels` response. It includes membership page titles, the “Why upgrade?” and benefits introductions, each website benefit heading/description, plan-selection actions, and the existing localized `UpgradeMembershipText` action. Resource values and capitalization are passed through unchanged; fields ending in `Html` are the same resource-authored HTML used by the website membership page.

| API field | Dictionary resource |
| --- | --- |
| `screens.membership.title`, `optionsTitle`, `choosePlan` | `MembershipLabel`, `MembershipOptions`, `ChooseYourPlan` |
| `whyUpgrade`, `whyUpgradeDescription` | `WhyUpgrade`, `why_upgrade` |
| `benefitsTitle`, `benefitsIntroductionHtml` | `WhatDoIGet`, `what_you_get` |
| Benefit headings and descriptions | The matching resources used by `Views/Membership/Index.cshtml` (personal chart, compatibility, yearly/monthly and other cycles, Global Ki, Directionology, calendar, configurations, precision, Biorhythms, I Ching, Numerology, AI astrologer, Knowledge Base and consultation) |
| `screens.membership.actions.*` | `Select`, `Unlock`, `Upgrade`, `Unavailable`, `UpgradeMembershipText` |
| `shared.actions.upgradeMembership` | `UpgradeMembershipText` |

This is display copy only. It does not grant or imply access. The native app should show an upgrade action only beside a response field whose availability status is `requires_membership`, and should navigate to its own membership page. Native billing must obtain localized prices and billing periods from StoreKit or Google Play, not this copy or the legacy website/Stripe price fields.

## Existing mobile UI migration

Source: glantzsimon/ninestar-mobile `src/lib/ui-copy.ts` (blob `8ff158ad7c528055a4c828104c089eb9d2e8b508`), plus calculator-labels-handover.md, ui-label-audit.md and iching-api-handover.md. Left column is the previous local uiCopy path; middle column is the new API path under `data`. This table covers the current central inventory; additional scattered component literals should reuse these fields or be registered as explicit resources, never dynamically exported.

| Previous uiCopy path | Catalogue field | Dictionary resource |
| --- | --- | --- |
| `knowledge.back` | `screens.knowledgeBase.ui.back` | `MobileUiKnowledgeBack` |
| `knowledge.loading` | `screens.knowledgeBase.ui.loading` | `MobileUiKnowledgeLoading` |
| `knowledge.unavailable` | `screens.knowledgeBase.ui.unavailable` | `MobileUiKnowledgeUnavailable` |
| `externalLinkError` | `shared.chartUi.externalLinkError` | `MobileUiExternalLinkError` |
| `iching.generate` | `screens.iching.generate` | `GenerateHexagram` |
| `iching.unavailable` | `screens.iching.ui.unavailable` | `MobileUiIchingUnavailable` |
| `feedback.errorTitle` | `shared.feedback.errorTitle` | `MobileUiFeedbackErrorTitle` |
| `feedback.previewError` | `shared.feedback.previewError` | `MobileUiFeedbackPreviewError` |
| `dateTime.chooseDate` | `shared.dateTime.chooseDate` | `MobileUiDateTimeChooseDate` |
| `dateTime.chooseTime` | `shared.dateTime.chooseTime` | `MobileUiDateTimeChooseTime` |
| `dateTime.done` | `shared.dateTime.done` | `MobileUiDateTimeDone` |
| `dateTime.cancel` | `shared.dateTime.cancel` | `Cancel` |
| `dateTime.clear` | `shared.dateTime.clear` | `MobileUiDateTimeClear` |
| `accountValidation.firstName` | `screens.account.validation.firstName` | `MobileUiAccountValidationFirstName` |
| `accountValidation.lastName` | `screens.account.validation.lastName` | `MobileUiAccountValidationLastName` |
| `accountValidation.fullName` | `screens.account.validation.fullName` | `MobileUiAccountValidationFullName` |
| `accountValidation.emailAddress` | `screens.account.validation.emailAddress` | `MobileUiAccountValidationEmailAddress` |
| `accountValidation.phoneNumber` | `screens.account.validation.phoneNumber` | `MobileUiAccountValidationPhoneNumber` |
| `accountValidation.dateFormat` | `screens.account.validation.dateFormat` | `MobileUiAccountValidationDateFormat` |
| `accountValidation.dateRange` | `screens.account.validation.dateRange` | `MobileUiAccountValidationDateRange` |
| `accountValidation.timeKnown` | `screens.account.validation.timeKnown` | `MobileUiAccountValidationTimeKnown` |
| `accountValidation.time` | `screens.account.validation.time` | `MobileUiAccountValidationTime` |
| `accountValidation.gender` | `screens.account.validation.gender` | `MobileUiAccountValidationGender` |
| `accountValidation.birthTimeZoneId` | `screens.account.validation.birthTimeZoneId` | `MobileUiAccountValidationBirthTimeZoneId` |
| `accountValidation.userTimeZoneId` | `screens.account.validation.userTimeZoneId` | `MobileUiAccountValidationUserTimeZoneId` |
| `accountValidation.calculationMethod` | `screens.account.validation.calculationMethod` | `MobileUiAccountValidationCalculationMethod` |
| `accountValidation.housesDisplay` | `screens.account.validation.housesDisplay` | `MobileUiAccountValidationHousesDisplay` |
| `accountValidation.supportedMode` | `screens.account.validation.supportedMode` | `MobileUiAccountValidationSupportedMode` |
| `accountValidation.hemisphere` | `screens.account.validation.hemisphere` | `MobileUiAccountValidationHemisphere` |
| `myChart.title` | `screens.yourChart.title` | `YourChart` |
| `myChart.personalChart` | `screens.yourChart.personalChart` | `YourPersonalChart` |
| `myChart.calendar` | `screens.yourChart.calendar` | `YourCalendar` |
| `myChart.back` | `screens.yourChart.ui.back` | `MobileUiMyChartBack` |
| `calendar.title` | `screens.calendar.ui.title` | `MobileUiCalendarTitle` |
| `calendar.loading` | `screens.calendar.ui.loading` | `MobileUiCalendarLoading` |
| `calendar.previous` | `screens.calendar.ui.previous` | `MobileUiCalendarPrevious` |
| `calendar.next` | `screens.calendar.ui.next` | `MobileUiCalendarNext` |
| `calendar.today` | `screens.calendar.ui.today` | `Today` |
| `calendar.expand` | `screens.calendar.ui.expand` | `MobileUiCalendarExpand` |
| `calendar.copy` | `screens.calendar.ui.copy` | `MobileUiCalendarCopy` |
| `calendar.link` | `screens.calendar.ui.link` | `MobileUiCalendarLink` |
| `calendar.copied` | `screens.calendar.ui.copied` | `MobileUiCalendarCopied` |
| `calendar.copyFailed` | `screens.calendar.ui.copyFailed` | `MobileUiCalendarCopyFailed` |
| `calendar.disable` | `screens.calendar.ui.disable` | `MobileUiCalendarDisable` |
| `calendar.disabled` | `screens.calendar.ui.disabled` | `MobileUiCalendarDisabled` |
| `calendar.retry` | `screens.calendar.ui.retry` | `MobileUiCalendarRetry` |
| `calendar.revokeTitle` | `screens.calendar.ui.revokeTitle` | `MobileUiCalendarRevokeTitle` |
| `calendar.revokePrompt` | `screens.calendar.ui.revokePrompt` | `MobileUiCalendarRevokePrompt` |
| `calendar.cancel` | `screens.calendar.ui.cancel` | `Cancel` |
| `calendar.account` | `screens.calendar.ui.account` | `MobileUiCalendarAccount` |
| `calendar.signIn` | `screens.calendar.ui.signIn` | `MobileUiCalendarSignIn` |
| `calendar.signInRequired` | `screens.calendar.ui.signInRequired` | `MobileUiCalendarSignInRequired` |
| `calendar.mobileOnly` | `screens.calendar.ui.mobileOnly` | `MobileUiCalendarMobileOnly` |
| `calendar.unavailable` | `screens.calendar.ui.unavailable` | `MobileUiCalendarUnavailable` |
| `calendar.membership_required` | `screens.calendar.ui.membership_required` | `MobileUiCalendarMembershipRequired` |
| `calendar.calendar_details_required` | `screens.calendar.ui.calendar_details_required` | `MobileUiCalendarCalendarDetailsRequired` |
| `calendar.request_timeout` | `screens.calendar.ui.request_timeout` | `MobileUiCalendarRequestTimeout` |
| `calendar.network_error` | `screens.calendar.ui.network_error` | `MobileUiCalendarNetworkError` |
| `calendar.subscriptionUnavailable` | `screens.calendar.ui.subscriptionUnavailable` | `MobileUiCalendarSubscriptionUnavailable` |
| `loadingPersonalChart` | `shared.chartUi.loadingPersonalChart` | `MobileUiLoadingPersonalChart` |
| `predictions.title` | `navigation.predictions` | `Predictions` |
| `predictions.ownTitle` | `screens.predictions.titles.page` | `YourPredictions` |
| `predictions.housesHeading` | `screens.predictions.ui.housesHeading` | `HousesYouCurrentlyOccupy` |
| `predictions.additionalUnavailable` | `screens.predictions.ui.additionalUnavailable` | `MobileUiPredictionsAdditionalUnavailable` |
| `predictions.loading` | `screens.predictions.ui.loading` | `MobileUiPredictionsLoading` |
| `predictions.retry` | `screens.predictions.ui.retry` | `MobileUiCalendarRetry` |
| `predictions.signIn` | `screens.predictions.ui.signIn` | `MobileUiCalendarSignIn` |
| `predictions.signInRequired` | `screens.predictions.ui.signInRequired` | `MobileUiPredictionsSignInRequired` |
| `predictions.account` | `screens.predictions.ui.account` | `MobileUiCalendarAccount` |
| `predictions.unavailable` | `screens.predictions.ui.unavailable` | `MobileUiPredictionsUnavailable` |
| `predictions.birth_details_required` | `screens.predictions.ui.birth_details_required` | `MobileUiPredictionsBirthDetailsRequired` |
| `predictions.invalid_calculation_settings` | `screens.predictions.ui.invalid_calculation_settings` | `MobileUiPredictionsInvalidCalculationSettings` |
| `predictions.request_timeout` | `screens.predictions.ui.request_timeout` | `MobileUiCalendarRequestTimeout` |
| `predictions.network_error` | `screens.predictions.ui.network_error` | `MobileUiCalendarNetworkError` |
| `predictions.mobileOnly` | `screens.predictions.ui.mobileOnly` | `MobileUiPredictionsMobileOnly` |
| `calculator.title` | `screens.personalChart.titles.calculator` | `PersonalChartCalculator` |
| `calculator.birthDetails` | `shared.calculator.groups.birthDetails` | `BirthDetails` |
| `calculator.preferences` | `shared.calculator.groups.advancedPreferences` | `AdvancedCalculationPreferences` |
| `calculator.name` | `shared.calculator.fields.optionalName` | `OptionalNameLabel` |
| `calculator.missingPreferences` | `shared.calculator.ui.missingPreferences` | `MobileUiCalculatorMissingPreferences` |
| `calculator.dateOfBirth` | `shared.calculator.fields.dateOfBirth` | `DateOfBirthLabel` |
| `calculator.timeKnown` | `shared.calculator.fields.timeOfBirthKnown` | `TimeOfBirthKnownLabel` |
| `calculator.timeOfBirth` | `shared.calculator.fields.timeOfBirth` | `TimeOfBirthLabel` |
| `calculator.birthTimezone` | `shared.calculator.fields.birthTimeZone` | `BirthTimeZone` |
| `calculator.gender` | `shared.calculator.fields.gender` | `GenderLabel` |
| `calculator.currentTimezone` | `shared.calculator.fields.currentTimeZone` | `CurrentTimeZone` |
| `calculator.calculationMethod` | `shared.calculator.fields.calculationMethod` | `CalculationMethodLabel` |
| `calculator.housesDisplay` | `shared.calculator.fields.housesDisplay` | `HousesDisplayLabel` |
| `calculator.invertNatal` | `shared.calculator.fields.invertNatalKi` | `InvertDailyAndHourlyKiForSouthernHemisphereLabel` |
| `calculator.invertCycles` | `shared.calculator.fields.invertCycleKi` | `InvertDailyAndHourlyCycleKiForSouthernHemisphereLabel` |
| `calculator.selectedDate` | `shared.calculator.fields.selectedDate` | `SelectedDateLabel` |
| `calculator.selectedTime` | `shared.calculator.fields.selectedTime` | `SelectedTimeLabel` |
| `calculator.yes` | `shared.actions.yes` | `Base.Yes` |
| `calculator.no` | `shared.actions.no` | `Base.No` |
| `calculator.calculate` | `shared.actions.calculate` | `Calculate` |
| `calculator.edit` | `shared.actions.editCalculation` | `EditCalculation` |
| `calculator.back` | `shared.calculator.ui.back` | `MobileUiCalculatorBack` |
| `calculator.loading` | `shared.calculator.ui.loading` | `MobileUiCalculatorLoading` |
| `calculator.retry` | `shared.calculator.ui.retry` | `MobileUiCalendarRetry` |
| `calculator.signIn` | `shared.calculator.ui.signIn` | `MobileUiCalendarSignIn` |
| `calculator.signInRequired` | `shared.calculator.ui.signInRequired` | `MobileUiCalculatorSignInRequired` |
| `calculator.mobileOnly` | `shared.calculator.ui.mobileOnly` | `MobileUiCalculatorMobileOnly` |
| `calculator.errors.invalidName` | `shared.calculator.validation.invalidName` | `MobileUiCalculatorErrorsInvalidName` |
| `calculator.errors.invalidDate` | `shared.calculator.validation.invalidDate` | `MobileUiCalculatorErrorsInvalidDate` |
| `calculator.errors.invalidTime` | `shared.calculator.validation.invalidTime` | `MobileUiCalculatorErrorsInvalidTime` |
| `calculator.errors.invalidTimeKnown` | `shared.calculator.validation.invalidTimeKnown` | `MobileUiCalculatorErrorsInvalidTimeKnown` |
| `calculator.errors.invalidPreferences` | `shared.calculator.validation.invalidPreferences` | `MobileUiCalculatorErrorsInvalidPreferences` |
| `calculator.errors.invalidSelectedDate` | `shared.calculator.validation.invalidSelectedDate` | `MobileUiCalculatorErrorsInvalidSelectedDate` |
| `calculator.errors.membership_required` | `shared.calculator.validation.membership_required` | `MobileUiCalculatorErrorsMembershipRequired` |
| `calculator.errors.authentication_required` | `shared.calculator.validation.authentication_required` | `MobileUiCalculatorErrorsAuthenticationRequired` |
| `calculator.errors.invalid_csrf_token` | `shared.calculator.validation.invalid_csrf_token` | `MobileUiCalculatorErrorsInvalidCsrfToken` |
| `calculator.errors.invalid_calculation_request` | `shared.calculator.validation.invalid_calculation_request` | `MobileUiCalculatorErrorsInvalidCalculationRequest` |
| `calculator.errors.request_timeout` | `shared.calculator.validation.request_timeout` | `MobileUiCalculatorErrorsRequestTimeout` |
| `calculator.errors.network_error` | `shared.calculator.validation.network_error` | `MobileUiCalendarNetworkError` |
| `calculator.errors.unavailable` | `shared.calculator.validation.unavailable` | `MobileUiCalculatorErrorsUnavailable` |
| `predictionCalculator.title` | `screens.predictions.titles.calculator` | `PredictionsCalculator` |
| `predictionCalculator.signInRequired` | `screens.predictions.calculatorUi.signInRequired` | `MobileUiPredictionCalculatorSignInRequired` |
| `predictionCalculator.membership_required` | `screens.predictions.calculatorUi.membership_required` | `MobileUiPredictionCalculatorMembershipRequired` |
| `compatibility.title` | `screens.compatibility.ui.title` | `CompatibilityTitle` |
| `compatibility.firstPerson` | `screens.compatibility.labels.firstPerson` | `FirstPerson` |
| `compatibility.secondPerson` | `screens.compatibility.labels.secondPerson` | `SecondPerson` |
| `compatibility.hideSexualChemistry` | `screens.compatibility.ui.hideSexualChemistry` | `DoNotDisplaySexualityLabel` |
| `compatibility.signInRequired` | `screens.compatibility.ui.signInRequired` | `MobileUiCompatibilitySignInRequired` |
| `compatibility.membership_required` | `screens.compatibility.ui.membership_required` | `MobileUiCompatibilityMembershipRequired` |
| `compatibility.unavailable` | `screens.compatibility.ui.unavailable` | `MobileUiCompatibilityUnavailable` |
| `navigation.explore` | `shared.navigation.explore` | `MobileUiNavigationExplore` |
| `navigation.chart` | `shared.navigation.chart` | `MyChart` |
| `navigation.predictions` | `shared.navigation.predictions` | `MobileUiNavigationPredictions` |
| `navigation.account` | `shared.navigation.account` | `MobileUiNavigationAccount` |
| `explore.introduction` | `screens.explore.introduction` | `MobileUiExploreIntroduction` |
| `explore.readings` | `screens.explore.readings` | `MobileUiExploreReadings` |
| `explore.ask` | `screens.explore.ask` | `MobileUiExploreAsk` |
| `explore.learn` | `screens.explore.learn` | `Learn` |
| `explore.personalChart` | `navigation.personalChart` | `PersonalChart` |
| `explore.predictions` | `navigation.predictions` | `Predictions` |
| `explore.compatibility` | `navigation.compatibility` | `Compatibility` |
| `explore.iching` | `navigation.iching` | `IChing` |
| `explore.numerology` | `navigation.karmicNumerology` | `KarmicNumerology` |
| `explore.ai` | `navigation.aiAstrologer` | `AIAstrologer` |
| `explore.knowledge` | `navigation.knowledgeBase` | `KnowledgeBase` |
| `explore.articles` | `navigation.articles` | `Articles` |
| `explore.unavailable` | `screens.explore.unavailable` | `MobileUiExploreUnavailable` |
| `explore.predictionsTitle` | `screens.predictions.titles.page` | `YourPredictions` |
| `accountTitle` | `screens.account.title` | `Base.MyAccountTitle` |
| `personalChart` | `navigation.personalChart` | `PersonalChart` |
| `ownPersonalChart` | `screens.personalChart.titles.page` | `YourPersonalChart` |
| `login.username` | `screens.account.login.username` | `UserNameOrEmailAddress` |
| `login.password` | `screens.account.login.password` | `Base.PasswordLabel` |
| `login.submit` | `screens.account.login.submit` | `SignIn` |
| `login.staySignedIn` | `screens.account.login.staySignedIn` | `MobileUiLoginStaySignedIn` |
| `commonHealthChallenges` | `screens.personalChart.menu.commonHealthChallenges` | `Illnesses` |
| `readMore` | `shared.chartUi.readMore` | `MobileUiReadMore` |
| `showLess` | `shared.chartUi.showLess` | `MobileUiShowLess` |
| `backToMenu` | `shared.chartUi.backToMenu` | `MobileUiBackToMenu` |
| `close` | `shared.chartUi.close` | `MobileUiClose` |
| `menuHint` | `shared.chartUi.menuHint` | `MobileUiMenuHint` |
| `energyDetailsHint` | `shared.chartUi.energyDetailsHint` | `MobileUiEnergyDetailsHint` |
| `chartGuidance` | `shared.chartUi.chartGuidance` | `MobileUiChartGuidance` |
| `coreEnergies` | `screens.personalChart.groups.core` | `CoreEnergies` |
| `extendedProfile` | `screens.personalChart.groups.extended` | `ExtendedProfile` |
| `extendedUnavailable` | `shared.chartUi.extendedUnavailable` | `MobileUiExtendedUnavailable` |
| `summary` | `screens.personalChart.groups.summary` | `SummaryLabel` |
| `accessNotice` | `shared.chartUi.accessNotice` | `MobileUiAccessNotice` |
| `coreSections.energyFacts` | `screens.personalChart.menu.overview` | `OverviewLabel` |
| `coreSections.essence` | `screens.personalChart.menu.essence` | `Essence` |
| `coreSections.intellectualQualities` | `screens.personalChart.menu.intellectualQualities` | `IntellectualQualities` |
| `coreSections.interpersonalQualities` | `screens.personalChart.menu.interpersonalQualities` | `InterpersonalQualities` |
| `coreSections.emotionalLandscape` | `screens.personalChart.menu.emotionalLandscape` | `EmotionalLandscape` |
| `coreSections.relationships` | `screens.personalChart.menu.relationships` | `Relationships` |
| `coreSections.career` | `screens.personalChart.menu.career` | `Career` |
| `coreSections.finances` | `screens.personalChart.menu.finances` | `Finances` |
| `coreSections.health` | `screens.personalChart.menu.health` | `HealthLabel` |
| `coreSections.spirituality` | `screens.personalChart.menu.spirituality` | `Spirituality` |

## Verification/release

Regression coverage exercises anonymous access with a strict package mock, no-store, field/enum parity, distinction between labels and reading/help content, and nonempty strings. Source checks verify every main Dictionary accessor/resource and all 173 migration paths. Full Windows .NET Framework build/xUnit and authenticated Integration checks remain required; no deployment was performed here. The app still needs to consume this contract after backend deployment. No schema, purchasing, authentication, membership or private-cache changes are included.
