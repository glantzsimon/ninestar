using K9.WebApplication.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

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
        }

        private List<ECompatibilityScore> HarmonyScores { get; }
        private List<ECompatibilityScore> ConflictScores { get; }
        private List<ECompatibilityScore> SupportScores { get; }
        private List<ECompatibilityScore> MutualUnderstandingScores { get; }
        private List<ECompatibilityScore> ComplementarityScores { get; }
        private List<ESexualChemistryScore> SexualChemistryScores { get; }
        private List<ECompatibilityScore> SparkScores { get; }
        private List<ECompatibilityScore> LearningPotentialScores { get; }

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

        private void AddScore(List<ECompatibilityScore> scores, ECompatibilityScore score, int factor = 1)
        {
            for (int i = 0; i < factor; i++)
            {
                scores.Add(score);
            }
        }

        public ECompatibilityScore HarmonyScore => GetAverageScore(HarmonyScores);

        public ECompatibilityScore ConflictScore => GetAverageScore(ConflictScores);

        public ECompatibilityScore SupportScore => GetAverageScore(SupportScores);

        public ECompatibilityScore MutualUnderstandingScore => GetAverageScore(MutualUnderstandingScores);

        public ECompatibilityScore ComplementarityScore => GetAverageScore(ComplementarityScores);

        public ESexualChemistryScore SexualChemistryScore => (ESexualChemistryScore)Math.Round(SexualChemistryScores.Average(e => (int)e), MidpointRounding.AwayFromZero);

        public ECompatibilityScore SparkScore => GetAverageScore(SparkScores);

        public ECompatibilityScore LearningPotentialScore => GetAverageScore(LearningPotentialScores);

        private ECompatibilityScore GetAverageScore(List<ECompatibilityScore> scores)
        {
            if (scores.Any())
            {
                return (ECompatibilityScore)Math.Round(scores.Average(e => (int)e), MidpointRounding.AwayFromZero);
            }

            return ECompatibilityScore.Unspecified;
        }
    }

}