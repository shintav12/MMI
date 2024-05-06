using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace StartingWithSpeechRecognition
{
    public class Speaker
    {
        private SpeechSynthesizer reader;
        public Speaker()
        {
            reader = new SpeechSynthesizer();
        }

        public void Speak(String text)
        {
            reader.Dispose();
            reader = new SpeechSynthesizer();
            reader.SpeakAsync(text);
            //reader.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(reader_SpeakCompleted);
        }
    }
}
