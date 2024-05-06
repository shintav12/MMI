using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Speech.Recognition;
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
using WpfKb.Controls;

namespace Dashboardmmiwpf
{
    /// <summary>
    /// Interaction logic for DataSourceLsta.xaml
    /// </summary>
    public partial class DataSourceLista : Page
    {
        public DataSourceLista()
        {
            InitializeComponent();
            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("navegar", "nueva");
            grammarBuilder.Append(commandChoices);
            Choices valueChoices = new Choices();
            valueChoices.Add("dashboard");
            valueChoices.Add("conexión", "vista");
            grammarBuilder.Append(valueChoices);

            MainWindow._recognizer.UnloadAllGrammars();
            MainWindow._recognizer.LoadGrammar(new Grammar(grammarBuilder));
            MainWindow._recognizer.RecognizeAsync(RecognizeMode.Multiple);

        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouse = e.GetPosition(this);
            if (mouse.Y > 800 && Actions.IsOpen == false)
                Actions.IsOpen = true;
            else if (Actions.IsOpen && mouse.Y < 800)
                Actions.IsOpen = false;
        }

        private void Tile_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncCancel();
            MainWindow.sp.Speak("Ingrese los datos para la nueva conexión");
            DetalleDatasource detalledatasource = new DetalleDatasource();
            this.NavigationService.Navigate(detalledatasource);
        }

        private void Tile_Click_2(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncCancel();
            MainWindow.sp.Speak("Ingrese los datos para la nueva vista de datos");
            DetalleDataview detalledatasource = new DetalleDataview();
            this.NavigationService.Navigate(detalledatasource);
        }

        public static void  speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Words.Count == 2)
            {
                string command = e.Result.Words[0].Text.ToLower();
                string value = e.Result.Words[1].Text.ToLower();
                switch (command)
                {
                    case "navegar":
                        switch (value)
                        {
                            case "dashboard":
                                MainWindow._recognizer.RecognizeAsyncCancel();
                                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                MainWindow.sp.Speak("Lista de Dashboards");
                                DashboardLista listaashboard = new DashboardLista();
                                foreach (Window window in Application.Current.Windows)
                                {
                                    if (window.GetType() == typeof(MainWindow))
                                    {
                                        (window as MainWindow).frame.NavigationService.Navigate(listaashboard);
                                        break;
                                    }
                                }
                                break;
                        }
                        break;
                    case "nueva":
                        switch (value)
                        {
                            case "conexión":
                                MainWindow._recognizer.RecognizeAsyncCancel();
                                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                MainWindow.sp.Speak("Ingrese los datos para la nueva conexión");
                                DetalleDatasource listaashboard = new DetalleDatasource();
                                foreach (Window window in Application.Current.Windows)
                                {
                                    if (window.GetType() == typeof(MainWindow))
                                    {
                                        (window as MainWindow).frame.NavigationService.Navigate(listaashboard);
                                        break;
                                    }
                                }
                                break;
                            case "vista":
                                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                MainWindow._recognizer.RecognizeAsyncCancel();
                                MainWindow.sp.Speak("Ingrese los datos para la nueva vista de datos");
                                DetalleDataview listadatasource = new DetalleDataview();
                                foreach (Window window in Application.Current.Windows)
                                {
                                    if (window.GetType() == typeof(MainWindow))
                                    {
                                        (window as MainWindow).frame.NavigationService.Navigate(listadatasource);
                                        break;
                                    }
                                }
                                break;
                        }
                        break;
                }
            }
        }
    }
}
