using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class DharmaNumerologyCodeModel
    {
        public int Age { get; set; }
        
        public int Year { get; set; }

        public ENumerologyCode NumerologyCode { get; set; }
        
        public ENumerologyCode DharmaNumerologyBaseCode { get; set; }
        
        public ENumerologyCode DharmaNumerologyCode { get; set; }
        
        public ENumerologyCode DharmaGroupNumerologyCode { get; set; }

        public int NumerologyCodeNumber => (int)NumerologyCode;
        
        public int DharmaChakraBaseCodeNumber => (int)DharmaNumerologyBaseCode;
        
        public int DharmaNumerologyCodeNumber => (int)DharmaNumerologyCode;
        
        public int DharmaGroupCodeNumber => (int)DharmaGroupNumerologyCode;
    }
}