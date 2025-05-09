using K9.DataAccessLayer.Models;

namespace K9.WebApplication.ViewModels
{
    public class EmailTemplatesDynamicFieldsViewModel : DynamicFieldsViewModel<EmailTemplate>
    {
        public EmailTemplatesDynamicFieldsViewModel(int? emailTemplateId) : base(emailTemplateId)
        {
        }
    }
}