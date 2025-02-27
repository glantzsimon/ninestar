using K9.WebApplication.Enums;
using System.Linq;

namespace K9.WebApplication.Extensions
{
    public static  class NumerologyExtensions
    {
        public static ENumerologyCode Increment(this ENumerologyCode code, int value = 1)
        {
            return (ENumerologyCode)((int)code).Increment(value);
        }

        public static ENumerologyCode Decrement(this ENumerologyCode code, int value = 1)
        {
            return (ENumerologyCode)((int)code).Decrement(value);
        }

        public static int Increment(this int code, int value = 1)
        {
            for (int i = 0; i < value; i++)
            {
                code = code == 9 ? 1 : code + 1;
            }
            return code;
        }

        public static int Decrement(this int code, int value = 1)
        {
            for (int i = 0; i < value; i++)
            {
                code = code == 1 ? 9 : code - 1;
            }
            return code;
        }

        public static int ToNumerology(this int value)
        {
            if (value < 0)
            {
                value = value + 9;
            }

            if (value == 0)
            {
                return 0;
            }

            var result = 0;
            while (result >= 10 || result == 0)
            {
                if (result == 0)
                {
                    result = value;
                }

                result = result.ToString().Select(e => int.Parse(e.ToString())).Sum();
            }

            return result;
        }
    }
}
