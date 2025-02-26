using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class DharmaChakraCodeModel
    {
        public int Age { get; set; }
        
        public int Year { get; set; }

        public EChakraCode ChakraCode { get; set; }
        
        public EChakraCode DharmaChakraBaseCode { get; set; }
        
        public EChakraCode DharmaChakraCode { get; set; }
        
        public EChakraCode DharmaGroupChakraCode { get; set; }

        public int ChakraCodeNumber => (int)ChakraCode;
        
        public int DharmaChakraBaseCodeNumber => (int)DharmaChakraBaseCode;
        
        public int DharmaChakraCodeNumber => (int)DharmaChakraCode;
        
        public int DharmaGroupChakraCodeNumber => (int)DharmaGroupChakraCode;
    }
}