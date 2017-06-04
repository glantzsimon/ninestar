

namespace K9.SharedLibrary.Models
{
	public class ForeignKeyFilter : IForeignKeyFilter
	{
		public string ForeignKeyName { get; private set; }
		public int ForeignKeyId { get; private set; }

		public ForeignKeyFilter(string foreignKeyName, int foreignKeyId)
		{
			ForeignKeyName = foreignKeyName;
			ForeignKeyId = foreignKeyId;
		}
	}
}
