using K9.Base.DataAccessLayer.Attributes;
using K9.SharedLibrary.Extensions;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class NineStarKiDirection
    {
        public string Name { get; }
        public string Description { get; }
        public NineStarKiEnergy Energy { get; }
        public EUnfavourableDirection UnfavourableDirection { get; }
        public ENineStarKiDirection Direction => Energy.Direction;
        public int Score => GetScore();
        public string GetDirectionName() => Direction.GetAttribute<EnumDescriptionAttribute>().Name.ToProperCase();

        public NineStarKiDirection(EUnfavourableDirection unfavourableDirection, string description, NineStarKiEnergy energy)
        {
            UnfavourableDirection = unfavourableDirection;
            Name = unfavourableDirection.GetAttribute<EnumDescriptionAttribute>().Name;
            Description = description;
            Energy = energy;
        }

        private int? _score;
        private int GetScore()
        {
            if (!_score.HasValue)
            {
                switch (Energy.EnergyCycleType)
                {
                    case ENineStarKiEnergyCycleType.YearlyCycleEnergy:
                        _score = 5;
                        break;

                    case ENineStarKiEnergyCycleType.MonthlyCycleEnergy:
                        _score = 3;
                        break;

                    case ENineStarKiEnergyCycleType.DailyEnergy:
                        _score = 2;
                        break;

                    case ENineStarKiEnergyCycleType.HourlyEnergy:
                        _score = 1;
                        break;

                    default:
                        _score = 0;
                        break;
                }
            }

            return _score.Value;
        }
    }
}