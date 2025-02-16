using System.ComponentModel.DataAnnotations;
using System.Web.Script.Serialization;
using K9.Globalisation;
using Org.BouncyCastle.Asn1.Cms;

namespace K9.WebApplication.Models
{
    public class NineStarKiEnergySummary
    {
        private readonly NineStarKiEnergy _nineStarKiEnergy;

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

        [ScriptIgnore]
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ElementLabel)]
        public string Element => _nineStarKiEnergy.ElementName;
        
        [Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.ModalityLabel)]
        public string Modality => _nineStarKiEnergy.ModalityName;
    }
}