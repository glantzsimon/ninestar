

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using K9.Globalisation;
using K9.SharedLibrary.Models;

namespace K9.DataAccess.Models
{
	public class Role : ObjectBase, IRole
	{

		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.RoleLabel)]
		public string Description
		{
			get
			{
				return GetLocalisedDescription();
			}
		}

		public virtual ICollection<Permission> Permissions{ get; set; }

	}
}
