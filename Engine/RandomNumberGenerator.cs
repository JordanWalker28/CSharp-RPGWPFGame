using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace Engine
{
    public static class RandomNumberGenerator
    {
        private static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();

        public static int NumberBetween(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];
            _generator.GetBytes(randomNumber);

            double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

            double multiplyer = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);

            int range = maximumValue - minimumValue + 1;

            double randomValueInRange = Math.Floor(multiplyer * range);

            return (int)(minimumValue + randomValueInRange);

        }
    }
}
