using K9.Base.DataAccessLayer.Enums;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using K9.WebApplication.Packages;
using K9.WebApplication.Services;
using K9.WebApplication.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace K9.WebApplication.Controllers
{
    [RoutePrefix("api")]
    public partial class ApiController : BaseNineStarKiController
    {
        private readonly INineStarKiService _nineStarKiService;
        private readonly IIChingService _iChingService;
        private const string authRequestHeader = "Authorization";

        public ApiController(INineStarKiPackage nineStarKiPackage, INineStarKiService nineStarKiService, IIChingService iChingService)
            : base(nineStarKiPackage)
        {
            _nineStarKiService = nineStarKiService;
            _iChingService = iChingService;
        }

        [Route("personal-chart/get/{accountNumber}/" +
               "{dateOfBirth}/{gender}/{timeOfBirth}/{birthLocation}")]
        public JsonResult GetPersonalChart(string accountNumber, DateTime dateOfBirth, EGender gender, string timeOfBirth = "", string birthLocation = "")
        {
            return Validate(accountNumber, () =>
            {
                var model = new NineStarKiModel(new PersonModel
                {
                    DateOfBirth = dateOfBirth.Add(DateTimeHelper.ParseTime(timeOfBirth)),
                    Gender = gender,
                    BirthTimeZoneId = DateTimeHelper.ResolveTimeZone(birthLocation)
                })
                {
                    SelectedDate = DateTime.Today
                };

                var selectedDate = model.SelectedDate;
                model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false, selectedDate,
                    ECalculationMethod.Chinese);
                model.SelectedDate = selectedDate;

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        model.PersonModel,
                        GenerationEnergy = new NineStarKiEnergySummary(model.PersonalChartEnergies.Generation),
                        SolarEnergy = new NineStarKiEnergySummary(model.MainEnergy),
                        LunarEnergy = new NineStarKiEnergySummary(model.CharacterEnergy),
                        SocialExpressionEnergy = new NineStarKiEnergySummary(model.SurfaceEnergy),
                        DayStarEnergy = new NineStarKiEnergySummary(model.PersonalChartEnergies.Day),

                        model.Summary,
                        model.Overview,
                        IntellectualQualities = model.MainEnergy.IntellectualQualitiesSummary,
                        InterpersonalQualities = model.MainEnergy.InterpersonalQualitiesSummary,
                        EmotionalLandscape = model.MainEnergy.EmotionalLandscapeSummary,
                        Spirituality = model.MainEnergy.SpiritualitySummary,
                        Health = model.MainEnergy.HealthSummary,
                        model.MainEnergy.Illnesses,
                        Career = model.MainEnergy.CareerSummary,
                        Finances = model.MainEnergy.FinancesSummary,
                        model.MainEnergy.Occupations,
                        model.MainEnergyRelationshipsSummary,
                        model.StressResponseDetails,
                        model.StressResponseFromNatalHouseDetails,
                        model.AdultChildRelationsihpDescription,

                    }
                }, JsonRequestBehavior.AllowGet);
            });
        }

        [Route("predictions/get/{accountNumber}/" +
               "{dateOfBirth}/{gender}/{selectedDate}/{timeOfBirth}/{birthLocation}")]
        public JsonResult GetPredictions(string accountNumber, DateTime dateOfBirth, EGender gender, DateTime selectedDate, string timeOfBirth = "", string birthLocation = "")
        {
            return Validate(accountNumber, () =>
            {
                var personModel = new PersonModel
                {
                    DateOfBirth = dateOfBirth.Add(DateTimeHelper.ParseTime(timeOfBirth)),
                    Gender = gender,
                    BirthTimeZoneId = DateTimeHelper.ResolveTimeZone(birthLocation)
                };
                var model = new NineStarKiModel(personModel)
                {
                    SelectedDate = selectedDate
                };

                var displayFor = selectedDate.Date == DateTime.Today ? EDisplayDataForPeriod.Now : EDisplayDataForPeriod.SelectedDate;

                model = _nineStarKiService.CalculateNineStarKiProfile(model.PersonModel, false, false, selectedDate,
                    ECalculationMethod.Chinese, true, false, personModel.BirthTimeZoneId, EHousesDisplay.SolarHouse, false, false, displayFor);
                model.SelectedDate = selectedDate;

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        NineYearlyPrediction = new NineStarKiEnergyCycleSummary(model.PersonalHousesOccupiedEnergies.Generation),
                        YearlyPrediction = new NineStarKiEnergyCycleSummary(model.PersonalHousesOccupiedEnergies.Year),
                        MonthlyPrediction = new NineStarKiEnergyCycleSummary(model.PersonalHousesOccupiedEnergies.Month),
                        DailyPrediction = new NineStarKiEnergyCycleSummary(model.PersonalHousesOccupiedEnergies.Day),
                        LunarPercentageOfIllumination = model.MoonPhase.IlluminationDisplay,
                        model.MoonPhase.LunarDayDescription
                    }
                }, JsonRequestBehavior.AllowGet);
            });
        }

        [Route("compatibility/get/{accountNumber}/" +
               "{firstPersonName}/{firstPersonDateOfBirth}/{firstPersonGender}/" +
               "{secondPersonName}/{secondPersonDateOfBirth}/{secondPersonGender}/" +
               "{displaySexualChemistry}/{firstPersonTimeOfBirth}/{firstPersonBirthLocation}/{secondPersonTimeOfBirth}/{secondPersonBirthLocation}")]
        public JsonResult GetCompatibility(string accountNumber,
            string firstPersonName, DateTime firstPersonDateOfBirth, EGender firstPersonGender,
            string secondPersonName, DateTime secondPersonDateOfBirth, EGender secondPersonGender,
            bool displaySexualChemistry = false, string firstPersonTimeOfBirth = "", string firstPersonBirthLocation = "",
            string secondPersonTimeOfBirth = "", string secondPersonBirthLocation = "")
        {
            return Validate(accountNumber, () =>
            {
                var personModel1 = new PersonModel
                {
                    Name = firstPersonName,
                    DateOfBirth = firstPersonDateOfBirth.Add(DateTimeHelper.ParseTime(firstPersonTimeOfBirth)),
                    Gender = firstPersonGender,
                    BirthTimeZoneId = DateTimeHelper.ResolveTimeZone(firstPersonBirthLocation)
                };
                var personModel2 = new PersonModel
                {
                    Name = secondPersonName,
                    DateOfBirth = secondPersonDateOfBirth.Add(DateTimeHelper.ParseTime(secondPersonTimeOfBirth)),
                    Gender = secondPersonGender,
                    BirthTimeZoneId = DateTimeHelper.ResolveTimeZone(secondPersonBirthLocation)
                };

                var model = _nineStarKiService.CalculateCompatibility(personModel1, personModel2, !displaySexualChemistry, ECalculationMethod.Chinese);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        NineStarKiSummaryModel1 = new NineStarKiSummaryModel(model.NineStarKiModel1),
                        NineStarKiSummaryModel2 = new NineStarKiSummaryModel(model.NineStarKiModel2),
                        model.FundamentalEnergiesCompatibility,
                        SexualChemistryDetails = displaySexualChemistry ? model.SexualChemistryDetails : "",
                        model.CompatibilityDetails
                    }
                }, JsonRequestBehavior.AllowGet);
            });
        }

        [Route("knowledgebase/get/{accountNumber}")]
        [OutputCache(Duration = 2592000, VaryByParam = "accountNumber", Location = OutputCacheLocation.ServerAndClient)]
        public JsonResult GetKnowledgeBase(string accountNumber)
        {
            Response.Cache.SetCacheability(HttpCacheability.ServerAndPrivate);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(1));
            Response.Cache.SetValidUntilExpires(true);

            return Validate(accountNumber, () =>
            {
                var model = new NineStarKiSummaryKbViewModel(_nineStarKiService.GetNineStarKiSummaryViewModel());
                return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
            });
        }

        [Route("iching/get")]
        public JsonResult GetIChing()
        {
            return Validate(null, () =>
            {
                var model = new IChingViewModel(_iChingService.GenerateHexagram());
                return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult GetCompatibilityTest()
        {
            var personModel1 = new PersonModel
            {
                Name = "Simon Baby Kotik",
                DateOfBirth = new DateTime(1979, 06, 16),
                Gender = EGender.Male
            };
            var personModel2 = new PersonModel
            {
                Name = "Andrei Kotik",
                DateOfBirth = new DateTime(1984, 09, 07),
                Gender = EGender.Male
            };

            var model = _nineStarKiService.CalculateCompatibility(personModel1, personModel2, false);

            foreach (var propertyInfo in model.GetProperties())
            {
                if (propertyInfo.PropertyType == typeof(string) && propertyInfo.CanWrite)
                {
                    try
                    {
                        model.SetProperty(propertyInfo, string.Empty);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }

            return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("deploy/upload")]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            return Validate(null, () =>
            {
                try
                {
                    if (file == null || file.ContentLength == 0)
                    {
                        return Json(new { success = false, error = "No file uploaded" });
                    }

                    var allowedExtensions = new[] { ".dll", ".pdb", ".cshtml", ".css", ".png", ".jpg", ".jpeg" };
                    var extension = Path.GetExtension(file.FileName);
                    if (!allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                    {
                        My.Logger.Error("File type not allowed");
                        return Json(new { success = false, error = "File type not allowed" });
                    }

                    string destinationPath;

                    if (extension.Equals(".dll", StringComparison.OrdinalIgnoreCase) ||
                        extension.Equals(".pdb", StringComparison.OrdinalIgnoreCase))
                    {
                        // Send to /bin folder
                        destinationPath = Path.Combine(My.DefaultValuesConfiguration.VaultPath, "bin", Path.GetFileName(file.FileName));
                    }
                    else if (extension.Equals(".cshtml", StringComparison.OrdinalIgnoreCase))
                    {
                        // Preserve subfolder for views
                        var relativePath = file.FileName.Replace("/", "\\").TrimStart('\\');
                        destinationPath = Path.Combine(My.DefaultValuesConfiguration.VaultPath, "views", relativePath);
                    }
                    else if (extension.Equals(".css", StringComparison.OrdinalIgnoreCase))
                    {
                        // Preserve subfolder for less
                        var relativePath = file.FileName.Replace("/", "\\").TrimStart('\\');
                        destinationPath = Path.Combine(My.DefaultValuesConfiguration.VaultPath, "css", relativePath);
                    }
                    else if (extension.Equals(".png", StringComparison.OrdinalIgnoreCase) || 
                             extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
                             extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase))
                    {
                        // Preserve subfolder for images
                        var relativePath = file.FileName.Replace("/", "\\").TrimStart('\\');
                        destinationPath = Path.Combine(My.DefaultValuesConfiguration.VaultPath, "images", relativePath);
                    }
                    else
                    {
                        return Json(new { success = false, error = "Unsupported file type" });
                    }

                    My.Logger.Info($"Destination path: {destinationPath}");

                    // Create necessary directories if they do not exist
                    var directory = Path.GetDirectoryName(destinationPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                        My.Logger.Info($"Directory path created: {directory}");
                    }

                    // Save the file to the destination
                    file.SaveAs(destinationPath);
                    My.Logger.Info($"File saved to: {destinationPath}");

                    return Json(new { success = true, message = "File uploaded successfully" });
                }
                catch (Exception e)
                {
                    return Json(new { success = false, error = e.Message });  // Simplified for standard error message handling
                }
            });
        }

        [HttpPost]
        [Route("deploy/run-maintenance")]
        public JsonResult RunMaintenanceScript()
        {
            return Validate(null, () =>
            {
                try
                {
                    // Build the script path from configuration
                    var scriptPath = Path.Combine(My.DefaultValuesConfiguration.VaultPath, My.DefaultValuesConfiguration.VaultDeployScript);

                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell.exe",
                        Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };

                    var outputBuilder = new StringBuilder();
                    var errorBuilder = new StringBuilder();

                    using (var process = new Process())
                    {
                        process.StartInfo = processStartInfo;

                        // Set up asynchronous output event handlers.
                        process.OutputDataReceived += (sender, args) =>
                                {
                                    if (!string.IsNullOrEmpty(args.Data))
                                    {
                                        outputBuilder.AppendLine(args.Data);
                                    }
                                };

                        process.ErrorDataReceived += (sender, args) =>
                        {
                            if (!string.IsNullOrEmpty(args.Data))
                            {
                                errorBuilder.AppendLine(args.Data);
                            }
                        };

                        process.Start();

                        // Begin asynchronous reading.
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();

                        // Wait for the process to exit; use a timeout (e.g., 60 seconds)
                        const int timeoutMilliseconds = 60000;
                        if (!process.WaitForExit(timeoutMilliseconds))
                        {
                            // If the process doesn't exit in time, kill it and throw an error.
                            process.Kill();
                            throw new Exception("Process timed out.");
                        }

                        // Ensure asynchronous output events have flushed.
                        process.WaitForExit();

                        // If the process returned a non-zero exit code, throw an exception with the error output.
                        if (process.ExitCode != 0)
                        {
                            throw new Exception($"Error executing script: {errorBuilder.ToString()}");
                        }

                        // Optionally log or output the captured output.
                        Console.WriteLine(outputBuilder.ToString());
                    }

                    return Json(new { success = true, message = "Maintenance script executed successfully." });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, error = $"Failed to run maintenance script: {ex.Message}" });
                }
            });
        }

        private JsonResult Validate(string accountNumber, Func<JsonResult> method)
        {
            if (!IsValidApiKey(Request.Headers[authRequestHeader]))
            {
                return InvalidApiKeyResult();
            }

            if (accountNumber != null)
            {
                var membership = GetMembership(accountNumber);
                if (membership == null)
                {
                    return InvalidAccountNumberResult();
                }

                if (!IsValidMembership(membership))
                {
                    return MembershipRequiresUpgradeResult();
                }
            }

            return method.Invoke();
        }

        private JsonResult InvalidApiKeyResult()
        {
            return Json(new
            {
                success = false,
                error = "Invalid ApiKey",
                statusCode = 401
            }, JsonRequestBehavior.AllowGet);
        }

        private JsonResult InvalidAccountNumberResult()
        {
            return Json(new
            {
                success = false,
                error = "Invalid Account Number",
                statusCode = 404
            }, JsonRequestBehavior.AllowGet);
        }

        private JsonResult MembershipRequiresUpgradeResult()
        {
            return Json(new
            {
                success = false,
                error = "Membership Has Insufficient Permissions. Upgrade Required.",
                statusCode = 422
            }, JsonRequestBehavior.AllowGet);
        }

        private bool IsValidApiKey(string authHeader)
        {
            string apiKey = null;
            if (!string.IsNullOrEmpty(authHeader))
            {
                apiKey = authHeader.Substring("ApiKey".Length).Trim();
            }
            return apiKey != null && apiKey == My.ApiConfiguration.ApiKey;
        }

        private bool IsValidMembership(UserMembership membership)
        {
            return (membership.IsActive && membership.MembershipOption != null && membership.MembershipOption.IsUnlimited) ||
                   Roles.UserIsInRoles(membership.User?.Username, RoleNames.Administrators);
        }

        private UserMembership GetMembership(string accountNumber)
        {
            return My.MembershipService.GetActiveUserMembership(accountNumber);
        }
    }
}