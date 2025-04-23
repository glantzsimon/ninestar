using K9.DataAccessLayer.Models;

namespace K9.WebApplication.ViewModels
{
    public class ArticleDynamicFieldsViewModel : DynamicFieldsViewModel<Article>
    {
        public ArticleDynamicFieldsViewModel(int? articleId) : base(articleId)
        {
        }
    }
}