
using System.Collections.Generic;
using K9.DataAccess.Models;
using K9.SharedLibrary.Models;

namespace K9.DataAccess.Extensions
{
	public static class RepositoryExtensions
	{

		#region UserRoles

		public static List<UserRole> GetByUser(this IRepository<UserRole> repository, int userId)
		{
			return repository.GetQuery(string.Format("SELECT * FROM [UserRole] WHERE [UserId] = {0}", userId));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="repository"></param>
		/// <param name="userId"></param>
		/// <returns></returns>
		public static List<UserRole> GetAllByUser(this IRepository<UserRole> repository, int userId)
		{
			return repository.GetQuery(string.Format("SELECT * FROM [UserRole] RIGHT JOIN [Role] ON [UserRole].[RoleId] = [Role].[Id] WHERE [UserRole].[UserId] = {0}", userId));
		}

		#endregion

	}
}
