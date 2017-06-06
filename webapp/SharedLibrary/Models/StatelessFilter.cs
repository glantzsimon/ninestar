

using System.Web.Routing;

namespace K9.SharedLibrary.Models
{
	public class StatelessFilter : IStatelessFilter
	{
		public string Key { get; private set; }
		public int Id { get; private set; }

		public StatelessFilter(string key, int id)
		{
			Key = key;
			Id = id;
		}

		public RouteValueDictionary GetFilterRouteValues()
		{
			if (!string.IsNullOrEmpty(Key))
			{
				return new RouteValueDictionary
				{
					{Constants.Constants.Key, Key},
					{Constants.Constants.Value, Id},
				};
			}
			return new RouteValueDictionary();
		}
	}
}
