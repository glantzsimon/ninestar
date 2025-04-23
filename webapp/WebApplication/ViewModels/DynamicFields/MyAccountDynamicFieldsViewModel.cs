using K9.DataAccessLayer.Models;
using K9.Globalisation;
using K9.WebApplication.Enums;

namespace K9.WebApplication.ViewModels
{
    public class MyAccountDynamicFieldsViewModel : DynamicFieldsViewModel<UserInfo>
    {
        public MyAccountDynamicFieldsViewModel(int? userInfoId) : base(userInfoId)
        {
            Label = Dictionary.AvatarImages;
            Mode = EDynamicFieldsMode.Simple;
        }
    }
}