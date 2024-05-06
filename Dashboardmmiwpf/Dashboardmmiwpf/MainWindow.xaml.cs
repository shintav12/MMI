using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.ComponentModel;
using System.Speech.Recognition;
using Microsoft.Kinect.Wpf.Controls;
using Microsoft.Kinect;
using System.Threading;
using KinectV2MouseControl;
using WpfKb.Controls;
using MahApps.Metro.Controls.Dialogs;


namespace Dashboardmmiwpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {

        KinectControl kinectCtrl = new KinectControl();

        //SpeechRecognitionEngine _recognize;
        //public static BackgroundWorker bw;
        //private FloatingTouchScreenKeyboard VKeyboard = new FloatingTouchScreenKeyboard();
        private static bool Locked = false;
        public static Speaker sp;
        public static MetroWindow windoww;
        public static SpeechRecognitionEngine _recognizer;
        public static string page = "MainWindow";

        public MainWindow()
        {
            InitializeComponent();
            page = "MainWindow";
            _recognizer = new SpeechRecognitionEngine();
            sp = new Speaker();
            //Mouse.OverrideCursor = Cursors.None;
            //_recognize = new SpeechRecognitionEngine();
            //_recognize.SpeechRecognized += speechRecognize_SpeechRecognized;
            //GrammarBuilder grammarBuilder = new GrammarBuilder();
            //Choices commandChoices = new Choices("abrir", "cerrar");
            //grammarBuilder.Append(commandChoices);
            Mouse.OverrideCursor = ((FrameworkElement)this.Resources["OpenHandCursor"]).Cursor;
            windoww = this;
            sp.Speak("Soy didactive, en el momento que guste ingrese los comandos de voz y yo lo atenderé");
            //VKeyboard.IsOpen = false;
            //VKeyboard.Width = 1100;
            //VKeyboard.Height = 450;
            //VKeyboard.Placement = System.Windows.Controls.Primitives.PlacementMode.Center;
            //VKeyboard.AreAnimationsEnabled = true;
            //VKeyboard.PlacementTarget = this;
            //Choices valueChoices = new Choices();
            //valueChoices.Add("teclado");
            //valueChoices.Add("teclado");
            //grammarBuilder.Append(valueChoices);

            //_recognize.LoadGrammar(new Grammar(grammarBuilder));
            //_recognize.SetInputToDefaultAudioDevice();
            //_recognize.RecognizeAsync(RecognizeMode.Multiple);
            DashboardLista MenuPrincipal = new DashboardLista();
            frame.NavigationService.Navigate(MenuPrincipal);    

        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            
            Point mouse = e.GetPosition(frame);

            if (mouse.X > 0 && mouse.X < 10)
                Menu.IsOpen = true;

            if (Menu.IsOpen && mouse.X > 320)
                Menu.IsOpen = false;

        }

        public static async void AlertaExito()
        {
            var controller = await windoww.ShowProgressAsync("Operación Exitosa", "Se realizó la operación con éxito");
            controller.Minimum = 1;
            controller.Maximum = 5;

            await Task.Delay(5000);

            await controller.CloseAsync();

        }

        public static async void testConnection(string server, string database)
        {
            var controller = await windoww.ShowProgressAsync("Probando Conexión", "Prueba de conexión en proceso");

            MainWindow.sp.Speak("Prueba de conexión en proceso");

            await Task.Delay(5000);

            Conexion conexion = new Conexion();
            bool status = conexion.testconnection(server, database);

            await controller.CloseAsync();

            if (status)
            {
                sp.Speak("Conexión exitosa");
                AlertaCustom("Conexión exitosa", "La conexión a la base de datos se ha realizado con éxito");
            }
            else
            {
                sp.Speak("Conexión erronea");
                AlertaCustom("Conexión Erronea", "Hubo un problema con al conexión a la base de datos");
            }

        }

        public static async void AlertaError()
        {
            var controller = await windoww.ShowProgressAsync("Operación Fallida", "Ocurrió un problema mientras se procesaba tu solicitud");
            controller.Minimum = 1;
            controller.Maximum = 5;

            await Task.Delay(5000);

            await controller.CloseAsync();

        }

        public static async void AlertaFaltanDatos()
        {
            var controller = await windoww.ShowMessageAsync("Completa todos los datos", "Recuerda que todos los datos del formulario son importantes");

        }

        public static async void AlertaFaltanDatos(String mensaje)
        {
            var controller = await windoww.ShowMessageAsync("Completa todos los datos", mensaje);

        }


        public static async void AlertaCustom(String mensajegrande, String mensajeChico)
        {
            var controller = await windoww.ShowMessageAsync(mensajegrande, mensajeChico);

        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncCancel();
            sp.Speak("Lista de Conexiones");
            DataSourceLista MenuPrincipal = new DataSourceLista();
            frame.NavigationService.Navigate(MenuPrincipal);

        }


        private void Tile_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= DataSourceLista.speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncCancel();
            sp.Speak("Lista de dashboards");
            DashboardLista MenuPrincipal = new DashboardLista();
            frame.NavigationService.Navigate(MenuPrincipal);

        }

        private void Tile_Click_2(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
