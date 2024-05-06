using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace StartingWithSpeechRecognition
{
    public class VolumeManage
    {
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);

        public static void Mute()
        {
            
            SendMessageW(GetConsoleWindow(), WM_APPCOMMAND, GetConsoleWindow(),
                (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }

        public static void VolDown()
        {
            for (int i = 0; i < 10;i++ )
            {
                SendMessageW(GetConsoleWindow(), WM_APPCOMMAND, GetConsoleWindow(),
                    (IntPtr)APPCOMMAND_VOLUME_DOWN);
            }
                
        }

        public static void VolUp()
        {
            for (int i = 0; i < 10;i++ )
            {
                SendMessageW(GetConsoleWindow(), WM_APPCOMMAND, GetConsoleWindow(),
                    (IntPtr)APPCOMMAND_VOLUME_UP);
            }
                
        }
    }
}
