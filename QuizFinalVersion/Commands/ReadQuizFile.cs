using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QuizFinalVersion.Interfaces;

namespace QuizFinalVersion.Commands
{
    internal class ReadQuizFile<T> : CaesarCipher
    {
        public T ReadJsonFile(string fileName = "quiz.json")
        {
            var reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), fileName));
            var json = reader.ReadToEnd();
            var decodedJson = Decode(json);
            return JsonConvert.DeserializeObject<T>(decodedJson);
        }
    }
}
