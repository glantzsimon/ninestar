using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace K9.DataAccessLayer.Models
{
    [AutoGenerateName]
    [Name(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Names.UserPreference, PluralName = Globalisation.Strings.Names.UserPreferences)]
    [DefaultPermissions(Role = RoleNames.DefaultUsers)]
    public class UserPreference : ObjectBase, IUserData
    {
        [UIHint("User")]
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }

        [LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Username")]
        public string UserName { get; set; }

        [Required]
        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.KeyLabel)]
        public string Key { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name = Globalisation.Strings.Labels.TotalPriceLabel)]
        public string Value { get; set; }

        [Required]
        public string ValueType { get; set; }

        public object GetValue()
        {
            var type = Type.GetType(ValueType);
            if (type == null)
                return Value;

            return Convert.ChangeType(Value, type, CultureInfo.InvariantCulture);
        }
    }
}
