using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.SharedLibrary.Attributes;

namespace K9.DataAccessLayer.Models
{

	public enum Grade
	{
		A, B, C, D, F
	}

	[AutoGenerateName]
	[Grammar(ResourceType = typeof(Dictionary), DefiniteArticleName = Strings.Grammar.DefiniteArticleWithApostrophe, IndefiniteArticleName = Strings.Grammar.FeminineIndefiniteArticle, OfPrepositionName = Strings.Grammar.OfPrepositionWithApostrophe)]
	[Name(ResourceType = typeof(Dictionary), Name = Strings.Names.Enrollment)]
	public class Enrollment : ObjectBase
	{
		[ForeignKey("Course")]
		[UIHint("Course")]
		[Display(Name = "Course")]
		public int CourseId { get; set; }

		[ForeignKey("Student")]
		[UIHint("Student")]
		[Display(Name = "Student")]
		public int StudentId { get; set; }

		public Grade? Grade { get; set; }

		public virtual Course Course { get; set; }
		public virtual Student Student { get; set; }

		[LinkedColumn(LinkedTableName = "Student", LinkedColumnName = "Name")]
		public string StudentName { get; set; }

		[LinkedColumn(LinkedTableName = "Course", LinkedColumnName = "Name")]
		public string CourseName { get; set; }
	}
}
