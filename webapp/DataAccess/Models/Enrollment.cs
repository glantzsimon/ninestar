
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace K9.DataAccess.Models
{

	public enum Grade
	{
		A, B, C, D, F
	}

	public class Enrollment : ObjectBase
	{
		public int CourseId { get; set; }
		public int StudentId { get; set; }
		public Grade? Grade { get; set; }

		[Index(IsUnique = false)]
		public new string Name
		{
			get { return Guid.NewGuid().ToString(); }
		}

		public virtual Course Course { get; set; }
		public virtual Student Student { get; set; }
	}
}
