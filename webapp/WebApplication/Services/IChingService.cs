using K9.WebApplication.Enums;
using K9.WebApplication.Models;
using System;

namespace K9.WebApplication.Services
{

    public class IChingService : IIChingService
    {
        private static readonly Random _random = new Random();
        
        public Hexagram GenerateHexagram()
        {
            var eLines = new ELineType[6];

            for (int i = 0; i < 6; i++)
            {
                eLines[i] = GenerateLine();
            }

            return new Hexagram(eLines);
        }

        private static ELineType GenerateLine()
        {
            int heads = 0;
            for (int i = 0; i < 3; i++)
            {
                if (_random.Next(2) == 1)
                {
                    heads++;
                }
            }

            switch (heads)
            {
                case 3: return ELineType.ChangingYang;
                case 2: return ELineType.Yang;
                case 1: return ELineType.Yin;
                case 0: return ELineType.ChangingYin;
                default: throw new InvalidOperationException("Invalid coin toss result");
            }
        }
    }
}