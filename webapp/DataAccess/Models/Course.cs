using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9.Base.DataAccessLayer.Models;

namespace K9.DataAccessLayer.Models
{
	public class Course : ObjectBase
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public new int Id { get; set; }
		public int Credits { get; set; }
		public virtual ICollection<Enrollment> Enrollments { get; set; }
	}
}

