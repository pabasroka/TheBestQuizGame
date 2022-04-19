using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizFinalVersion.Commands
{
    internal class QuizTimer
    {
        public int Seconds { get; set; }
        public string TimerFormatter()
        {
            var time = TimeSpan.FromSeconds(Seconds);
            return time.ToString(@"hh\:mm\:ss");
        }
    }
}
