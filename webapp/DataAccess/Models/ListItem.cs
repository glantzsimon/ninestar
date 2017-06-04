
using K9.SharedLibrary.Models;

namespace K9.DataAccess.Models
{
	public class ListItem : IListItem
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
