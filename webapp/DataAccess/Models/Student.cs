
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using K9.Globalisation;

namespace K9.DataAccess.Models
{
	public class Student : ObjectBase
	{
		public string FirstMidName { get; set; }
		public string LastName { get; set; }
		[DataType(DataType.Date, ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidDate)]
		public DateTime EnrollmentDate { get; set; }

		public virtual ICollection<Enrollment> Enrollments { get; set; }

		public override void UpdateName()
		{
			Name = string.Format("{0} {1}", FirstMidName, LastName);
		}
	}
}
