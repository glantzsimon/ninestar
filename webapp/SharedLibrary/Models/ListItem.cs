
namespace K9.SharedLibrary.Models
{
	public class ListItem : IListItem
	{
		public virtual int Id { get; private set; }
		public virtual string Name { get; private set; }

		public ListItem(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}

	public class ListItem<T> : ListItem where T : IObjectBase
	{
		public ListItem(int id, string name) : base(id, name)
		{
		}
	}
}
