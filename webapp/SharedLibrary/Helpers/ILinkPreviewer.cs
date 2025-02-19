using K9.SharedLibrary.Models;

namespace K9.SharedLibrary.Helpers
{
    public interface ILinkPreviewer
	{
	    ILinkPreviewResult GetPreview(string url, int imageMinimumWidth = 400, int imageMinimumHeight = 300);
	}
}
