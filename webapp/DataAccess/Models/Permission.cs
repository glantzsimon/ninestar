

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9.DataAccess.Attributes;

namespace K9.DataAccess.Models
{
	public class Permission : ObjectBase
	{
		[ForeignKey("Role")]
		[UIHint("Role")]
		[Display(Name = "Role")]
		public int RoleId { get; set; }

		public virtual Role Role { get; set; }

		[LinkedColumn(LinkedTableName = "Role", LinkedColumnName = "Name")]
		public string RoleName { get; set; }
	}
}
