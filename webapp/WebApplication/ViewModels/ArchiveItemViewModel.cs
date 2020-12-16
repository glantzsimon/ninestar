using System.Linq;
using K9.Base.DataAccessLayer.Models;

namespace K9.WebApplication.ViewModels
{
    public class ArchiveItemViewModel
    {
        public Base.WebApplication.ViewModels.ArchiveViewModel ArchiveViewModel { get; set; }
        public int SelectedItemId { get; set; }

        public ArchiveItem SelectedArchiveItem =>
            ArchiveViewModel.SelectedArchive.Items.First(e => e.Id == SelectedItemId);
    }
}