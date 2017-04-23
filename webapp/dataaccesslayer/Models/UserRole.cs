
using System.ComponentModel.DataAnnotations;

namespace K9.DataAccess.Models
{
	public class UserRole : ObjectBase
	{
		[Required]
		public int UserId { get; set; }
		[Required]
		public int RoleId { get; set; }

		public virtual User User { get; set; }
		public virtual Role Role { get; set; }
	}
}
