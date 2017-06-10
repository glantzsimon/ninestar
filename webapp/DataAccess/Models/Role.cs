

using System.Collections.Generic;

namespace K9.DataAccess.Models
{
	public class Role : ObjectBase
	{

		public virtual ICollection<Permission> Permissions{ get; set; }

	}
}
