using K9.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace K9.WebApplication.Models
{
    public class NineStarKiDirections
    {
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

        public string FiveYellowKillingDescription => TemplateParser.Parse(FiveYelloKilling.Description, new
        {
            Direction = FiveYelloKilling.Direction.ToString().ToProperCase()
        });

        public string DarkSwordKillingDescription => TemplateParser.Parse(DarkSwordKilling.Description, new
        {
            Direction = DarkSwordKilling.Direction.ToString().ToProperCase()
        });

        public string SelfLifeKillingDescription => TemplateParser.Parse(SelfLifeKilling.Description, new
        {
            Direction = SelfLifeKilling.Direction.ToString().ToProperCase()
        });

        public string TargetKillingDescription => TemplateParser.Parse(TargetKilling.Description, new
        {
            Direction = TargetKilling.Direction.ToString().ToProperCase()
        });

        public NineStarKiDirections(ENineStarKiDirection fiveYellowKilling, ENineStarKiDirection darkSwordKilling, ENineStarKiDirection selfLifeKilling, ENineStarKiDirection targetKilling)
        {
            FiveYelloKilling = new NineStarKiDirection(Dictionary.FiveYellowKilling, Dictionary.FiveYellowKillingDescription, fiveYellowKilling);
            DarkSwordKilling = new NineStarKiDirection(Dictionary.DarkSwordKilling, Dictionary.DarkSwordKillingDescription, darkSwordKilling);
            SelfLifeKilling = new NineStarKiDirection(Dictionary.SelfLifeKilling, Dictionary.SelfLifeKillingDescription, selfLifeKilling);
            TargetKilling = new NineStarKiDirection(Dictionary.TargetKilling, Dictionary.TargetKillingDescription, targetKilling);

            FavourableDirections = _Directions.Where(e => new List<ENineStarKiDirection>
            {
                fiveYellowKilling,
                darkSwordKilling,
                selfLifeKilling,
                targetKilling
            }.Contains(e.Value))
                .Select(e => e.Value)
                .ToList();
        }

        public List<ENineStarKiDirection> FavourableDirections { get; }
    }
}