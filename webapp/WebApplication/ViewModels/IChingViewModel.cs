using K9.WebApplication.Models;

namespace K9.WebApplication.ViewModels
{
    public class IChingViewModel
    {
        public Hexagram Hexagram { get; }
        public Hexagram TransformedHexagram { get; }
        public HexagramInfo HexagramInfo => Hexagram?.GetHexagramInfo();
        public HexagramInfo TransformedHexagramInfo => TransformedHexagram?.GetHexagramInfo();

        public IChingViewModel(Hexagram hexagram)
        {
            Hexagram = hexagram;

            var transformedHexagram = Hexagram.Transform();

            if (Hexagram.Number != transformedHexagram.Number)
            {
                TransformedHexagram = transformedHexagram;
            }
        }
    }
}