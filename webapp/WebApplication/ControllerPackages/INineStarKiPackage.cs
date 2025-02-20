using K9.WebApplication.Services;

namespace K9.WebApplication.Packages
{
    public interface INineStarKiPackage : INineStarKiBasePackage
    {
        IMembershipService MembershipService { get; set; }
        IAccountService AccountService { get; set; }
        IUserService UserService { get; set; }
        IContactService ContactService { get; set; }
    }
}