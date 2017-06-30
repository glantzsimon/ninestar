

using System.ComponentModel.DataAnnotations;
using K9.DataAccess.Attributes;
using K9.Globalisation;
using K9.SharedLibrary.Models;

namespace K9.DataAccess.Models
{
	[Grammar(ResourceType = typeof(Dictionary), DefiniteArticleName = Strings.Grammar.MasculineDefiniteArticle, IndefiniteArticleName = Strings.Grammar.MasculineIndefiniteArticle)]
	[Name(ResourceType = typeof(Dictionary), Name = Strings.Names.Role)]
	
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

	}
}
