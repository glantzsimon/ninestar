
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9.Globalisation;

namespace K9.DataAccess.Models
{
	public class Student : ObjectBase
	{
		public string LastName { get; set; }
		public string FirstMidName { get; set; }
		public DateTime EnrollmentDate { get; set; }

		[Index(IsUnique = true)]
		[StringLength(128)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NameLabel)]
		public new string Name { get; set; }

		public virtual ICollection<Enrollment> Enrollments { get; set; }

		public override void UpdateName()
		{
			Name = string.Format("{0} {1}", FirstMidName, LastName);
		}
	}
}
