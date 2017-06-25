
using System.Data.Entity;
using System.Linq;
using K9.DataAccess.Exceptions;
using K9.DataAccess.Models;
using K9.SharedLibrary.Models;

namespace K9.DataAccess.Helpers
{
	public class Users
	{
		private readonly DbContext _db;
		private readonly IRepository<User> _repository;

		public Users(DbContext db, IRepository<User> repository)
		{
			_db = db;
			_repository = repository;
		}

		public User GetUser(string username)
		{
			var user = _repository.GetQuery(string.Format("SELECT TOP 1 * FROM [User] WHERE [Username] = '{0}'", username)).FirstOrDefault();
			if (user == null)
			{
				throw new UserNotFoundException(username);
			}
			return user;
		}

	}
}
