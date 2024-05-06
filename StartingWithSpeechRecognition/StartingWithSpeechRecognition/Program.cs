using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace StartingWithSpeechRecognition
{
    class Program
    {
        MusicPlayer mPlayer = new MusicPlayer();
        static SpeechRecognitionEngine _recognizer = null;
        static ManualResetEvent manualResetEvent = null;
        static void Main(string[] args)
        {
            manualResetEvent = new ManualResetEvent(false);
            SpeechRecognitionWithDictationGrammar();
            manualResetEvent.WaitOne();
            
            if (_recognizer != null)
            {
                _recognizer.Dispose();
            }

            Console.WriteLine("Press any key to continue . . .");
            Console.ReadKey(true);
        }
        #region Recognize speech and write to console
        static void RecognizeSpeechAndWriteToConsole()
        {
            _recognizer = new SpeechRecognitionEngine();
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("test"))); // load a "test" grammar
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("exit"))); // load a "exit" grammar
            _recognizer.SpeechRecognized += _recognizeSpeechAndWriteToConsole_SpeechRecognized; // if speech is recognized, call the specified method
            _recognizer.SpeechRecognitionRejected += _recognizeSpeechAndWriteToConsole_SpeechRecognitionRejected; // if recognized speech is rejected, call the specified method
            _recognizer.SetInputToDefaultAudioDevice(); // set the input to the default audio device
            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // recognize speech asynchronous

        }
        static void _recognizeSpeechAndWriteToConsole_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "test")
            {
                Console.WriteLine("test");
            }
            else if (e.Result.Text == "exit")
            {
                manualResetEvent.Set();
            }
        }
        static void _recognizeSpeechAndWriteToConsole_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Console.WriteLine("Speech rejected. Did you mean:");
            foreach (RecognizedPhrase r in e.Result.Alternates)
            {
                Console.WriteLine("    " + r.Text);
            }
        }
        #endregion

        #region Recognize speech and make sure the computer speaks to you (text to speech)
        static void RecognizeSpeechAndMakeSureTheComputerSpeaksToYou()
        {
            _recognizer = new SpeechRecognitionEngine();
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("hello computer"))); // load a "hello computer" grammar
            _recognizer.SpeechRecognized += _recognizeSpeechAndMakeSureTheComputerSpeaksToYou_SpeechRecognized; // if speech is recognized, call the specified method
            _recognizer.SpeechRecognitionRejected += _recognizeSpeechAndMakeSureTheComputerSpeaksToYou_SpeechRecognitionRejected;
            _recognizer.SetInputToDefaultAudioDevice(); // set the input to the default audio device
            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // recognize speech asynchronous
        }
        static void _recognizeSpeechAndMakeSureTheComputerSpeaksToYou_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "hello computer")
            {
                SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();
                speechSynthesizer.Speak("hello user");
                speechSynthesizer.Dispose();
            }
            manualResetEvent.Set();
        }
        static void _recognizeSpeechAndMakeSureTheComputerSpeaksToYou_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            if (e.Result.Alternates.Count == 0)
            {
                Console.WriteLine("No candidate phrases found.");
                return;
            }
            Console.WriteLine("Speech rejected. Did you mean:");
            foreach (RecognizedPhrase r in e.Result.Alternates)
            {
                Console.WriteLine("    " + r.Text);
            }
        }
        #endregion

        #region Emulate speech recognition
        static void EmulateRecognize()
        {
            _recognizer = new SpeechRecognitionEngine();
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("emulate speech"))); // load "emulate speech" grammar
            _recognizer.SpeechRecognized += _emulateRecognize_SpeechRecognized;

            _recognizer.EmulateRecognize("emulate speech");

        }
        static void _emulateRecognize_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text == "emulate speech")
            {
                Console.WriteLine("Speech was emulated!");
            }
            manualResetEvent.Set();
        }
        #endregion

        #region Speech recognition with Choices and GrammarBuilder.Append
        static void SpeechRecognitionWithChoices()
        {
            _recognizer = new SpeechRecognitionEngine();
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append("I"); // add "I"
            grammarBuilder.Append(new Choices("like", "dislike")); // load "like" & "dislike"
            grammarBuilder.Append(new Choices("dogs", "cats", "birds", "snakes", "fishes", "tigers", "lions", "snails", "elephants")); // add animals
            _recognizer.LoadGrammar(new Grammar(grammarBuilder)); // load grammar
            _recognizer.SpeechRecognized += speechRecognitionWithChoices_SpeechRecognized;
            _recognizer.SetInputToDefaultAudioDevice(); // set input to default audio device
            _recognizer.RecognizeAsync(RecognizeMode.Multiple); // recognize speech
        }

        static void speechRecognitionWithChoices_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("Do you really " + e.Result.Words[1].Text + " " + e.Result.Words[2].Text + "?");
            manualResetEvent.Set();
        }
        #endregion

        #region Speech recognition with DictationGrammar
        static void SpeechRecognitionWithDictationGrammar()
        {
            _recognizer = new SpeechRecognitionEngine();
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("exit")));
            _recognizer.LoadGrammar(new DictationGrammar());
            _recognizer.SpeechRecognized += speechRecognitionWithDictationGrammar_SpeechRecognized;
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        static void speechRecognitionWithDictationGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Speaker sp = new Speaker();

            String r = e.Result.Text.ToUpper();
            Console.WriteLine("Dijiste: " + r);
            switch(r){
                case "ESCUCHAR MÚSICA":
                    sp.Speak("Reproduciendo canción");
                    Console.WriteLine("[SYSTEM] Reproduciendo canción...\n");
                    Thread.Sleep(500);
                    MusicPlayer.PlayMusic();
                    break;
                case "PARA LA MÚSICA":
                    sp.Speak("Pausando canción");
                    Console.WriteLine("[SYSTEM] Pausando canción...\n");
                    Thread.Sleep(500);
                    MusicPlayer.PauseMusic();
                    break;
                case "TERMINAR LA MÚSICA":
                    sp.Speak("Deteniendo canción");
                    Console.WriteLine("[SYSTEM] Deteniendo canción...\n");
                    Thread.Sleep(500);
                    MusicPlayer.StopMusic();
                    break;
                case "SIGUIENTE CANCIÓN":
                    sp.Speak("Reproduciendo siguiente canción");
                    Console.WriteLine("[SYSTEM] Reproduciendo siguiente canción...\n");
                    Thread.Sleep(500);
                    MusicPlayer.StopMusic();
                    MusicPlayer.NextSong();
                    break;
                case "ANTERIOR CANCIÓN":
                    sp.Speak("Reproduciendo anterior canción");
                    Console.WriteLine("[SYSTEM] Reproduciendo anterior canción...\n");
                    Thread.Sleep(500);
                    MusicPlayer.PreviousSong();
                    break;
                case "SUBE VOLUMEN":
                    sp.Speak("Subiendo volumen");
                    Console.WriteLine("[SYSTEM] Subiendo volumen...\n");
                    Thread.Sleep(500);
                    VolumeManage.VolUp();
                    break;
                case "BAJA VOLUMEN":
                    sp.Speak("Bajando volumen");
                    Console.WriteLine("[SYSTEM] Bajando volumen...\n");
                    Thread.Sleep(500);
                    VolumeManage.VolDown();
                    break;
                case "SILENCIO":
                    sp.Speak("Silenciando canción");
                    Console.WriteLine("[SYSTEM] Silenciando canción...\n");
                    Thread.Sleep(500);
                    VolumeManage.Mute();
                    break;
                case "DIME LA HORA":
                    sp.Speak("La hora es " + DateTime.Now.ToString());
                    Console.WriteLine("[SYSTEM] " + DateTime.Now.ToString() + "\n");
                    break;
                case "CHAU":
                sp.Speak("Adiós");
                 manualResetEvent.Set();
                 Console.WriteLine("[SYSTEM] Adiós...\n");
                 return;
                case "CHAO":
                 sp.Speak("Adiós");
                 manualResetEvent.Set();
                 Console.WriteLine("[SYSTEM] Adiós...\n");
                return;
                default:
                sp.Speak("No entendí, ¿podrías repetirlo?");
                   Console.WriteLine("[SYSTEM] No entendí, ¿podrías repetirlo?\n");
                    break;

            }
            
        }
        #endregion

        #region Prompt building
        static void PromptBuilding()
        {
            PromptBuilder builder = new PromptBuilder();

            builder.StartSentence();
            builder.AppendText("This is a prompt building example.");
            builder.EndSentence();

            builder.StartSentence();
            builder.AppendText("Now, there will be a break of 2 seconds.");
            builder.EndSentence();

            builder.AppendBreak(new TimeSpan(0, 0, 2));

            builder.StartStyle(new PromptStyle(PromptVolume.ExtraSoft));
            builder.AppendText("This text is spoken extra soft.");
            builder.EndStyle();

            builder.StartStyle(new PromptStyle(PromptRate.Fast));
            builder.AppendText("This text is spoken fast.");
            builder.EndStyle();

            SpeechSynthesizer synthesizer = new SpeechSynthesizer();
            synthesizer.Speak(builder);
            synthesizer.Dispose();
        }
        #endregion

    }
}
