

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9.DataAccess.Attributes;
using K9.Globalisation;
using K9.SharedLibrary.Attributes;

namespace K9.DataAccess.Models
{
	[AutoGenerateName]
	public class RolePermission : ObjectBase
	{
		[ForeignKey("Role")]
		[UIHint("Role")]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.RoleLabel)]
		public int RoleId { get; set; }

		[ForeignKey("Permission")]
		[UIHint("Permission")]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PermissionLabel)]
		public int PermissionId { get; set; }

		public virtual Role Role { get; set; }

		public virtual Permission Permission { get; set; }

		[LinkedColumn(LinkedTableName = "Role", LinkedColumnName = "Name")]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.RoleLabel)]
		public string RoleName { get; set; }

		[LinkedColumn(LinkedTableName = "Permission", LinkedColumnName = "Name")]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PermissionLabel)]
		public string PermissionName { get; set; }
	}
}
