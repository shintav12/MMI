using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;


namespace StartingWithSpeechRecognition
{
    public class MusicPlayer
    {
        //static List<String> musica;
        static List<String> playList = new List<String>()
	    {
            "H:\\Musica\\RERE.mp3",
            "H:\\Musica\\Toradora.mp3",
            "H:\\Musica\\yowapeda1.mp3",
            "H:\\Musica\\yowapeda2.mp3"
	        
	    };
        private static bool isplaying = false;       
        static int it = 0;

        [DllImport("winmm.dll")]
        private static extern int mciSendString(string MciComando, string MciRetorno, int MciRetornoLeng, int CallBack);

        public static void PlayMusic()
        {
            isplaying = true;
            mciSendString("play " + playList[it], null, 0, 0);
        }

        public static void PauseMusic()
        {
            mciSendString("pause " + playList[it], null, 0, 0);
        }

        public static void StopMusic()
        {
            isplaying = false;
            mciSendString("stop " + playList[it], null, 0, 0);
        }

        public static void NextSong()
        {
            if(isplaying == true)
            {
                mciSendString("stop " + playList[it], null, 0, 0);
                Thread.Sleep(500);
            }
            it++;
            if (it == 10) it = 0;
            PlayMusic();
        }
        public static void PreviousSong()
        {
            if (isplaying == true)
            {
                mciSendString("stop " + playList[it], null, 0, 0);
                Thread.Sleep(500);
            }
            it--;
            if (it == -1) it = 5;
            PlayMusic();
        }

    }
}
