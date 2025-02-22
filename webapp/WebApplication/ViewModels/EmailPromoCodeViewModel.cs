using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.DataAccessLayer.Models;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace K9.WebApplication.ViewModels
{
    public class EmailPromoCodeViewModel
    {
        public Promotion Promotion { get; set; }
        
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidEmailAddress)]
        [EmailAddress(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.InvalidEmailAddress)]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EmailAddressLabel)]
        [StringLength(255)]
        public string EmailAddress { get; set; }

        [Display(ResourceType = typeof(Globalisation.Dictionary), Name =Globalisation.Strings.Labels.CustomerLabel)]
        [StringLength(128)]
        public string Name { get; set; }

        public string FirstName => Name?.Split(' ').FirstOrDefault();

        [UIHint("User")]
        [Display(ResourceType = typeof(Dictionary),
            Name = Strings.Names.User)]
        public int? UserId { get; set; }

        public User User { get; set; }
    }
}