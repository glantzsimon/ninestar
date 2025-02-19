using K9.Base.Globalisation;
using System.ComponentModel.DataAnnotations;

namespace K9.SharedLibrary.Models
{
    public class UploadedFile
	{
	    [Display(ResourceType = typeof(Dictionary), Name = Strings.Names.FileName)]
        public string FileName { get; set; }
		public bool IsDeleted { get; set; }
		public IAssetInfo AssetInfo { get; set; }
	}
}