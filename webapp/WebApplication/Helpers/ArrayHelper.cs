using System;

namespace K9.WebApplication.Helpers
{
    public enum ExpansionType
    {
        ReduceToLower,
        ReduceToHigher,
        ReduceToAverage,
        ExpandFromMinimum,
        ExpandFromMaximum,
        ExpandFromAverage,
        None
    }

    public static class ArrayHelper
    {
        public static double[] ManipulateArray(double[] inputArray, double modifyRangeBy, ExpansionType expansionType)
        {
            double minimum = inputArray[0];
            double maximum = inputArray[0];
            double sum = 0;

            // Find minimum, maximum, and sum of input array
            for (int i = 0; i < inputArray.Length; i++)
            {
                minimum = Math.Min(minimum, inputArray[i]);
                maximum = Math.Max(maximum, inputArray[i]);
                sum += inputArray[i];
            }

            double average = sum / inputArray.Length;
            double range = maximum - minimum;

            if (range == 0)
            {
                // Given array has only one value, no manipulation required
                return inputArray;
            }

            double[] manipulatedArray = new double[inputArray.Length];

            for (int i = 0; i < inputArray.Length; i++)
            {
                double originalValue = inputArray[i];
                double normalizedValue = (originalValue - minimum) / range;  // Normalize value between 0 and 1

                // Manipulate the normalized value based on the expansion type
                double manipulatedValue;
                if (expansionType == ExpansionType.ReduceToLower)
                {
                    manipulatedValue = normalizedValue * (1 - modifyRangeBy);  // Reduce lower values
                }
                else if (expansionType == ExpansionType.ReduceToHigher)
                {
                    manipulatedValue = modifyRangeBy + (1 - modifyRangeBy) * normalizedValue;  // Reduce higher values
                }
                else if (expansionType == ExpansionType.ReduceToAverage)
                {
                    manipulatedValue = Math.Abs(normalizedValue - 0.5) * (1 - modifyRangeBy) + 0.5;  // Reduce values towards average
                }
                else if (expansionType == ExpansionType.ExpandFromMinimum)
                {
                    manipulatedValue = normalizedValue * modifyRangeBy;  // Expand values from minimum
                }
                else if (expansionType == ExpansionType.ExpandFromMaximum)
                {
                    manipulatedValue = modifyRangeBy + (1 - modifyRangeBy) * normalizedValue;  // Expand values from maximum
                }
                else if (expansionType == ExpansionType.ExpandFromAverage)
                {
                    manipulatedValue = (normalizedValue - 0.5) * modifyRangeBy + 0.5;  // Expand values from average
                }
                else
                {
                    manipulatedValue = normalizedValue;  // No manipulation, return original value
                }

                manipulatedArray[i] = manipulatedValue * range + minimum;  // Unnormalize the value
            }

            return manipulatedArray;
        }
    }
}