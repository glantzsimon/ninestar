
using System;
using System.Collections.Generic;

namespace K9.DataAccess.Models
{
	public class Student : ObjectBase
	{
		public string LastName { get; set; }
		public string FirstMidName { get; set; }
		public DateTime EnrollmentDate { get; set; }

		public virtual ICollection<Enrollment> Enrollments { get; set; }
	}
}
