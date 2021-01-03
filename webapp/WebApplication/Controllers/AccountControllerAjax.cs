using System.Web.Mvc;

namespace K9.WebApplication.Controllers
{
    public partial class AccountController 
	{
	    public JsonResult IsUserNameAvailable(string username)
	    {
	        return Json(_userRepository.Exists(u => u.Username == username), JsonRequestBehavior.AllowGet);
	    }

	    public JsonResult IsEmailAddressAvailable(string emailAddress)
	    {
	        return Json(_userRepository.Exists(u => u.EmailAddress == emailAddress), JsonRequestBehavior.AllowGet);
	    }
    }
}