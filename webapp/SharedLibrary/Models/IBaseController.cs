
namespace K9.SharedLibrary.Models
{
	public interface IBaseController
	{
		IDataSetsHelper DropdownDataSets { get; }
		IRoles Roles { get; }
	    IAuthentication Authentication { get; }
		string GetObjectName();
	}
}
