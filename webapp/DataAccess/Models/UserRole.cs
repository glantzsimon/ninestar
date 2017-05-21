
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccess.Models
{
	public class UserRole : ObjectBase
	{
		[Required]
		[ForeignKey("User")]
		public int UserId { get; set; }
		
		[Required]
		[ForeignKey("Role")]
		public int RoleId { get; set; }

		public virtual User User { get; set; }
		public virtual Role Role { get; set; }
	}
}
