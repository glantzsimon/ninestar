

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9.DataAccess.Attributes;
using K9.Globalisation;
using K9.SharedLibrary.Models;

namespace K9.DataAccess.Models
{
	public class Permission : ObjectBase, IPermission
	{
		[ForeignKey("Role")]
		[UIHint("Role")]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.RoleLabel)]
		public int RoleId { get; set; }

		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PermissionLabel)]
		public string Description
		{
			get
			{
				return GetLocalisedDescription();
			}
		}

		public virtual Role Role { get; set; }

		[LinkedColumn(LinkedTableName = "Role", LinkedColumnName = "Name")]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.RoleLabel)]
		public string RoleName { get; set; }
	}
}
