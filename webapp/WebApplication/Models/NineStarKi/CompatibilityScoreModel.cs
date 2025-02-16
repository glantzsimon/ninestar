using K9.WebApplication.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace K9.WebApplication.Models
{
    public class CompatibilityScoreModel
    {
        public CompatibilityScoreModel()
        {
            HarmonyScores = new List<ECompatibilityScore>();
            ConflictScores = new List<ECompatibilityScore>();
            SupportScores = new List<ECompatibilityScore>();
            MutualUnderstandingScores = new List<ECompatibilityScore>();
            ComplementarityScores = new List<ECompatibilityScore>();
            SexualChemistryScores = new List<ESexualChemistryScore>();
            SparkScores = new List<ECompatibilityScore>();
            LearningPotentialScores = new List<ECompatibilityScore>();
            SupportiveScores = new List<ECompatibilityScore>();
            SameScores = new List<ECompatibilityScore>();
            ChallengingScores = new List<ECompatibilityScore>();
        }

        [ScriptIgnore]
        public double SupportiveScoreAsPercentage { get; set; }

        [ScriptIgnore]
        public double SameScoreAsPercentage { get; set; }

        [ScriptIgnore]
        public double ChallengingAsPercentage { get; set; }

        public ECompatibilityScore HarmonyScore { get; set; }

        public ECompatibilityScore ConflictScore { get; set; }

        public ECompatibilityScore SupportScore { get; set; }

        public ECompatibilityScore MutualUnderstandingScore { get; set; }

        public ECompatibilityScore ComplementarityScore { get; set; }

        public ESexualChemistryScore SexualChemistryScore { get; set; }

        public ECompatibilityScore SparkScore { get; set; }

        public ECompatibilityScore LearningPotentialScore { get; set; }

        [ScriptIgnore]
        public ECompatibilityScore OverallScore { get; set; }

        private List<ECompatibilityScore> HarmonyScores { get; }
        private List<ECompatibilityScore> ConflictScores { get; }
        private List<ECompatibilityScore> SupportScores { get; }
        private List<ECompatibilityScore> MutualUnderstandingScores { get; }
        private List<ECompatibilityScore> ComplementarityScores { get; }
        private List<ESexualChemistryScore> SexualChemistryScores { get; }
        private List<ECompatibilityScore> SparkScores { get; }
        private List<ECompatibilityScore> LearningPotentialScores { get; }

        private List<ECompatibilityScore> SupportiveScores { get; }
        private List<ECompatibilityScore> SameScores { get; }
        private List<ECompatibilityScore> ChallengingScores { get; }

        public void AddSupportiveScore(ECompatibilityScore score, int factor = 1)
        {
            AddScore(SupportiveScores, score, factor);
        }

        public void AddSameScore(ECompatibilityScore score, int factor = 1)
        {
            AddScore(SameScores, score, factor);
        }

        public void AddChallengingScore(ECompatibilityScore score, int factor = 1)
        {
            AddScore(ChallengingScores, score, factor);
        }

        public void AddHarmonyScore(ECompatibilityScore score, int factor = 1)
        {
            AddScore(HarmonyScores, score, factor);
        }

        public void AddConflictScore(ECompatibilityScore score, int factor = 1)
        {
            AddScore(ConflictScores, score, factor);
        }

        public void AddSupportScore(ECompatibilityScore score, int factor = 1)
        {
            AddScore(SupportScores, score, factor);
        }

        public void AddMutualUnderstandingScore(ECompatibilityScore score, int factor = 1)
        {
            AddScore(MutualUnderstandingScores, score, factor);
        }

        public void AddComplementarityScore(ECompatibilityScore score, int factor = 1)
        {
            AddScore(ComplementarityScores, score, factor);
        }

        public void AddSparkScore(ECompatibilityScore score, int factor = 1)
        {
            AddScore(SparkScores, score, factor);
        }

        public void AddLearningPotentialScore(ECompatibilityScore score, int factor = 1)
        {
            AddScore(LearningPotentialScores, score, factor);
        }

        public void AddSexualChemistryScore(ESexualChemistryScore score, int factor = 1)
        {
            for (int i = 0; i < factor; i++)
            {
                SexualChemistryScores.Add(score);
            }
        }

        public void CalculateAverages()
        {
            GetAverageScoreAsPercentage();

            HarmonyScore = GetAverageScore(HarmonyScores);
            ConflictScore = GetAverageScore(ConflictScores);
            SupportScore = GetAverageScore(SupportScores);
            MutualUnderstandingScore = GetAverageScore(MutualUnderstandingScores);
            ComplementarityScore = GetAverageScore(ComplementarityScores);
            SexualChemistryScore = (ESexualChemistryScore)Math.Round(SexualChemistryScores.Average(e => (int)e), MidpointRounding.AwayFromZero);
            SparkScore = GetAverageScore(SparkScores);
            LearningPotentialScore = GetAverageScore(LearningPotentialScores);
            OverallScore = GetAverageScore(new List<ECompatibilityScore>
            {
                HarmonyScore,
                SupportScore,
                MutualUnderstandingScore,
                (int)ECompatibilityScore.ExtremelyHigh - ConflictScore,
                ComplementarityScore,
                (ECompatibilityScore)SexualChemistryScore
            });
        }

        private void AddScore(List<ECompatibilityScore> scores, ECompatibilityScore score, int factor = 1)
        {
            for (int i = 0; i < factor; i++)
            {
                scores.Add(score);
            }
        }

        private ECompatibilityScore GetAverageScore(List<ECompatibilityScore> scores)
        {
            if (scores.Any())
            {
                return (ECompatibilityScore)Math.Round(scores.Average(e => (int)e), MidpointRounding.AwayFromZero);
            }

            return ECompatibilityScore.Unspecified;
        }

        private void GetAverageScoreAsPercentage()
        {
            var supportiveSum = SupportiveScores.Sum(e => (double)e);
            var sameSum = SameScores.Sum(e => (double)e);
            var challengingSum = ChallengingScores.Sum(e => (double)e);

            var total = supportiveSum + sameSum +
                        challengingSum;

            SupportiveScoreAsPercentage = supportiveSum / total * 100;
            SameScoreAsPercentage = sameSum / total * 100;
            ChallengingAsPercentage = challengingSum / total * 100;
        }
    }

}