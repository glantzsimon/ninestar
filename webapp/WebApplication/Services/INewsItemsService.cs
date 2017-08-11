
using System.Web;

namespace K9.WebApplication.Services
{
	public interface INewsItemsService
	{
		string SaveNewsItemImageToDisk(HttpPostedFileBase imageFile);
	}
}