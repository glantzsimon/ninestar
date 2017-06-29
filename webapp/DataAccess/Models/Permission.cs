

using System.ComponentModel.DataAnnotations;
using K9.Globalisation;
using K9.SharedLibrary.Models;

namespace K9.DataAccess.Models
{
	public class Permission : ObjectBase, IPermission
	{
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.PermissionLabel)]
		public string Description
		{
			get
			{
				return GetLocalisedDescription();
			}
		}
	}
}
