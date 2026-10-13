using K9.Globalisation;
using System.ComponentModel.DataAnnotations;

namespace K9.WebApplication.Models
{
    public class NineStarKiEnergySummary
    {
        internal NineStarKiEnergySummary CreateRequestCopy(CalculationCopyContext context) =>
            context.Copy(this, source => (NineStarKiEnergySummary)source.MemberwiseClone(), (source, copy) =>
            {
                copy._nineStarKiEnergy = source._nineStarKiEnergy?.CreateRequestCopy(context);
            });

        private NineStarKiEnergy _nineStarKiEnergy;

        public NineStarKiEnergySummary(NineStarKiEnergy nineStarKiEnergy)
        {
            _nineStarKiEnergy = nineStarKiEnergy;
        }

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EnergyLabel)]
        public string Energy => _nineStarKiEnergy.EnergyName;

        public int EnergyNumber => _nineStarKiEnergy.EnergyNumber;

        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EnergyDescriptionLabel)]
        public string EnergyDescription => _nineStarKiEnergy.EnergyDescription;
        
        public string YinYang => _nineStarKiEnergy.YinYangName;
        
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ElementLabel)]
        public string Element => _nineStarKiEnergy.ElementName;
        
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ModalityLabel)]
        public string Modality => _nineStarKiEnergy.ModalityName;
    }
}