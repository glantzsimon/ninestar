
namespace K9.SharedLibrary.Models
{
	public interface IPermissable
	{
		string CreatePermissionName { get; }
		string EditPermissionName { get; }
		string DeletePermissionName { get; }
		string ViewPermissionName { get; }

	}
}
