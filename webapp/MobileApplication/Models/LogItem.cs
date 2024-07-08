using System;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.Models
{
    public class LogItem
    {
        [Display(Name = "Logged On")]
        public DateTime LoggedOn { get; set; }

        [Display(Name = "Logged On")]
        public string ErrorLoggedOn => LoggedOn.ToString(Constants.FormatConstants.dataTableDateTimeFormat);

        [Display(Name = "Class")]
        public string ClassName { get; set; }

        [Display(Name = "Method")]
        public string MethodName { get; set; }

        [Display(Name = "Error Message")]
        public string ErrorMessage { get; set; }
    }
}