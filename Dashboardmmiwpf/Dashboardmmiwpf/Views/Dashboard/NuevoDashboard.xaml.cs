using System;
using System.Collections.Generic;
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
    /// Interaction logic for NuevoDashboard.xaml
    /// </summary>
    public partial class NuevoDashboard : Page
    {
        private FloatingTouchScreenKeyboard VKeyboard = new FloatingTouchScreenKeyboard();
        private int ProjectId = 0;
        public NuevoDashboard()
        {
            InitializeComponent();

            btnActualizar.Visibility = Visibility.Hidden;
            btnGuardar.Visibility = Visibility.Visible;
            btnGuardar_Copy.Visibility = Visibility.Visible;
            btnEliminar.Visibility = Visibility.Hidden;
            VKeyboard.IsOpen = false;
            VKeyboard.Width = 1100;
            VKeyboard.Height = 450;
            VKeyboard.Placement = System.Windows.Controls.Primitives.PlacementMode.Center;
            VKeyboard.AreAnimationsEnabled = true;
            VKeyboard.PlacementTarget = this;
            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("guardar", "cancelar", "llenar", "abrir", "cerrar");
            grammarBuilder.Append(commandChoices);

            Choices valueChoices = new Choices();
            valueChoices.Add("grupo");
            valueChoices.Add("accion");
            valueChoices.Add("nombredelgrupo");
            valueChoices.Add("teclado");
            grammarBuilder.Append(valueChoices);

            VKeyboard.IsOpen = false;
            VKeyboard.Width = 1100;
            VKeyboard.Height = 450;
            VKeyboard.Placement = System.Windows.Controls.Primitives.PlacementMode.Center;
            VKeyboard.AreAnimationsEnabled = true;
            VKeyboard.PlacementTarget = this;

            MainWindow._recognizer.UnloadAllGrammars();
            MainWindow._recognizer.LoadGrammar(new Grammar(grammarBuilder));
            MainWindow._recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        public NuevoDashboard(int id)
        {
            InitializeComponent();

            Conexion conexion = new Conexion();
            Projects grupo = new Projects();

            grupo = grupo.DataTabletoList(conexion.GetProject(id)).First();

            txtServer.Text = grupo.Nombre;

            btnActualizar.Visibility = Visibility.Visible;
            btnGuardar.Visibility = Visibility.Hidden;
            btnGuardar_Copy.Visibility = Visibility.Hidden;
            btnEliminar.Visibility = Visibility.Visible;
            ProjectId = id;
            VKeyboard.IsOpen = false;
            VKeyboard.Width = 1100;
            VKeyboard.Height = 450;
            VKeyboard.Placement = System.Windows.Controls.Primitives.PlacementMode.Center;
            VKeyboard.AreAnimationsEnabled = true;
            VKeyboard.PlacementTarget = this;
            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("guardar", "cancelar", "llenar", "abrir", "cerrar","eliminar");
            grammarBuilder.Append(commandChoices);

            Choices valueChoices = new Choices();
            valueChoices.Add("grupo");
            valueChoices.Add("accion");
            valueChoices.Add("nombredelgrupo");
            valueChoices.Add("teclado");
            grammarBuilder.Append(valueChoices);

            VKeyboard.IsOpen = false;
            VKeyboard.Width = 1100;
            VKeyboard.Height = 450;
            VKeyboard.Placement = System.Windows.Controls.Primitives.PlacementMode.Center;
            VKeyboard.AreAnimationsEnabled = true;
            VKeyboard.PlacementTarget = this;

            MainWindow._recognizer.UnloadAllGrammars();
            MainWindow._recognizer.LoadGrammar(new Grammar(grammarBuilder));
            MainWindow._recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.sp.Speak("Acción Cancelada");
            DashboardLista datasourcelista = new DashboardLista();
            this.NavigationService.Navigate(datasourcelista);
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (validar())
            {
                Conexion conexion = new Conexion();
                conexion.GuardarProject(txtServer.Text.ToString());
                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                MainWindow.sp.Speak("Grupo Gurdado");
                DashboardLista datasourcelista = new DashboardLista();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).frame.NavigationService.Navigate(datasourcelista);
                        break;
                    }
                }
            }
            else
            {
                MainWindow.sp.Speak("Ingrese todo los datos");
            }
            
        }

        public bool validar()
        {
            if (txtServer.Text == "") return false;

            return true;
        }

        private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Words.Count == 2)
            {
                string command = e.Result.Words[0].Text.ToLower();
                string value = e.Result.Words[1].Text.ToLower();
                switch (command)
                {
                    case "guardar":
                        switch (value)
                        {
                            case "grupo":
                                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                MainWindow._recognizer.RecognizeAsyncStop();
                                Conexion conexion = new Conexion();
                                if(ProjectId != 0)
                                {
                                    conexion.updateProject(txtServer.Text.ToString(),ProjectId);
                                }
                                else
                                {
                                    conexion.GuardarProject(txtServer.Text.ToString());
                                }
                                MainWindow.sp.Speak("Grupo Guardado");
                                DashboardLista datasourcelista = new DashboardLista();
                                foreach (Window window in Application.Current.Windows)
                                {
                                    if (window.GetType() == typeof(MainWindow))
                                    {
                                        (window as MainWindow).frame.NavigationService.Navigate(datasourcelista);
                                        break;
                                    }
                                }
                                break;
                        }
                        break;
                    case "eliminar":
                        switch (value)
                        {
                            case "grupo":
                                if (ProjectId != 0)
                                {
                                    Conexion conexion = new Conexion();
                                    conexion.deleteGrupo(ProjectId);
                                    MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                    MainWindow._recognizer.RecognizeAsyncStop();
                                    MainWindow.sp.Speak("Grupo Eliminado");
                                    MainWindow.AlertaExito();
                                    DashboardLista datasourcelista = new DashboardLista();
                                    foreach (Window window in Application.Current.Windows)
                                    {
                                        if (window.GetType() == typeof(MainWindow))
                                        {
                                            (window as MainWindow).frame.NavigationService.Navigate(datasourcelista);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    MainWindow.sp.Speak("No existe el grupo");
                                }
                                break;
                        }
                        break;
                    case "cancelar":
                        switch (value)
                        {
                            case "accion":
                                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                MainWindow._recognizer.RecognizeAsyncStop();
                                DashboardLista listaashboard = new DashboardLista();
                                MainWindow.sp.Speak("Accion Cancelada");
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
                    case "llenar":
                        switch (value)
                        {
                            case "nombredelgrupo":
                                txtServer.Focus();
                                MainWindow.sp.Speak("Ingrese el Nombre del Grupo");
                                break;
                        }
                        break;
                    case "abrir":
                        switch (value)
                        {
                            case "teclado":
                                VKeyboard.IsOpen = true;
                                //sp.Speak("Teclado Habilitado");
                                break;
                        }
                        break;
                    case "cerrar":
                        switch (value)
                        {
                            case "teclado":
                                VKeyboard.IsOpen = false;
                                //sp.Speak("Teclado Deshabilitado");
                                break;
                        }
                        break;
                    default:
                        MainWindow.sp.Speak("Comando de Voz no Reconocido");
                        break;
                }
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (ProjectId != 0)
            {
                Conexion conexion = new Conexion();
                conexion.deleteGrupo(ProjectId);
                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                MainWindow.sp.Speak("Grupo Eliminado");
                MainWindow.AlertaExito();
                DashboardLista datasourcelista = new DashboardLista();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).frame.NavigationService.Navigate(datasourcelista);
                        break;
                    }
                }
            }
            else
            {
                MainWindow.sp.Speak("No existe el grupo");
            }
        }

        private void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            if (validar())
            {
                Conexion conexion = new Conexion();
                conexion.updateProject(txtServer.Text.ToString(),ProjectId);
                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                MainWindow.sp.Speak("Grupo Gurdado");
                MainWindow.AlertaExito();
                DashboardLista datasourcelista = new DashboardLista();
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).frame.NavigationService.Navigate(datasourcelista);
                        break;
                    }
                }
            }
            else
            {
                MainWindow.sp.Speak("Ingrese todo los datos");
            }
        }
    }
}
