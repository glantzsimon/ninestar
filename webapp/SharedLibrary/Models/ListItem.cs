
namespace K9.SharedLibrary.Models
{
	public class ListItem : IListItem
	{
		public virtual int Id { get; private set; }
		public virtual string Name { get; private set; }
	}

	public class ListItem<T> : ListItem where T : IObjectBase
	{
		private readonly int _id;
		private readonly string _name;

		public override int Id
		{
			get
			{
				return _id;
			}
		}

		public override string Name
		{
			get { return _name; }
		}

		protected ListItem(int id, string name)
		{
			_id = id;
			_name = name;
		}
	}
}
