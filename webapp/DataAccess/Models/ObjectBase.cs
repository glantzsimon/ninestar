
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Routing;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using WebMatrix.WebData;

namespace K9.DataAccess.Models
{
	public abstract class ObjectBase : IObjectBase, IValidatableObject
	{

		#region Properties

		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Index(IsUnique = true)]
		[StringLength(128)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NameLabel)]
		public string Name { get; set; }

		public string ForeignKeyName
		{
			get { return string.Format("{0}Id", GetType().Name); }
		}

		#endregion


		#region Audit Fields

		[StringLength(255)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CreatedByLabel)]
		public string CreatedBy { get; set; }

		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.CreatedOnLabel)]
		public DateTime? CreatedOn { get; set; }

		[StringLength(255)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.LastUpdatedByLabel)]
		public string LastUpdatedBy { get; set; }

		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.LastUpdatedOnLabel)]
		public DateTime? LastUpdatedOn { get; set; }

		#endregion


		#region Methods

		public void UpdateAuditFields()
		{
			var loggedinUser = "";

			try
			{
				loggedinUser = WebSecurity.CurrentUserName;
			}
			catch (Exception)
			{
				loggedinUser = SystemUser.System;
			}

			CreatedBy = loggedinUser;
			CreatedOn = DateTime.Now;
			LastUpdatedBy = loggedinUser;
			LastUpdatedOn = DateTime.Now;
		}

		public virtual void UpdateName() { }

		public RouteValueDictionary GetForeignKeyFilterRouteValues()
		{
			return new StatelessFilter(ForeignKeyName, Id).GetFilterRouteValues();
		}

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			UpdateName();
			if (string.IsNullOrEmpty(Name))
			{
				yield return new ValidationResult(Dictionary.FieldIsRequired, new[] { "Name" });
			}
		}

		#endregion

	}
}
