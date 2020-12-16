using K9.WebApplication.Models;
using System.Collections.Generic;

namespace K9.WebApplication.ViewModels
{
    public class NineStarKiModalitySummaryViewModel
    {
        public ENineStarKiModality Modality { get; set; }
        public List<NineStarKiEnergy> ModalityEnergies { get; set; }
        public string Body => GetBody();
        public string Title => $"{Modality} {Globalisation.Dictionary.ModalityLabel}";
        public string ModalityName => Modality.ToString();

        public NineStarKiModalitySummaryViewModel(ENineStarKiModality modality, List<NineStarKiEnergy> energies)
        {
            Modality = modality;
            ModalityEnergies = energies;
        }

        private string GetBody()
        {
            switch (Modality)
            {
                    case ENineStarKiModality.Dynamic:
                        return Globalisation.Dictionary.dynamic_modality;

                case ENineStarKiModality.Static:
                    return Globalisation.Dictionary.static_modality;

                case ENineStarKiModality.Flexible:
                    return Globalisation.Dictionary.flexible_modality;
            }

            return string.Empty;
        }
    }
}