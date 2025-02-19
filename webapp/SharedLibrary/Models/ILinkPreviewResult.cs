
namespace K9.SharedLibrary.Models
{
    public interface ILinkPreviewResult 
    {
        string Url { get; }
        string Title { get; }
        string Description { get; }
        string ImageUrl { get; }
    }
}
