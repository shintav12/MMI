using System.Speech.Recognition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfKb.Controls;

namespace Dashboardmmiwpf
{
    /// <summary>
    /// Interaction logic for DashboardLista.xaml
    /// </summary>
    public partial class DashboardLista : Page
    {
       
        private FloatingTouchScreenKeyboard VKeyboard = new FloatingTouchScreenKeyboard();
        public DashboardLista()
        {
            InitializeComponent();
            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("navegar", "nuevo");
            grammarBuilder.Append(commandChoices);
            Choices valueChoices = new Choices();
            valueChoices.Add("conexiones");
            valueChoices.Add("grupo", "dashboard");
            grammarBuilder.Append(valueChoices);

            MainWindow._recognizer.UnloadAllGrammars();
            MainWindow._recognizer.LoadGrammar(new Grammar(grammarBuilder));
            MainWindow._recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.sp.Speak("Ingrese los datos del nuevo Dashboard");
            NewDashboard MenuPrincipal = new NewDashboard();
            this.NavigationService.Navigate(MenuPrincipal);
        }
        
        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouse = e.GetPosition(this);
            if (mouse.Y > 800 && Actions.IsOpen == false)
                Actions.IsOpen = true;
            else if (Actions.IsOpen && mouse.Y < 800)
                Actions.IsOpen = false;
        }

        private void Tile_Click_2(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.sp.Speak("Ingrese los datos del nuevo grupo");
            NuevoDashboard MenuPrincipal = new NuevoDashboard();
            this.NavigationService.Navigate(MenuPrincipal);
        }

        public static void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
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
                            case "conexiones":
                                MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
                                MainWindow._recognizer.RecognizeAsyncCancel();
                                MainWindow.sp.Speak("Lista de Conexiones");
                                DataSourceLista listadatasource = new DataSourceLista();
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
                    case "nuevo":
                        switch (value)
                        {
                            case "dashboard":
                                MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
                                MainWindow._recognizer.RecognizeAsyncCancel();
                                MainWindow.sp.Speak("Ingreso los datos del nuevo Cuadro de Mando");
                                NewDashboard listaashboard = new NewDashboard();
                                foreach (Window window in Application.Current.Windows)
                                {
                                    if (window.GetType() == typeof(MainWindow))
                                    {
                                        (window as MainWindow).frame.NavigationService.Navigate(listaashboard);
                                        break;
                                    }
                                }
                                break;
                            case "grupo":
                                MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
                                MainWindow._recognizer.RecognizeAsyncCancel();
                                MainWindow.sp.Speak("Ingrese los datos del nuevo Grupo");
                                NuevoDashboard listadatasource = new NuevoDashboard();
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
