using K9.WebApplication.Enums;
using System.Collections.Generic;
using System.Text;

namespace K9.WebApplication.Models
{
    public class Hexagram
    {
        private static readonly Dictionary<int, string> HexagramNames = new Dictionary<int, string>
        {
            { 1, Globalisation.Dictionary.TheCreative },
            { 2, Globalisation.Dictionary.TheReceptive },
            { 3, Globalisation.Dictionary.DifficultyAtTheBeginning },
            { 4, Globalisation.Dictionary.YouthfulFolly },
            { 5, Globalisation.Dictionary.Waiting },
            { 6, Globalisation.Dictionary.Conflict },
            { 7, Globalisation.Dictionary.TheArmy },
            { 8, Globalisation.Dictionary.HoldingTogether },
            { 9, Globalisation.Dictionary.TamingPowerOfTheSmall },
            { 10, Globalisation.Dictionary.Treading },
            { 11, Globalisation.Dictionary.Peace },
            { 12, Globalisation.Dictionary.Standstill },
            { 13, Globalisation.Dictionary.FellowshipWithMen },
            { 14, Globalisation.Dictionary.PossessionInGreatMeasure },
            { 15, Globalisation.Dictionary.Modesty },
            { 16, Globalisation.Dictionary.Enthusiasm },
            { 17, Globalisation.Dictionary.Following },
            { 18, Globalisation.Dictionary.WorkOnWhatHasBeenSpoiled },
            { 19, Globalisation.Dictionary.Approach },
            { 20, Globalisation.Dictionary.Contemplation },
            { 21, Globalisation.Dictionary.BitingThrough },
            { 22, Globalisation.Dictionary.Grace },
            { 23, Globalisation.Dictionary.SplittingApart },
            { 24, Globalisation.Dictionary.Return },
            { 25, Globalisation.Dictionary.Innocence },
            { 26, Globalisation.Dictionary.TamingPowerOfTheGreat },
            { 27, Globalisation.Dictionary.ProvidingNourishment },
            { 28, Globalisation.Dictionary.PreponderanceOfTheGreat },
            { 29, Globalisation.Dictionary.TheAbysmal },
            { 30, Globalisation.Dictionary.TheClinging },
            { 31, Globalisation.Dictionary.Influence },
            { 32, Globalisation.Dictionary.Duration },
            { 33, Globalisation.Dictionary.Retreat },
            { 34, Globalisation.Dictionary.PowerOfTheGreat },
            { 35, Globalisation.Dictionary.Progress },
            { 36, Globalisation.Dictionary.DarkeningOfTheLight },
            { 37, Globalisation.Dictionary.TheFamily },
            { 38, Globalisation.Dictionary.Opposition },
            { 39, Globalisation.Dictionary.Obstruction },
            { 40, Globalisation.Dictionary.Deliverance },
            { 41, Globalisation.Dictionary.Decrease },
            { 42, Globalisation.Dictionary.Increase },
            { 43, Globalisation.Dictionary.Breakthrough },
            { 44, Globalisation.Dictionary.ComingToMeet },
            { 45, Globalisation.Dictionary.GatheringTogether },
            { 46, Globalisation.Dictionary.PushingUpward },
            { 47, Globalisation.Dictionary.Oppression },
            { 48, Globalisation.Dictionary.TheWell },
            { 49, Globalisation.Dictionary.Revolution },
            { 50, Globalisation.Dictionary.TheCauldron },
            { 51, Globalisation.Dictionary.TheArousing },
            { 52, Globalisation.Dictionary.KeepingStill },
            { 53, Globalisation.Dictionary.GradualProgress },
            { 54, Globalisation.Dictionary.TheMarryingMaiden },
            { 55, Globalisation.Dictionary.Abundance },
            { 56, Globalisation.Dictionary.TheWanderer },
            { 57, Globalisation.Dictionary.TheGentle },
            { 58, Globalisation.Dictionary.TheJoyous },
            { 59, Globalisation.Dictionary.Dispersion },
            { 60, Globalisation.Dictionary.Limitation },
            { 61, Globalisation.Dictionary.InnerTruth },
            { 62, Globalisation.Dictionary.PreponderanceOfTheSmall },
            { 63, Globalisation.Dictionary.AfterCompletion },
            { 64, Globalisation.Dictionary.BeforeCompletion }
        };

        private static readonly Dictionary<int, HexagramInfo> HexagramDetails = new Dictionary<int, HexagramInfo>
        {
            { 1, new HexagramInfo(Globalisation.Dictionary.BeginningsAndChallenges, Globalisation.Dictionary.TheCreative,
                Globalisation.Dictionary.TheCreativeTitle,
                Globalisation.Dictionary.TheCreativeSummary) },

            { 2, new HexagramInfo(Globalisation.Dictionary.BeginningsAndChallenges, Globalisation.Dictionary.TheReceptive,
                Globalisation.Dictionary.TheReceptiveTitle,
                Globalisation.Dictionary.TheReceptiveSummary) },

            { 3, new HexagramInfo(Globalisation.Dictionary.BeginningsAndChallenges, Globalisation.Dictionary.DifficultyAtTheBeginning,
                Globalisation.Dictionary.DifficultyAtTheBeginningTitle,
                Globalisation.Dictionary.DifficultyAtTheBeginningSummary) },

            { 4, new HexagramInfo(Globalisation.Dictionary.BeginningsAndChallenges, Globalisation.Dictionary.YouthfulFolly,
                Globalisation.Dictionary.YouthfulFollyTitle,
                Globalisation.Dictionary.YouthfulFollySummary) },

            { 5, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.Waiting,
                Globalisation.Dictionary.WaitingTitle,
                Globalisation.Dictionary.WaitingSummary) },

            { 6, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.Conflict,
                Globalisation.Dictionary.ConflictTitle,
                Globalisation.Dictionary.ConflictSummary) },

            { 7, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.TheArmy,
                Globalisation.Dictionary.TheArmyTitle,
                Globalisation.Dictionary.TheArmySummary) },

            { 8, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.HoldingTogether,
                Globalisation.Dictionary.HoldingTogetherTitle,
                Globalisation.Dictionary.HoldingTogetherSummary) },

            { 9, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.TamingPowerOfTheSmall,
                Globalisation.Dictionary.TamingPowerOfTheSmallTitle,
                Globalisation.Dictionary.TamingPowerOfTheSmallSummary) },

            { 10, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Treading,
                Globalisation.Dictionary.TreadingTitle,
                Globalisation.Dictionary.TreadingSummary) },

            { 11, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.Peace,
                Globalisation.Dictionary.PeaceTitle,
                Globalisation.Dictionary.PeaceSummary) },

            { 12, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Standstill,
                Globalisation.Dictionary.StandstillTitle,
                Globalisation.Dictionary.StandstillSummary) },

            { 13, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.FellowshipWithMen,
                Globalisation.Dictionary.FellowshipWithMenTitle,
                Globalisation.Dictionary.FellowshipWithMenSummary) },

            { 14, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.PossessionInGreatMeasure,
                Globalisation.Dictionary.PossessionInGreatMeasureTitle,
                Globalisation.Dictionary.PossessionInGreatMeasureSummary) },

            { 15, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Modesty,
                Globalisation.Dictionary.ModestyTitle,
                Globalisation.Dictionary.ModestySummary) },

            { 16, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Enthusiasm,
                Globalisation.Dictionary.EnthusiasmTitle,
                Globalisation.Dictionary.EnthusiasmSummary) },

            { 17, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.Following,
                Globalisation.Dictionary.FollowingTitle,
                Globalisation.Dictionary.FollowingSummary) },

            { 18, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.WorkOnWhatHasBeenSpoiled,
                Globalisation.Dictionary.WorkOnWhatHasBeenSpoiledTitle,
                Globalisation.Dictionary.WorkOnWhatHasBeenSpoiledSummary) },

            { 19, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.Approach,
                Globalisation.Dictionary.ApproachTitle,
                Globalisation.Dictionary.ApproachSummary) },

            { 20, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.Contemplation,
                Globalisation.Dictionary.ContemplationTitle,
                Globalisation.Dictionary.ContemplationSummary) },

            { 21, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.BitingThrough,
                Globalisation.Dictionary.BitingThroughTitle,
                Globalisation.Dictionary.BitingThroughSummary) },

            { 22, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Grace,
                Globalisation.Dictionary.GraceTitle,
                Globalisation.Dictionary.GraceSummary) },

            { 23, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.SplittingApart,
                Globalisation.Dictionary.SplittingApartTitle,
                Globalisation.Dictionary.SplittingApartSummary) },

            { 24, new HexagramInfo(Globalisation.Dictionary.MasteryAndCompletion, Globalisation.Dictionary.Return,
                Globalisation.Dictionary.ReturnTitle,
                Globalisation.Dictionary.ReturnSummary) },

            { 25, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Innocence,
                Globalisation.Dictionary.InnocenceTitle,
                Globalisation.Dictionary.InnocenceSummary) },

            { 26, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.TamingPowerOfTheGreat,
                Globalisation.Dictionary.TamingPowerOfTheGreatTitle,
                Globalisation.Dictionary.TamingPowerOfTheGreatSummary) },

            { 27, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.ProvidingNourishment,
                Globalisation.Dictionary.ProvidingNourishmentTitle,
                Globalisation.Dictionary.ProvidingNourishmentSummary) },

            { 28, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.PreponderanceOfTheGreat,
                Globalisation.Dictionary.PreponderanceOfTheGreatTitle,
                Globalisation.Dictionary.PreponderanceOfTheGreatSummary) },

            { 29, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.TheAbysmal,
                Globalisation.Dictionary.TheAbysmalTitle,
                Globalisation.Dictionary.TheAbysmalSummary) },

            { 30, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.TheClinging,
                Globalisation.Dictionary.TheClingingTitle,
                Globalisation.Dictionary.TheClingingSummary) },

            { 31, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.Influence,
                Globalisation.Dictionary.InfluenceTitle,
                Globalisation.Dictionary.InfluenceSummary) },

            { 32, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.Duration,
                Globalisation.Dictionary.DurationTitle,
                Globalisation.Dictionary.DurationSummary) },

            { 33, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Retreat,
                Globalisation.Dictionary.RetreatTitle,
                Globalisation.Dictionary.RetreatSummary) },

            { 34, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.PowerOfTheGreat,
                Globalisation.Dictionary.PowerOfTheGreatTitle,
                Globalisation.Dictionary.PowerOfTheGreatSummary) },

            { 35, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Progress,
                Globalisation.Dictionary.ProgressTitle,
                Globalisation.Dictionary.ProgressSummary) },

            { 36, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.DarkeningOfTheLight,
                Globalisation.Dictionary.DarkeningOfTheLightTitle,
                Globalisation.Dictionary.DarkeningOfTheLightSummary) },

            { 37, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.TheFamily,
                Globalisation.Dictionary.TheFamilyTitle,
                Globalisation.Dictionary.TheFamilySummary) },

            { 38, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.Opposition,
                Globalisation.Dictionary.OppositionTitle,
                Globalisation.Dictionary.OppositionSummary) },

            { 39, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Obstruction,
                Globalisation.Dictionary.ObstructionTitle,
                Globalisation.Dictionary.ObstructionSummary) },

            { 40, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.Deliverance,
                Globalisation.Dictionary.DeliveranceTitle,
                Globalisation.Dictionary.DeliveranceSummary) },

            { 41, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Decrease,
                Globalisation.Dictionary.DecreaseTitle,
                Globalisation.Dictionary.DecreaseSummary) },

            { 42, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Increase,
                Globalisation.Dictionary.IncreaseTitle,
                Globalisation.Dictionary.IncreaseSummary) },

            { 43, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Breakthrough,
                Globalisation.Dictionary.BreakthroughTitle,
                Globalisation.Dictionary.BreakthroughSummary) },

            { 44, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.ComingToMeet,
                Globalisation.Dictionary.ComingToMeetTitle,
                Globalisation.Dictionary.ComingToMeetSummary) },

            { 45, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.GatheringTogether,
                Globalisation.Dictionary.GatheringTogetherTitle,
                Globalisation.Dictionary.GatheringTogetherSummary) },

            { 46, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.PushingUpward,
                Globalisation.Dictionary.PushingUpwardTitle,
                Globalisation.Dictionary.PushingUpwardSummary) },

            { 47, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Oppression,
                Globalisation.Dictionary.OppressionTitle,
                Globalisation.Dictionary.OppressionSummary) },

            { 48, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.TheWell,
                Globalisation.Dictionary.TheWellTitle,
                Globalisation.Dictionary.TheWellSummary) },

            { 49, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Revolution,
                Globalisation.Dictionary.RevolutionTitle,
                Globalisation.Dictionary.RevolutionSummary) },

            { 50, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.TheCauldron,
                Globalisation.Dictionary.TheCauldronTitle,
                Globalisation.Dictionary.TheCauldronSummary) },

            { 51, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.TheArousing,
                Globalisation.Dictionary.TheArousingTitle,
                Globalisation.Dictionary.TheArousingSummary) },

            { 52, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.KeepingStill,
                Globalisation.Dictionary.KeepingStillTitle,
                Globalisation.Dictionary.KeepingStillSummary) },

            { 53, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.GradualProgress,
                Globalisation.Dictionary.GradualProgressTitle,
                Globalisation.Dictionary.GradualProgressSummary) },

            { 54, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.TheMarryingMaiden,
                Globalisation.Dictionary.TheMarryingMaidenTitle,
                Globalisation.Dictionary.TheMarryingMaidenSummary) },

            { 55, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Abundance,
                Globalisation.Dictionary.AbundanceTitle,
                Globalisation.Dictionary.AbundanceSummary) },

            { 56, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.TheWanderer,
                Globalisation.Dictionary.TheWandererTitle,
                Globalisation.Dictionary.TheWandererSummary) },

            { 57, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.TheGentle,
                Globalisation.Dictionary.TheGentleTitle,
                Globalisation.Dictionary.TheGentleSummary) },

            { 58, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.TheJoyous,
                Globalisation.Dictionary.TheJoyousTitle,
                Globalisation.Dictionary.TheJoyousSummary) },

            { 59, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Dispersion,
                Globalisation.Dictionary.DispersionTitle,
                Globalisation.Dictionary.DispersionSummary) },

            { 60, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.Limitation,
                Globalisation.Dictionary.LimitationTitle,
                Globalisation.Dictionary.LimitationSummary) },

            { 61, new HexagramInfo(Globalisation.Dictionary.MasteryAndCompletion, Globalisation.Dictionary.InnerTruth,
                Globalisation.Dictionary.InnerTruthTitle,
                Globalisation.Dictionary.InnerTruthSummary) },

            { 62, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.PreponderanceOfTheSmall,
                Globalisation.Dictionary.PreponderanceOfTheSmallTitle,
                Globalisation.Dictionary.PreponderanceOfTheSmallSummary) },

            { 63, new HexagramInfo(Globalisation.Dictionary.MasteryAndCompletion, Globalisation.Dictionary.AfterCompletion,
                Globalisation.Dictionary.AfterCompletionTitle,
                Globalisation.Dictionary.AfterCompletionSummary) },

            { 64, new HexagramInfo(Globalisation.Dictionary.MasteryAndCompletion, Globalisation.Dictionary.BeforeCompletion,
                Globalisation.Dictionary.BeforeCompletionTitle,
                Globalisation.Dictionary.BeforeCompletionSummary) }
        };

        public ELineType[] Lines { get; }
        public int Number { get; }
        public string Name { get; }

        public Hexagram(ELineType[] lines)
        {
            Lines = lines;
            Number = GetHexagramNumber(lines);
            Name = HexagramNames.ContainsKey(Number) ? HexagramNames[Number] : "Unknown";
        }

        public HexagramInfo GetHexagramInfo()
        {
            return HexagramDetails.ContainsKey(Number) ? HexagramDetails[Number] : null;
        }

        public Hexagram Transform()
        {
            ELineType[] transformedELines = new ELineType[6];
            for (int i = 0; i < 6; i++)
            {
                switch (Lines[i])
                {
                    case ELineType.ChangingYang:
                        transformedELines[i] = ELineType.Yin;
                        break;
                    case ELineType.ChangingYin:
                        transformedELines[i] = ELineType.Yang;
                        break;
                    default:
                        transformedELines[i] = Lines[i];
                        break;
                }
            }
            return new Hexagram(transformedELines);
        }

        public string ToHtml()
        {
            var html = new StringBuilder();
            html.Append("<div class='hexagram-container'>");

            for (int i = 5; i >= 0; i--)
            {
                html.Append($"<div class='{GetCssClass(Lines[i])}'></div>");
            }

            html.Append($"<div class='hexagram-title'>Hexagram {Number}: {Name}</div>");
            html.Append("</div>");
            return html.ToString();
        }

        public override string ToString()
        {
            return $"Hexagram {Number}: {Name}\n" + GetHexagramDiagram(Lines);
        }

        private static int GetHexagramNumber(ELineType[] eLines)
        {
            int binaryValue = 0;
            for (int i = 0; i < 6; i++)
            {
                bool isYang = eLines[5 - i] == ELineType.Yang || eLines[5 - i] == ELineType.ChangingYang;
                binaryValue = (binaryValue << 1) | (isYang ? 1 : 0);
            }
            return binaryValue + 1;
        }

        private static string GetHexagramDiagram(ELineType[] eLines)
        {
            List<string> diagram = new List<string>();
            foreach (var line in eLines)
            {
                switch (line)
                {
                    case ELineType.Yang:
                        diagram.Add("───────");
                        break;
                    case ELineType.Yin:
                        diagram.Add("──   ──");
                        break;
                    case ELineType.ChangingYang:
                        diagram.Add("─────── (X)");
                        break;
                    case ELineType.ChangingYin:
                        diagram.Add("──   ── (O)");
                        break;
                }
            }

            diagram.Reverse();
            return string.Join("\n", diagram);
        }

        private static string GetCssClass(ELineType line)
        {
            switch (line)
            {
                case ELineType.Yang: return "hex-line yang";
                case ELineType.Yin: return "hex-line yin";
                case ELineType.ChangingYang: return "hex-line yang changing";
                case ELineType.ChangingYin: return "hex-line yin changing";
                default: return "hex-line";
            }
        }
    }
}