
using K9.SharedLibrary.Models;

namespace K9.DataAccess.Models
{
	public class ListItem<T> : IListItem where T : IObjectBase
	{
		public int Id { get; private set; }
		public string Name { get; private set; }

		protected ListItem(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}
}
