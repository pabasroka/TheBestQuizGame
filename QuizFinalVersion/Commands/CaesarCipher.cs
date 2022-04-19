using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizFinalVersion.Interfaces;

namespace QuizFinalVersion.Commands
{
    internal class CaesarCipher : IDecode
    {
        private const int Shift = 1;

        public string Decode(string codedString)
        {
            var stringArray = codedString.ToCharArray();

            for (var i = 0; i < stringArray.Length; i++)
            {
                stringArray[i] = (char)(stringArray[i] - Shift);
            }

            return new string(stringArray);
        }
    }
}
