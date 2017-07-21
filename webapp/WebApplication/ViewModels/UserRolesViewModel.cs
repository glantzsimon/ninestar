
using System.Collections.Generic;
using K9.DataAccess.Models;

namespace K9.WebApplication.ViewModels
{
	public class UserRolesViewModel
	{
		public User User { get; set; }
		public List<UserRole> UserRoles { get; set; }
	}
}