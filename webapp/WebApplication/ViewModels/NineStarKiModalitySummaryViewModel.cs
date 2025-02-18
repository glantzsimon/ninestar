using K9.WebApplication.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiModalitySummaryViewModel
    {
        public string Body => ModalityEnergies.FirstOrDefault()?.ModalityDescription;
        public string Title => $"{Modality} {Globalisation.Dictionary.ModalityLabel}";

        [ScriptIgnore]
        public ENineStarKiModality Modality { get; set; }

        [ScriptIgnore]
        public List<NineStarKiEnergy> ModalityEnergies { get; set; }

        [ScriptIgnore]
        public string ModalityName => Modality.ToString();

        public NineStarKiModalitySummaryViewModel(ENineStarKiModality modality, List<NineStarKiEnergy> energies)
        {
            Modality = modality;
            ModalityEnergies = energies;
        }
    }
}