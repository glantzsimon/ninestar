namespace K9.SharedLibrary.Models
{
	public class UploadedFile
	{
		public string FileName { get; set; }
		public bool IsDeleted { get; set; }
		public AssetInfo AssetInfo { get; set; }
	}
}