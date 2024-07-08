using K9.WebApplication.Models;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiModalitySummaryViewModel
    {
        public ENineStarKiModality Modality { get; set; }
        public List<NineStarKiEnergy> ModalityEnergies { get; set; }
        public string Body => ModalityEnergies.FirstOrDefault()?.ModalityDescription;
        public string Title => $"{Modality} {Globalisation.Dictionary.ModalityLabel}";
        public string ModalityName => Modality.ToString();

        public NineStarKiModalitySummaryViewModel(ENineStarKiModality modality, List<NineStarKiEnergy> energies)
        {
            Modality = modality;
            ModalityEnergies = energies;
        }
    }
}