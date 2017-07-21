
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9.DataAccess.Attributes;
using K9.Globalisation;

namespace K9.DataAccess.Models
{
	[AutoGenerateName]
	[Grammar(ResourceType = typeof(Dictionary), DefiniteArticleName = Strings.Grammar.MasculineDefiniteArticle, IndefiniteArticleName = Strings.Grammar.MasculineIndefiniteArticle)]
	[Name(ResourceType = typeof(Dictionary), Name = Strings.Names.UserRoles, PluralName = Strings.Names.UserRoles)]
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

		[LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Username")]
		public string UserName { get; set; }

		[LinkedColumn(LinkedTableName = "Role", LinkedColumnName = "Description")]
		public string RoleName { get; set; }
	}
}
