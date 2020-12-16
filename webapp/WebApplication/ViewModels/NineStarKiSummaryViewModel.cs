using System.Collections.Generic;
using K9.WebApplication.Models;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiSummaryViewModel
    {
        public List<NineStarKiModel> Energies { get; set; }

        public NineStarKiSummaryViewModel(List<NineStarKiModel> energies)
        {
            Energies = energies;
        }
    }
}