
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9.DataAccess.Attributes;
using K9.Globalisation;
using K9.SharedLibrary.Attributes;

namespace K9.DataAccess.Models
{

	public enum EMessageDirection
	{
		Received,
		Sent
	}

	[AutoGenerateName]
	[Grammar(ResourceType = typeof(Dictionary), DefiniteArticleName = Strings.Grammar.MasculineDefiniteArticle, IndefiniteArticleName = Strings.Grammar.MasculineIndefiniteArticle)]
	[Name(ResourceType = typeof(Dictionary), Name = Strings.Names.Message)]
	public class Message : ObjectBase
	{
		[ForeignKey("SentByUser")]
		[UIHint("User")]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SentByUserLabel)]
		public int SentByUserId { get; set; }

		[ForeignKey("SentToUser")]
		[UIHint("User")]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SentToUserLabel)]
		public int SentToUserId { get; set; }

		public EMessageDirection MessageDirection { get; set; }

		[StringLength(256)]
		public string Subject { get; set; }

		[StringLength(Int32.MaxValue)]
		public string Body { get; set; }

		public virtual User SentByUser { get; set; }
		public virtual User SentToUser { get; set; }

		[LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Name")]
		public string SentToUserName { get; set; }

		[LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Name")]
		public string SentByUserName { get; set; }

	}
}
