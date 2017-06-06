

namespace K9.SharedLibrary.Models
{
	public class ForeignKeyFilter : IForeignKeyFilter
	{
		public string ForeignKeyName { get; private set; }
		public int ForeignKeyValue { get; private set; }

		public ForeignKeyFilter(string foreignKeyName, int foreignKeyValue)
		{
			ForeignKeyName = foreignKeyName;
			ForeignKeyValue = foreignKeyValue;
		}
	}
}
