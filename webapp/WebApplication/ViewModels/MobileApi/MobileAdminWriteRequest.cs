using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace K9.WebApplication.ViewModels.MobileApi
{
    public class MobileAdminWriteRequest
    {
        public int? Id { get; set; }
        [Required, AllowHtml]
        public Dictionary<string, object> Fields { get; set; }
    }
}
