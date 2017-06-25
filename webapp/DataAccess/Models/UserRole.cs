
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9.DataAccess.Attributes;

namespace K9.DataAccess.Models
{
	[AutoGenerateName]
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
