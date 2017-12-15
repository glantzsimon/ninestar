using System;
using K9.Base.DataAccessLayer.Config;
using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.EventArgs;
using K9.Base.WebApplication.Filters;
using K9.Base.WebApplication.UnitsOfWork;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using System.Web.Mvc;
using WebMatrix.WebData;

namespace K9.WebApplication.Controllers
{
    [Authorize]
    [RequirePermissions(Role = RoleNames.Administrators)]
    public class UsersController : BaseController<User>
    {
        private readonly IOptions<DatabaseConfiguration> _dataConfig;
        private readonly IRoles _roles;

        public UsersController(IControllerPackage<User> controllerPackage, IOptions<DatabaseConfiguration> dataConfig, IRoles roles)
            : base(controllerPackage)
        {
            _dataConfig = dataConfig;
            _roles = roles;
            RecordCreated += UsersController_RecordCreated;
            RecordBeforeDeleted += UsersController_RecordBeforeDeleted;
        }

        private void UsersController_RecordBeforeDeleted(object sender, CrudEventArgs e)
        {
            var user = e.Item as User;
            try
            {
                user.SetToDeleted();
                Repository.Update(user);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
        
        void UsersController_RecordCreated(object sender, CrudEventArgs e)
        {
            var user = e.Item as User;
            WebSecurity.CreateAccount(user.Username, _dataConfig.Value.DefaultUserPassword);
            _roles.AddUserToRole(user.Username, RoleNames.DefaultUsers);
        }
        
    }
}
