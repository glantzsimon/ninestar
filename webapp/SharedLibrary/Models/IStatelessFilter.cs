using System.Web.Routing;

namespace K9.SharedLibrary.Models
{
	public interface IStatelessFilter
	{
		string Key { get; }
		int Id { get; }
		bool IsSet();
		RouteValueDictionary GetFilterRouteValues();
	}
}
