
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccess.Models
{

	public enum Grade
	{
		A, B, C, D, F
	}

	public class Enrollment : ObjectBase
	{
		[ForeignKey("Course")]
		[UIHint("Course")]
		public int CourseId { get; set; }
		
		[ForeignKey("Student")]
		[UIHint("Student")]
		public int StudentId { get; set; }
		public Grade? Grade { get; set; }

		public virtual Course Course { get; set; }
		public virtual Student Student { get; set; }
	}
}
