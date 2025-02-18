using K9.SharedLibrary.Helpers;
using K9.WebApplication.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

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
                Globalisation.Dictionary.TheCreativeSummary,
                Globalisation.Dictionary.the_creative_details) },

            { 2, new HexagramInfo(Globalisation.Dictionary.BeginningsAndChallenges, Globalisation.Dictionary.TheReceptive,
                Globalisation.Dictionary.TheReceptiveTitle,
                Globalisation.Dictionary.TheReceptiveSummary,
                Globalisation.Dictionary.the_receptive_details) },

            { 3, new HexagramInfo(Globalisation.Dictionary.BeginningsAndChallenges, Globalisation.Dictionary.DifficultyAtTheBeginning,
                Globalisation.Dictionary.DifficultyAtTheBeginningTitle,
                Globalisation.Dictionary.DifficultyAtTheBeginningSummary,
                Globalisation.Dictionary.difficulty_at_the_beginning_details) },

            { 4, new HexagramInfo(Globalisation.Dictionary.BeginningsAndChallenges, Globalisation.Dictionary.YouthfulFolly,
                Globalisation.Dictionary.YouthfulFollyTitle,
                Globalisation.Dictionary.YouthfulFollySummary,
                Globalisation.Dictionary.youthful_folly_details) },

            { 5, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.Waiting,
                Globalisation.Dictionary.WaitingTitle,
                Globalisation.Dictionary.WaitingSummary,
                Globalisation.Dictionary.waiting_details) },

            { 6, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.Conflict,
                Globalisation.Dictionary.ConflictTitle,
                Globalisation.Dictionary.ConflictSummary,
                Globalisation.Dictionary.conflict_details) },

            { 7, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.TheArmy,
                Globalisation.Dictionary.TheArmyTitle,
                Globalisation.Dictionary.TheArmySummary,
                Globalisation.Dictionary.the_army_details) },

            { 8, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.HoldingTogether,
                Globalisation.Dictionary.HoldingTogetherTitle,
                Globalisation.Dictionary.HoldingTogetherSummary,
                Globalisation.Dictionary.holding_together_details) },

            { 9, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.TamingPowerOfTheSmall,
                Globalisation.Dictionary.TamingPowerOfTheSmallTitle,
                Globalisation.Dictionary.TamingPowerOfTheSmallSummary,
                Globalisation.Dictionary.taming_power_of_the_small_details) },

            { 10, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Treading,
                Globalisation.Dictionary.TreadingTitle,
                Globalisation.Dictionary.TreadingSummary,
                Globalisation.Dictionary.treading_details) },

            { 11, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.Peace,
                Globalisation.Dictionary.PeaceTitle,
                Globalisation.Dictionary.PeaceSummary,
                Globalisation.Dictionary.peace_details) },

            { 12, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Standstill,
                Globalisation.Dictionary.StandstillTitle,
                Globalisation.Dictionary.StandstillSummary,
                Globalisation.Dictionary.standstill_details) },

            { 13, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.FellowshipWithMen,
                Globalisation.Dictionary.FellowshipWithMenTitle,
                Globalisation.Dictionary.FellowshipWithMenSummary,
                Globalisation.Dictionary.fellowship_with_men_details) },

            { 14, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.PossessionInGreatMeasure,
                Globalisation.Dictionary.PossessionInGreatMeasureTitle,
                Globalisation.Dictionary.PossessionInGreatMeasureSummary,
                Globalisation.Dictionary.possession_in_great_measure_details) },

            { 15, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Modesty,
                Globalisation.Dictionary.ModestyTitle,
                Globalisation.Dictionary.ModestySummary,
                Globalisation.Dictionary.modesty_details) },

            { 16, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Enthusiasm,
                Globalisation.Dictionary.EnthusiasmTitle,
                Globalisation.Dictionary.EnthusiasmSummary,
                Globalisation.Dictionary.enthusiasm_details) },

            { 17, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.Following,
                Globalisation.Dictionary.FollowingTitle,
                Globalisation.Dictionary.FollowingSummary,
                Globalisation.Dictionary.following_details) },

            { 18, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.WorkOnWhatHasBeenSpoiled,
                Globalisation.Dictionary.WorkOnWhatHasBeenSpoiledTitle,
                Globalisation.Dictionary.WorkOnWhatHasBeenSpoiledSummary,
                Globalisation.Dictionary.work_on_what_has_been_spoiled_details) },

            { 19, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.Approach,
                Globalisation.Dictionary.ApproachTitle,
                Globalisation.Dictionary.ApproachSummary,
                Globalisation.Dictionary.approach_details) },

            { 20, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.Contemplation,
                Globalisation.Dictionary.ContemplationTitle,
                Globalisation.Dictionary.ContemplationSummary,
                Globalisation.Dictionary.contemplation_details) },

            { 21, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.BitingThrough,
                Globalisation.Dictionary.BitingThroughTitle,
                Globalisation.Dictionary.BitingThroughSummary,
                Globalisation.Dictionary.biting_through_details) },

            { 22, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Grace,
                Globalisation.Dictionary.GraceTitle,
                Globalisation.Dictionary.GraceSummary,
                Globalisation.Dictionary.grace_details) },

            { 23, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.SplittingApart,
                Globalisation.Dictionary.SplittingApartTitle,
                Globalisation.Dictionary.SplittingApartSummary,
                Globalisation.Dictionary.splitting_apart_details) },

            { 24, new HexagramInfo(Globalisation.Dictionary.MasteryAndCompletion, Globalisation.Dictionary.Return,
                Globalisation.Dictionary.ReturnTitle,
                Globalisation.Dictionary.ReturnSummary,
                Globalisation.Dictionary.return_details) },

            { 25, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Innocence,
                Globalisation.Dictionary.InnocenceTitle,
                Globalisation.Dictionary.InnocenceSummary,
                Globalisation.Dictionary.innocence_details) },

            { 26, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.TamingPowerOfTheGreat,
                Globalisation.Dictionary.TamingPowerOfTheGreatTitle,
                Globalisation.Dictionary.TamingPowerOfTheGreatSummary,
                Globalisation.Dictionary.taming_power_of_the_great_details) },

            { 27, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.ProvidingNourishment,
                Globalisation.Dictionary.ProvidingNourishmentTitle,
                Globalisation.Dictionary.ProvidingNourishmentSummary,
                Globalisation.Dictionary.providing_nourishment_details) },

            { 28, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.PreponderanceOfTheGreat,
                Globalisation.Dictionary.PreponderanceOfTheGreatTitle,
                Globalisation.Dictionary.PreponderanceOfTheGreatSummary,
                Globalisation.Dictionary.preponderance_of_the_great_details) },

            { 29, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.TheAbysmal,
                Globalisation.Dictionary.TheAbysmalTitle,
                Globalisation.Dictionary.TheAbysmalSummary,
                Globalisation.Dictionary.the_abysmal_details) },

            { 30, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.TheClinging,
                Globalisation.Dictionary.TheClingingTitle,
                Globalisation.Dictionary.TheClingingSummary,
                Globalisation.Dictionary.the_clinging_details) },

            { 31, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.Influence,
                Globalisation.Dictionary.InfluenceTitle,
                Globalisation.Dictionary.InfluenceSummary,
                Globalisation.Dictionary.influence_details) },

            { 32, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.Duration,
                Globalisation.Dictionary.DurationTitle,
                Globalisation.Dictionary.DurationSummary,
                Globalisation.Dictionary.duration_details) },

            { 33, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Retreat,
                Globalisation.Dictionary.RetreatTitle,
                Globalisation.Dictionary.RetreatSummary,
                Globalisation.Dictionary.retreat_details) },

            { 34, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.PowerOfTheGreat,
                Globalisation.Dictionary.PowerOfTheGreatTitle,
                Globalisation.Dictionary.PowerOfTheGreatSummary,
                Globalisation.Dictionary.the_power_of_the_great_details) },

            { 35, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Progress,
                Globalisation.Dictionary.ProgressTitle,
                Globalisation.Dictionary.ProgressSummary,
                Globalisation.Dictionary.progress_details) },

            { 36, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.DarkeningOfTheLight,
                Globalisation.Dictionary.DarkeningOfTheLightTitle,
                Globalisation.Dictionary.DarkeningOfTheLightSummary,
                Globalisation.Dictionary.darkening_of_the_light_details) },

            { 37, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.TheFamily,
                Globalisation.Dictionary.TheFamilyTitle,
                Globalisation.Dictionary.TheFamilySummary,
                Globalisation.Dictionary.the_family_details) },

            { 38, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.Opposition,
                Globalisation.Dictionary.OppositionTitle,
                Globalisation.Dictionary.OppositionSummary,
                Globalisation.Dictionary.opposition_details) },

            { 39, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Obstruction,
                Globalisation.Dictionary.ObstructionTitle,
                Globalisation.Dictionary.ObstructionSummary,
                Globalisation.Dictionary.obstruction_details) },

            { 40, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.Deliverance,
                Globalisation.Dictionary.DeliveranceTitle,
                Globalisation.Dictionary.DeliveranceSummary,
                Globalisation.Dictionary.deliverance_details) },

            { 41, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Decrease,
                Globalisation.Dictionary.DecreaseTitle,
                Globalisation.Dictionary.DecreaseSummary,
                Globalisation.Dictionary.decrease_details) },

            { 42, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Increase,
                Globalisation.Dictionary.IncreaseTitle,
                Globalisation.Dictionary.IncreaseSummary,
                Globalisation.Dictionary.increase_details) },

            { 43, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Breakthrough,
                Globalisation.Dictionary.BreakthroughTitle,
                Globalisation.Dictionary.BreakthroughSummary,
                Globalisation.Dictionary.breakthrough_details) },

            { 44, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.ComingToMeet,
                Globalisation.Dictionary.ComingToMeetTitle,
                Globalisation.Dictionary.ComingToMeetSummary,
                Globalisation.Dictionary.coming_to_meet_details) },

            { 45, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.GatheringTogether,
                Globalisation.Dictionary.GatheringTogetherTitle,
                Globalisation.Dictionary.GatheringTogetherSummary,
                Globalisation.Dictionary.gathering_together_details) },

            { 46, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.PushingUpward,
                Globalisation.Dictionary.PushingUpwardTitle,
                Globalisation.Dictionary.PushingUpwardSummary,
                Globalisation.Dictionary.pushing_upward_details) },

            { 47, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Oppression,
                Globalisation.Dictionary.OppressionTitle,
                Globalisation.Dictionary.OppressionSummary,
                Globalisation.Dictionary.oppression_details) },

            { 48, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.TheWell,
                Globalisation.Dictionary.TheWellTitle,
                Globalisation.Dictionary.TheWellSummary,
                Globalisation.Dictionary.the_well_details) },

            { 49, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Revolution,
                Globalisation.Dictionary.RevolutionTitle,
                Globalisation.Dictionary.RevolutionSummary,
                Globalisation.Dictionary.revolution_details) },

            { 50, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.TheCauldron,
                Globalisation.Dictionary.TheCauldronTitle,
                Globalisation.Dictionary.TheCauldronSummary,
                Globalisation.Dictionary.the_cauldron_details) },

            { 51, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.TheArousing,
                Globalisation.Dictionary.TheArousingTitle,
                Globalisation.Dictionary.TheArousingSummary,
                Globalisation.Dictionary.the_arousing_details) },

            { 52, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.KeepingStill,
                Globalisation.Dictionary.KeepingStillTitle,
                Globalisation.Dictionary.KeepingStillSummary,
                Globalisation.Dictionary.keeping_still_details) },

            { 53, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.GradualProgress,
                Globalisation.Dictionary.GradualProgressTitle,
                Globalisation.Dictionary.GradualProgressSummary,
                Globalisation.Dictionary.gradual_progress_details) },

            { 54, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.TheMarryingMaiden,
                Globalisation.Dictionary.TheMarryingMaidenTitle,
                Globalisation.Dictionary.TheMarryingMaidenSummary,
                Globalisation.Dictionary.the_marrying_maiden_details) },
            
            { 55, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.Abundance,
                Globalisation.Dictionary.AbundanceTitle,
                Globalisation.Dictionary.AbundanceSummary,
                Globalisation.Dictionary.abundance_details) },

            { 56, new HexagramInfo(Globalisation.Dictionary.RelationshipsAndTransformation, Globalisation.Dictionary.TheWanderer,
                Globalisation.Dictionary.TheWandererTitle,
                Globalisation.Dictionary.TheWandererSummary,
                Globalisation.Dictionary.the_wanderer_details) },

            { 57, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.TheGentle,
                Globalisation.Dictionary.TheGentleTitle,
                Globalisation.Dictionary.TheGentleSummary,
                Globalisation.Dictionary.the_gentle_details) },

            { 58, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.TheJoyous,
                Globalisation.Dictionary.TheJoyousTitle,
                Globalisation.Dictionary.TheJoyousSummary,
                Globalisation.Dictionary.the_joyous_details) },

            { 59, new HexagramInfo(Globalisation.Dictionary.OvercomingObstacles, Globalisation.Dictionary.Dispersion,
                Globalisation.Dictionary.DispersionTitle,
                Globalisation.Dictionary.DispersionSummary,
                Globalisation.Dictionary.dispersion_details) },

            { 60, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.Limitation,
                Globalisation.Dictionary.LimitationTitle,
                Globalisation.Dictionary.LimitationSummary,
                Globalisation.Dictionary.limitation_details) },

            { 61, new HexagramInfo(Globalisation.Dictionary.AbundanceAndChange, Globalisation.Dictionary.InnerTruth,
                Globalisation.Dictionary.InnerTruthTitle,
                Globalisation.Dictionary.InnerTruthSummary,
                Globalisation.Dictionary.inner_truth_details) },

            { 62, new HexagramInfo(Globalisation.Dictionary.GrowthAndStability, Globalisation.Dictionary.PreponderanceOfTheSmall,
                Globalisation.Dictionary.PreponderanceOfTheSmallTitle,
                Globalisation.Dictionary.PreponderanceOfTheSmallSummary,
                Globalisation.Dictionary.preponderance_of_the_small_details) },

            { 63, new HexagramInfo(Globalisation.Dictionary.MasteryAndCompletion, Globalisation.Dictionary.AfterCompletion,
                Globalisation.Dictionary.AfterCompletionTitle,
                Globalisation.Dictionary.AfterCompletionSummary,
                Globalisation.Dictionary.after_completion_details) },

            { 64, new HexagramInfo(Globalisation.Dictionary.MasteryAndCompletion, Globalisation.Dictionary.BeforeCompletion,
                Globalisation.Dictionary.BeforeCompletionTitle, 
                Globalisation.Dictionary.BeforeCompletionSummary,
                Globalisation.Dictionary.before_completion_details) }
        };

        private static readonly Dictionary<int, Dictionary<int, string>> ChangingLineInterpretations = new Dictionary<int, Dictionary<int, string>>
        {
            { 1, new Dictionary<int, string>
                {
                    { 1, "Strength in beginnings. Be cautious and avoid arrogance in early success." },
                    { 2, "Patience is needed. Do not rush; steady growth leads to power." },
                    { 3, "Unexpected obstacles appear. Strength alone is not enough; wisdom is needed." },
                    { 4, "A moment of clarity—learning when to yield is as important as knowing when to act." },
                    { 5, "True leadership emerges through humility and balance. Trust in timing." },
                    { 6, "Overreaching leads to exhaustion. Success requires rest and reflection." }
                }
            },

            { 2, new Dictionary<int, string>
                {
                    { 1, "Receptivity is key. Accept help and allow guidance to shape your path." },
                    { 2, "A time of stillness and preparation. Do not force progress." },
                    { 3, "Challenges in leadership arise. Remain open but firm in your integrity." },
                    { 4, "A shift is coming. Trust that the right path will be revealed." },
                    { 5, "The right people enter your life. Collaboration brings success." },
                    { 6, "Remaining passive for too long leads to stagnation. Take action." }
                }
            },

            { 3, new Dictionary<int, string>
                {
                    { 1, "Beginnings are difficult, but perseverance leads to success." },
                    { 2, "Seek support from others rather than struggling alone." },
                    { 3, "Uncertainty is temporary. Adapt and remain flexible." },
                    { 4, "Rushing forward without clarity brings failure. Step back and reassess." },
                    { 5, "Persistence pays off—support is coming." },
                    { 6, "The struggle ends, but be mindful of exhaustion. Rebuild wisely." }
                }
            },

            { 4, new Dictionary<int, string>
                {
                    { 1, "Inexperience is natural. Remain open to learning." },
                    { 2, "Avoid overconfidence; seek guidance before acting." },
                    { 3, "A mistake can be a lesson if approached with humility." },
                    { 4, "Learning requires patience. Do not demand immediate results." },
                    { 5, "True wisdom comes through experience, not shortcuts." },
                    { 6, "Ignoring wisdom leads to repeating past mistakes." }
                }
            },

            { 5, new Dictionary<int, string>
                {
                    { 1, "Wait with trust. The right moment will arrive soon." },
                    { 2, "Preparation is key. Do not act prematurely." },
                    { 3, "External struggles reflect internal uncertainty." },
                    { 4, "Patience will be rewarded. Unexpected help arrives." },
                    { 5, "Trust your inner strength—confidence attracts success." },
                    { 6, "The waiting is over; movement is now necessary." }
                }
            },

            { 6, new Dictionary<int, string>
                {
                    { 1, "Do not escalate conflict. Seek resolution through wisdom." },
                    { 2, "Avoid unnecessary fights. Compromise is possible." },
                    { 3, "The struggle continues, but victory is not always the best outcome." },
                    { 4, "A change of strategy is needed. Step away if necessary." },
                    { 5, "Patience wins over aggression. The conflict will resolve naturally." },
                    { 6, "True victory comes not from fighting, but from knowing when to let go." }
                }
            },

            { 7, new Dictionary<int, string>
                {
                    { 1, "Leadership requires discipline. Avoid arrogance." },
                    { 2, "A strong foundation is needed before expanding further." },
                    { 3, "Teamwork is crucial. Lone efforts will fail." },
                    { 4, "Know when to act and when to wait." },
                    { 5, "Lead by example, not by force." },
                    { 6, "Power misused leads to downfall. Remain ethical." }
                }
            },

            { 8, new Dictionary<int, string>
                {
                    { 1, "Unity is forming. Stay open to new alliances." },
                    { 2, "Trust is essential for cooperation." },
                    { 3, "Disagreements test relationships—resolve with wisdom." },
                    { 4, "Patience strengthens bonds over time." },
                    { 5, "A strong leader unites others effectively." },
                    { 6, "Isolation weakens success. Seek connection." }
                }
            },

            { 9, new Dictionary<int, string>
                {
                    { 1, "Small actions accumulate to bring great change." },
                    { 2, "Subtle influence is more powerful than direct force." },
                    { 3, "Avoid overextending yourself; focus on the essentials." },
                    { 4, "Timing is key—be patient and persistent." },
                    { 5, "Transformation is occurring; trust the process." },
                    { 6, "Your efforts will bear fruit if you maintain balance." }
                }
            },

            { 10, new Dictionary<int, string>
                {
                    { 1, "Caution is needed in early steps. Move carefully and be aware of surroundings." },
                    { 2, "Be mindful of your position. Balance confidence with humility." },
                    { 3, "A risky situation is ahead. Step forward only if truly prepared." },
                    { 4, "The path becomes clear, but patience is required before taking action." },
                    { 5, "Recognition and success come, but avoid arrogance." },
                    { 6, "Overstepping the boundary leads to misfortune. Know when to stop." }
                }
            },

            { 11, new Dictionary<int, string>
                {
                    { 1, "A new opportunity arises. Align with the flow of progress." },
                    { 2, "Harmonious conditions strengthen relationships and endeavors." },
                    { 3, "External harmony may hide internal struggles. Stay aware." },
                    { 4, "A change is coming. Adapt gracefully." },
                    { 5, "A leader emerges who brings balance and wisdom." },
                    { 6, "The peak has been reached. Prepare for a shift in circumstances." }
                }
            },

            { 12, new Dictionary<int, string>
                {
                    { 1, "Difficult times are beginning. Remain steady." },
                    { 2, "Withdraw from conflict and focus on inner growth." },
                    { 3, "Efforts seem blocked. Stay patient and observant." },
                    { 4, "A shift in perspective reveals a new path." },
                    { 5, "Change is coming, but take care to act at the right time." },
                    { 6, "The situation will soon reverse. Endurance brings success." }
                }
            },

            { 13, new Dictionary<int, string>
                {
                    { 1, "Seek unity with those around you. Cooperation is key." },
                    { 2, "Not all alliances are beneficial. Choose wisely." },
                    { 3, "Conflicts within a group arise. Resolution requires diplomacy." },
                    { 4, "A new perspective strengthens relationships." },
                    { 5, "Shared values unite people. Stay true to principles." },
                    { 6, "Lasting harmony is achieved through mutual trust." }
                }
            },

            { 14, new Dictionary<int, string>
                {
                    { 1, "Handle prosperity with humility and wisdom." },
                    { 2, "Wealth alone does not bring happiness. Seek deeper fulfillment." },
                    { 3, "Be generous with your success. Sharing brings greater rewards." },
                    { 4, "A new opportunity appears—embrace it carefully." },
                    { 5, "True abundance comes from inner peace." },
                    { 6, "Holding too tightly to material things leads to loss." }
                }
            },

            { 15, new Dictionary<int, string>
                {
                    { 1, "Remain modest in success. Humility attracts support." },
                    { 2, "Balance ambition with gratitude." },
                    { 3, "Avoid pride—stay grounded and sincere." },
                    { 4, "By acting with humility, you gain greater influence." },
                    { 5, "True strength comes from inner modesty." },
                    { 6, "Even in great success, remain humble." }
                }
            },

            { 16, new Dictionary<int, string>
                {
                    { 1, "A wave of enthusiasm is rising. Channel it productively." },
                    { 2, "Avoid rushing ahead blindly. Thoughtful planning is needed." },
                    { 3, "Excitement is contagious. Use it to inspire others." },
                    { 4, "A shift in focus brings new energy." },
                    { 5, "A time of creativity and inspiration arrives." },
                    { 6, "Too much enthusiasm can lead to recklessness. Stay balanced." }
                }
            },

            { 17, new Dictionary<int, string>
                {
                    { 1, "Following the right path brings good fortune." },
                    { 2, "Be mindful of who you follow. Choose your influences carefully." },
                    { 3, "Adapting to circumstances is necessary for success." },
                    { 4, "A shift in leadership brings new opportunities." },
                    { 5, "True leadership comes from understanding others." },
                    { 6, "Following blindly leads to misfortune. Stay true to yourself." }
                }
            },

            { 18, new Dictionary<int, string>
                {
                    { 1, "Past mistakes must be corrected before progress is possible." },
                    { 2, "Healing requires effort, but the results will be lasting." },
                    { 3, "Recognizing errors is the first step to improvement." },
                    { 4, "A fresh approach is needed. Let go of old ways." },
                    { 5, "Transformation brings renewal and clarity." },
                    { 6, "Lingering on past mistakes leads to stagnation. Move forward." }
                }
            },

            { 19, new Dictionary<int, string>
                {
                    { 1, "New opportunities are approaching. Prepare wisely." },
                    { 2, "Growth is steady, but patience is required." },
                    { 3, "Be mindful of arrogance in success." },
                    { 4, "A shift in perspective brings new understanding." },
                    { 5, "Welcoming change leads to expansion." },
                    { 6, "The cycle is reaching its peak—be ready for transition." }
                }
            },

            { 20, new Dictionary<int, string>
                {
                    { 1, "Observation brings clarity. Step back before making decisions." },
                    { 2, "A broader perspective reveals hidden details." },
                    { 3, "A time for introspection—seek wisdom before acting." },
                    { 4, "By contemplating the past, the future becomes clearer." },
                    { 5, "Deep insight leads to wise leadership." },
                    { 6, "Seeing the truth may be uncomfortable, but it is necessary." }
                }
            },

            { 21, new Dictionary<int, string>
                {
                    { 1, "Justice requires action. Address unresolved issues." },
                    { 2, "Patience is key—avoid acting in haste." },
                    { 3, "The situation is complex—seek clarity before proceeding." },
                    { 4, "Obstacles can be overcome through persistence." },
                    { 5, "Truth must be upheld, even if difficult." },
                    { 6, "A harsh decision may be necessary for justice to prevail." }
                }
            },

            { 22, new Dictionary<int, string>
                {
                    { 1, "True beauty lies in authenticity." },
                    { 2, "Simplicity brings elegance and clarity." },
                    { 3, "Appearances may deceive—focus on substance." },
                    { 4, "Balance inner and outer refinement." },
                    { 5, "A moment of inspiration creates lasting impact." },
                    { 6, "Superficiality leads to emptiness—seek depth." }
                }
            },

            { 23, new Dictionary<int, string>
                {
                    { 1, "A situation is deteriorating—prepare for change." },
                    { 2, "Letting go of attachments allows renewal." },
                    { 3, "Structures are crumbling—rebuild wisely." },
                    { 4, "Transformation is painful but necessary." },
                    { 5, "The worst is almost over—endurance brings relief." },
                    { 6, "A complete shift is imminent—embrace the new." }
                }
            },

            { 24, new Dictionary<int, string>
                {
                    { 1, "A fresh start is emerging—stay open to it." },
                    { 2, "Trust in cycles of renewal—things improve." },
                    { 3, "The past may return—be mindful of lessons learned." },
                    { 4, "Change is happening—flow with it, not against it." },
                    { 5, "A key opportunity is approaching—be ready." },
                    { 6, "Do not resist transformation—it leads to growth." }
                }
            },

            { 25, new Dictionary<int, string>
                {
                    { 1, "Stay true to yourself—genuine actions bring success." },
                    { 2, "Unexpected events require flexibility." },
                    { 3, "Honesty and integrity lead to lasting rewards." },
                    { 4, "Do not manipulate outcomes—trust the process." },
                    { 5, "Spontaneity brings good fortune—stay open." },
                    { 6, "Avoid forcing things—let nature take its course." }
                }
            },

            { 26, new Dictionary<int, string>
                {
                    { 1, "Gather strength before taking action." },
                    { 2, "Patience is required—avoid rushing." },
                    { 3, "Discipline ensures long-term success." },
                    { 4, "A moment of restraint is necessary." },
                    { 5, "Mastery comes through perseverance." },
                    { 6, "Balance power with wisdom." }
                }
            },

            { 27, new Dictionary<int, string>
                {
                    { 1, "Nourish yourself wisely—body and mind." },
                    { 2, "True fulfillment comes from meaningful sources." },
                    { 3, "Avoid unhealthy influences." },
                    { 4, "Sustenance comes from inner wisdom." },
                    { 5, "Give and receive nourishment equally." },
                    { 6, "What you feed grows—choose carefully." }
                }
            },

            { 28, new Dictionary<int, string>
                {
                    { 1, "The burden is heavy—seek support where needed." },
                    { 2, "Balance is key to handling pressure effectively." },
                    { 3, "Struggles lead to resilience—persist wisely." },
                    { 4, "Adaptation is necessary to avoid collapse." },
                    { 5, "A critical point has been reached—handle with care." },
                    { 6, "Transformation is inevitable—embrace the shift." }
                }
            },

            { 29, new Dictionary<int, string>
                {
                    { 1, "Difficulties lie ahead—stay resilient and cautious." },
                    { 2, "Patience and endurance will help navigate obstacles." },
                    { 3, "The situation is overwhelming—seek support." },
                    { 4, "Embrace the challenge—strength is forged in adversity." },
                    { 5, "A breakthrough is near—trust your inner strength." },
                    { 6, "Hardships will soon pass—keep faith in the journey." }
                }
            },

            { 30, new Dictionary<int, string>
                {
                    { 1, "Clarity is emerging—focus on what is essential." },
                    { 2, "Let go of distractions to find true insight." },
                    { 3, "Passion must be balanced with wisdom." },
                    { 4, "A guiding light appears—follow it with trust." },
                    { 5, "Illumination brings understanding—share your knowledge." },
                    { 6, "Brightness fades—sustain your inner light." }
                }
            },

            { 31, new Dictionary<int, string>
                {
                    { 1, "Influence is subtle—act with gentleness." },
                    { 2, "True connection is built on sincerity." },
                    { 3, "Avoid manipulative tendencies—genuine influence is best." },
                    { 4, "Emotional balance leads to effective persuasion." },
                    { 5, "A powerful attraction is forming—handle it wisely." },
                    { 6, "Do not force relationships—let things develop naturally." }
                }
            },

            { 32, new Dictionary<int, string>
                {
                    { 1, "Endurance requires patience—do not rush." },
                    { 2, "Sustained effort leads to lasting success." },
                    { 3, "A long-term perspective brings clarity." },
                    { 4, "Stability is key—do not waver in your commitments." },
                    { 5, "Stay consistent and avoid sudden changes." },
                    { 6, "An outdated approach needs adjustment—adapt wisely." }
                }
            },

            { 33, new Dictionary<int, string>
                {
                    { 1, "Withdrawal is sometimes the best strategy." },
                    { 2, "Stepping back can lead to a better position later." },
                    { 3, "Do not flee out of fear—retreat with purpose." },
                    { 4, "Timing is crucial—know when to advance again." },
                    { 5, "A wise leader knows when to step aside." },
                    { 6, "A complete withdrawal may be necessary." }
                }
            },

            { 34, new Dictionary<int, string>
                {
                    { 1, "Great power must be handled responsibly." },
                    { 2, "Restraint strengthens true power." },
                    { 3, "Uncontrolled strength leads to conflict—use wisdom." },
                    { 4, "Power used wisely uplifts everyone." },
                    { 5, "Strength is best combined with compassion." },
                    { 6, "Overreaching leads to downfall—know your limits." }
                }
            },

            { 35, new Dictionary<int, string>
                {
                    { 1, "Progress begins with a single step—move forward." },
                    { 2, "Steady effort leads to success." },
                    { 3, "Momentum is building—use it wisely." },
                    { 4, "Rapid advancement is possible, but stay grounded." },
                    { 5, "Recognition is coming—stay humble and focused." },
                    { 6, "Success requires ongoing effort—do not become complacent." }
                }
            },

            { 36, new Dictionary<int, string>
                {
                    { 1, "A time of darkness—move with caution." },
                    { 2, "Protect your inner light from external pressures." },
                    { 3, "Misunderstanding is possible—communicate clearly." },
                    { 4, "A challenge tests your strength—persevere." },
                    { 5, "Wisdom grows through difficult times." },
                    { 6, "A new dawn approaches—hold onto your vision." }
                }
            },

            { 37, new Dictionary<int, string>
                {
                    { 1, "A strong foundation ensures a secure future." },
                    { 2, "Family harmony is built on mutual respect." },
                    { 3, "Responsibility strengthens relationships." },
                    { 4, "Balance personal and communal needs." },
                    { 5, "True leadership begins at home." },
                    { 6, "Nurturing brings long-term stability." }
                }
            },

            { 38, new Dictionary<int, string>
                {
                    { 1, "Opposition can be productive—seek common ground." },
                    { 2, "Disagreements reveal deeper truths." },
                    { 3, "Avoid unnecessary conflicts—focus on resolution." },
                    { 4, "Seeing things from another perspective creates harmony." },
                    { 5, "Respecting differences leads to understanding." },
                    { 6, "Unity is possible despite contrast." }
                }
            },

            { 39, new Dictionary<int, string>
                {
                    { 1, "Obstacles are present—pause and reassess." },
                    { 2, "A change of approach is needed to overcome difficulties." },
                    { 3, "Seek allies to help navigate challenges." },
                    { 4, "Patience is key—obstacles will soon clear." },
                    { 5, "Growth comes through perseverance." },
                    { 6, "Breakthrough is near—stay determined." }
                }
            },

            { 40, new Dictionary<int, string>
                {
                    { 1, "Release burdens that no longer serve you." },
                    { 2, "Forgiveness brings freedom." },
                    { 3, "Letting go allows new opportunities to emerge." },
                    { 4, "Relief is coming—trust in the process." },
                    { 5, "Moving forward requires leaving the past behind." },
                    { 6, "A fresh start is possible—embrace it." }
                }
            },

            { 41, new Dictionary<int, string>
                {
                    { 1, "Simplification leads to clarity—let go of excess." },
                    { 2, "Giving up something small brings greater rewards." },
                    { 3, "A shift in priorities is necessary for balance." },
                    { 4, "True wealth is found in simplicity and contentment." },
                    { 5, "Reducing distractions brings sharper focus." },
                    { 6, "Excessive sacrifice may not be beneficial—evaluate wisely." }
                }
            },

            { 42, new Dictionary<int, string>
                {
                    { 1, "Small efforts lead to great benefits—take action." },
                    { 2, "Support from others strengthens progress." },
                    { 3, "Abundance must be shared to be truly valuable." },
                    { 4, "A shift in circumstances brings unexpected gain." },
                    { 5, "Prosperity comes through integrity and wisdom." },
                    { 6, "Rapid expansion must be managed carefully." }
                }
            },

            { 43, new Dictionary<int, string>
                {
                    { 1, "A bold step forward requires confidence and clarity." },
                    { 2, "Standing firm in truth brings challenges but is necessary." },
                    { 3, "Old patterns must be released for new growth." },
                    { 4, "Patience is required—sudden actions may backfire." },
                    { 5, "A great breakthrough is near—stay prepared." },
                    { 6, "Beware of acting out of frustration—wait for the right time." }
                }
            },

            { 44, new Dictionary<int, string>
                {
                    { 1, "Unexpected encounters bring both opportunity and risk." },
                    { 2, "Remain cautious—appearances may be deceiving." },
                    { 3, "A sudden influence may disrupt plans—stay grounded." },
                    { 4, "A powerful force is approaching—handle it wisely." },
                    { 5, "Stay true to your path despite external pressures." },
                    { 6, "Control over outside influences is essential for balance." }
                }
            },

            { 45, new Dictionary<int, string>
                {
                    { 1, "Community and unity bring strength—seek allies." },
                    { 2, "A shared vision leads to success—cooperate wisely." },
                    { 3, "Doubt weakens unity—trust in collective strength." },
                    { 4, "Aligning with the right people brings prosperity." },
                    { 5, "A leader must act with wisdom and fairness." },
                    { 6, "Chaos results from lack of clear guidance—step up." }
                }
            },

            { 46, new Dictionary<int, string>
                {
                    { 1, "Steady progress leads to long-term success—persist." },
                    { 2, "Gaining momentum—continue building on your efforts." },
                    { 3, "A slow but sure approach is best—do not rush." },
                    { 4, "Opportunities arise through perseverance and trust." },
                    { 5, "Hard work brings rewards—stay dedicated." },
                    { 6, "The summit is near—maintain focus and discipline." }
                }
            },

            { 47, new Dictionary<int, string>
                {
                    { 1, "Challenges are temporary—resilience is key." },
                    { 2, "Inner strength is your greatest asset in difficult times." },
                    { 3, "A time of struggle—seek wisdom and patience." },
                    { 4, "Endurance leads to eventual success." },
                    { 5, "Trust in your own ability to overcome adversity." },
                    { 6, "Do not lose hope—relief is near." }
                }
            },

            { 48, new Dictionary<int, string>
                {
                    { 1, "A deep resource of wisdom is available—tap into it." },
                    { 2, "Seek guidance from those with experience." },
                    { 3, "Neglecting knowledge leads to wasted potential." },
                    { 4, "The well must be maintained—nurture your inner self." },
                    { 5, "True wisdom is timeless—learn from the past." },
                    { 6, "A full well benefits everyone—share your insights." }
                }
            },

            { 49, new Dictionary<int, string>
                {
                    { 1, "Revolution must begin from within." },
                    { 2, "Sudden change can be destabilizing—prepare wisely." },
                    { 3, "Transformation is inevitable—embrace it with courage." },
                    { 4, "A shift in perspective leads to major breakthroughs." },
                    { 5, "Gradual change is more sustainable than abrupt shifts." },
                    { 6, "Completion of transformation—step into the new." }
                }
            },

            { 50, new Dictionary<int, string>
                {
                    { 1, "Nourishment must be carefully prepared." },
                    { 2, "Wisdom is refined through experience." },
                    { 3, "Balance spiritual and material pursuits." },
                    { 4, "Your talents are valuable—use them wisely." },
                    { 5, "Caring for others enhances your own growth." },
                    { 6, "The cauldron is full—great success is at hand." }
                }
            },

            { 51, new Dictionary<int, string>
                {
                    { 1, "A sudden shock awakens new awareness." },
                    { 2, "Fear must be met with courage and wisdom." },
                    { 3, "Disruptions lead to personal growth." },
                    { 4, "Unexpected events reveal hidden strengths." },
                    { 5, "A moment of crisis brings new clarity." },
                    { 6, "Mastering fear leads to true power." }
                }
            },

            { 52, new Dictionary<int, string>
                {
                    { 1, "Stillness brings inner peace—pause and reflect." },
                    { 2, "Movement is not always necessary—wait for the right moment." },
                    { 3, "Resisting change causes frustration—flow with nature." },
                    { 4, "A calm mind is a powerful tool." },
                    { 5, "Deep meditation leads to wisdom and insight." },
                    { 6, "True mastery comes from inner stability." }
                }
            },

            { 53, new Dictionary<int, string>
                {
                    { 1, "Progress is gradual—trust the natural flow of growth." },
                    { 2, "Steady advancement brings lasting success." },
                    { 3, "Patience is required—do not rush the process." },
                    { 4, "Adaptation is necessary for continued growth." },
                    { 5, "Consistency and dedication lead to mastery." },
                    { 6, "Long-term vision brings fulfillment—stay committed." }
                }
            },

            { 54, new Dictionary<int, string>
                {
                    { 1, "Adjusting to new circumstances requires humility." },
                    { 2, "Compromise is necessary, but maintain self-respect." },
                    { 3, "A shift in dynamics requires careful navigation." },
                    { 4, "Balance obligations with personal needs." },
                    { 5, "Recognize your limits and act accordingly." },
                    { 6, "Accepting reality brings peace—avoid resistance." }
                }
            },

            { 55, new Dictionary<int, string>
                {
                    { 1, "A time of abundance—seize the moment." },
                    { 2, "Success is within reach, but remain diligent." },
                    { 3, "Opportunities arise—act with confidence." },
                    { 4, "Avoid excess—moderation ensures sustainability." },
                    { 5, "Generosity strengthens prosperity." },
                    { 6, "Maintain balance to avoid sudden decline." }
                }
            },

            { 56, new Dictionary<int, string>
                {
                    { 1, "A journey begins—embrace the unknown." },
                    { 2, "Adaptability is key in unfamiliar situations." },
                    { 3, "Remain observant—new insights emerge." },
                    { 4, "Avoid attachment—detachment leads to wisdom." },
                    { 5, "New experiences bring transformation." },
                    { 6, "Do not wander aimlessly—find purpose in movement." }
                }
            },

            { 57, new Dictionary<int, string>
                {
                    { 1, "Gentle influence is more effective than force." },
                    { 2, "Patience and persistence lead to success." },
                    { 3, "A subtle approach yields better results." },
                    { 4, "Stay flexible and adapt to changing circumstances." },
                    { 5, "True power lies in quiet determination." },
                    { 6, "Clarity emerges when you trust the process." }
                }
            },

            { 58, new Dictionary<int, string>
                {
                    { 1, "Joy comes from within—cultivate inner peace." },
                    { 2, "Expressing gratitude brings happiness." },
                    { 3, "Encourage positive connections with others." },
                    { 4, "Seek harmony in relationships and communication." },
                    { 5, "A joyful heart attracts good fortune." },
                    { 6, "True contentment arises from simplicity." }
                }
            },

            { 59, new Dictionary<int, string>
                {
                    { 1, "Breaking through stagnation restores flow." },
                    { 2, "Dissolve obstacles with patience and clarity." },
                    { 3, "Letting go of resistance allows movement." },
                    { 4, "Unifying efforts bring resolution." },
                    { 5, "A fresh perspective leads to solutions." },
                    { 6, "Separation is necessary for new beginnings." }
                }
            },

            { 60, new Dictionary<int, string>
                {
                    { 1, "Boundaries provide structure—respect limits." },
                    { 2, "Moderation is key—avoid extremes." },
                    { 3, "Self-discipline leads to stability." },
                    { 4, "Recognizing limitations fosters growth." },
                    { 5, "True freedom is found within constraints." },
                    { 6, "Rigid rules may hinder progress—balance is essential." }
                }
            },

            { 61, new Dictionary<int, string>
                {
                    { 1, "Inner truth brings clarity—trust your intuition." },
                    { 2, "Honest connections create harmony." },
                    { 3, "Sincerity dissolves misunderstandings." },
                    { 4, "A genuine heart attracts the right people." },
                    { 5, "Deep insight leads to wisdom." },
                    { 6, "Spiritual alignment brings fulfillment." }
                }
            },

            { 62, new Dictionary<int, string>
                {
                    { 1, "Small efforts build towards great achievements." },
                    { 2, "Focus on details—precision is crucial." },
                    { 3, "Avoid overextending—progress steadily." },
                    { 4, "Attention to minor aspects leads to mastery." },
                    { 5, "Patience in small tasks leads to larger rewards." },
                    { 6, "Over-focusing on small things can lead to stagnation—balance is needed." }
                }
            },

            { 63, new Dictionary<int, string>
                {
                    { 1, "Completion is near—stay vigilant in the final steps." },
                    { 2, "Success requires ongoing effort." },
                    { 3, "Be mindful—complacency can lead to setbacks." },
                    { 4, "A cycle is ending—prepare for what comes next." },
                    { 5, "Balance is achieved—sustain it wisely." },
                    { 6, "Even after success, adaptability remains key." }
                }
            },

            { 64, new Dictionary<int, string>
                {
                    { 1, "Completion is near, but caution is needed." },
                    { 2, "Patience is required—do not rush the final steps." },
                    { 3, "Details matter. A small mistake could cause setbacks." },
                    { 4, "The transition is unclear—stay flexible." },
                    { 5, "The goal is in sight. Persevere to the end." },
                    { 6, "Success is achieved, but maintaining it requires awareness." }
                }
            }
        };

        private static ELineType[] ChangingLineTypes =>
            new ELineType[] { ELineType.ChangingYang, ELineType.ChangingYin }.ToArray();

        [ScriptIgnore]
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
            var info = HexagramDetails.ContainsKey(Number) ? HexagramDetails[Number] : null;
            if (info != null && GetChangingLines().Any())
            {
                info.ChangingLinesInterpretation = GetChangingLinesInterpretation();
            }

            return info;
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

        private ELineType[] GetChangingLines() => Lines.Where(e => ChangingLineTypes.Contains(e)).ToArray();

        private string GetChangingLinesInterpretation()
        {
            var html = new StringBuilder();

            for (int i = 0; i < Lines.Length; i++)
            {
                var line = Lines[i];
                var lineNumber = i + 1;
                if (ChangingLineTypes.Contains(line))
                {
                    if (ChangingLineInterpretations.ContainsKey(Number) &&
                        ChangingLineInterpretations[Number].ContainsKey(lineNumber))
                    {
                        var interpretation = ChangingLineInterpretations[Number][lineNumber];
                        var title = TemplateProcessor.PopulateTemplate(
                            Globalisation.Dictionary.ChangingLineInterpretation, new
                            {
                                LineNumber = lineNumber
                            });
                        html.AppendLine($"<h5>{title}</h5>");
                        html.AppendLine($"<p>{interpretation}</p>");
                    }
                }
            }

            return html.ToString();
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