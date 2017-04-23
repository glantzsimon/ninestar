
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Interfaces;
using WebMatrix.WebData;

namespace K9.DataAccess.Models
{
	public abstract class ObjectBase : IIdentity
	{

		#region Properties

		[Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[StringLength(128)]
		[Index(IsUnique = true)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NameLabel)]
		public string Name { get; set; }

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

		#endregion

	}
}
