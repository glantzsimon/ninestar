

namespace K9.SharedLibrary.Models
{
	public class ForeignKeyFilter : IForeignKeyFilter
	{
		public string Key { get; private set; }
		public int Id { get; private set; }

		public ForeignKeyFilter(string key, int id)
		{
			Key = key;
			Id = id;
		}
	}
}
