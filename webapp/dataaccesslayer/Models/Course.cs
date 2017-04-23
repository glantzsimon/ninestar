using System.Collections.Generic;

namespace K9.DataAccess.Models
{
	public class Course
	{
		public int ID { get; set; }
		public string Title { get; set; }
		public int Credits { get; set; }

		public virtual ICollection<Enrollment> Enrollments { get; set; }
	}
}

