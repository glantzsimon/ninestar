using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using System.Collections.Generic;
using System.Linq;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
    public class NineStarKiDirections
    {
        public NineStarKiEnergy PersonalChartEnergy { get; }

        private static readonly Dictionary<int, ENineStarKiDirection> _Directions = new Dictionary<int, ENineStarKiDirection>
        {
            { 1, ENineStarKiDirection.North },
            { 2, ENineStarKiDirection.SouthWest },
            { 3, ENineStarKiDirection.East },
            { 4, ENineStarKiDirection.SouthEast },
            { 5, ENineStarKiDirection.Centre },
            { 6, ENineStarKiDirection.NorthWest },
            { 7, ENineStarKiDirection.West },
            { 8, ENineStarKiDirection.NorthEast },
            { 9, ENineStarKiDirection.South },
        };

        public NineStarKiDirection FiveYelloKilling { get; }
        public NineStarKiDirection DarkSwordKilling { get; }
        public NineStarKiDirection SelfLifeKilling { get; }
        public NineStarKiDirection TargetKilling { get; }

        public string FiveYellowKillingDescription
        {
            get
            {
                var centreDescription = TemplateParser.Parse(Dictionary.InAnyDirection, new { Energy = 5 });
                return TemplateParser.Parse(FiveYelloKilling.Description, new
                {
                    Direction = FiveYelloKilling.Direction == ENineStarKiDirection.Centre
                        ? centreDescription
                        : FiveYelloKilling.Direction.ToString().ToProperCase(),
                    Direction2 = Dictionary.Somewhere
                });
            }
        }

        public string DarkSwordKillingDescription
        {
            get
            {
                var centreDescription = TemplateParser.Parse(Dictionary.InAnyDirection, new { Energy = 5 });
                return TemplateParser.Parse(DarkSwordKilling.Description, new
                {
                    Direction = DarkSwordKilling.Direction == ENineStarKiDirection.Centre
                    ? centreDescription
                    : DarkSwordKilling.Direction.ToString().ToProperCase(),
                    Direction2 = Dictionary.Somewhere
                });
            }
        }

        public string SelfLifeKillingDescription
        {
            get
            {
                var directionDescription = TemplateParser.Parse(Dictionary.InAnyDirection, new { Energy = PersonalChartEnergy?.EnergyNumber });
                return TemplateParser.Parse(SelfLifeKilling.Description, new
                {
                    Direction = SelfLifeKilling.Direction == ENineStarKiDirection.Centre
                        ? directionDescription
                        : SelfLifeKilling.Direction.ToString().ToProperCase(),
                    Direction2 = Dictionary.Somewhere
                });
            }
        }

        public string TargetKillingDescription
        {
            get
            {
                var directionDescription = TemplateParser.Parse(Dictionary.InAnyDirection, new { Energy = PersonalChartEnergy?.EnergyNumber });
                return TemplateParser.Parse(TargetKilling.Description, new
                {
                    Direction = TargetKilling.Direction == ENineStarKiDirection.Centre
                        ? directionDescription
                        : TargetKilling.Direction.ToString().ToProperCase(),
                    Direction2 = Dictionary.Somewhere
                });
            }
        }

        public NineStarKiDirections(NineStarKiEnergy fiveYellowKilling, NineStarKiEnergy darkSwordKilling, NineStarKiEnergy selfLifeKilling, NineStarKiEnergy targetKilling, NineStarKiEnergy personalChartEnergy)
        {
            PersonalChartEnergy = personalChartEnergy;
            FiveYelloKilling = new NineStarKiDirection(EUnfavourableDirection.FiveYelloKilling, Dictionary.FiveYellowKillingDescription, fiveYellowKilling);
            DarkSwordKilling = new NineStarKiDirection(EUnfavourableDirection.DarkSwordKilling, Dictionary.DarkSwordKillingDescription, darkSwordKilling);
            SelfLifeKilling = new NineStarKiDirection(EUnfavourableDirection.SelfLifeKilling, Dictionary.SelfLifeKillingDescription, selfLifeKilling);
            TargetKilling = new NineStarKiDirection(EUnfavourableDirection.TargetKilling, Dictionary.TargetKillingDescription, targetKilling);

            FavourableDirections = _Directions.Where(e => !new List<ENineStarKiDirection>
            {
                fiveYellowKilling.Direction,
                darkSwordKilling.Direction,
                selfLifeKilling.Direction,
                targetKilling.Direction
            }.Contains(e.Value))
                .Select(e => e.Value)
                .ToList();
        }

        public List<NineStarKiDirection> UnfavourableDirections => new List<NineStarKiDirection>
        {
            FiveYelloKilling,
            DarkSwordKilling,
            SelfLifeKilling,
            TargetKilling
        };

        public List<ENineStarKiDirection> FavourableDirections { get; }
    }
}