
namespace K9.SharedLibrary.Models
{
    public class ListItem : IListItem
    {
        public int Id { get; }
        public string Name { get; set; }
        public string Value { get; set; }

        public ListItem() { }

        public ListItem(int id, string name, string value = null)
        {
            Id = id;
            Name = name;
            Value = value;
        }
    }

    public class ListItem<T> : ListItem where T : IObjectBase
    {
        public ListItem() { }

        public ListItem(int id, string name) : base(id, name)
        {
        }

        public ListItem(int id, string name, string value) : base(id, name)
        {
            Value = value;
        }
    }
}
