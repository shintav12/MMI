using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for EditarEliminar.xaml
    /// </summary>
    public partial class EditarEliminar : Page
    {
        DataSource Datasource;
        private FloatingTouchScreenKeyboard VKeyboard = new FloatingTouchScreenKeyboard();
        public EditarEliminar()
        {
            InitializeComponent();
            this.Datasource = new DataSource();

            Conexion conexion = new Conexion();
            DataTable dbs = conexion.GetDataSource();

            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("guardar", "eliminar","volver", "abrir", "cerrar");
            grammarBuilder.Append(commandChoices);

            Choices valueChoices = new Choices();
            valueChoices.Add("datasource");
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

        public EditarEliminar(int id)
        {
            InitializeComponent();
            this.Datasource = new DataSource();

            Conexion conexion = new Conexion();
            DataTable dbs = conexion.GetDataSource();

            Conexion conect = new Conexion();

            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("guardar", "eliminar", "volver","llenar", "abrir", "cerrar");
            grammarBuilder.Append(commandChoices);

            DataSource datas = new DataSource();
            List<DataSource> ds = datas.DataTabletoList(conect.GetDataSource());

            Choices valueChoices = new Choices();
            valueChoices.Add("conexión");
            valueChoices.Add("acción");
            valueChoices.Add("nombre", "usuario", "clave");
            valueChoices.Add("teclado");
            grammarBuilder.Append(valueChoices);

            Datasource = ds.Where(x => x.id.Equals(id)).ToList().First();

            txtTitle.Text = Datasource.titulo;
            txtPassword.Password = Datasource.password;
            txtusername.Text = Datasource.user;
            txtServer.Text = Datasource.Servername;
            txtusername_Copy.Text = Datasource.nombre;

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

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.AlertaExito();
            MainWindow.sp.Speak("Conexión Guardada");
            Conexion conexion = new Conexion();
            conexion.updatedatasource(Datasource.id, txtTitle.Text, txtusername.Text, txtPassword.Password);
            DataSourceLista datasourcelista = new DataSourceLista();
            this.NavigationService.Navigate(datasourcelista);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            DataSourceLista datasourcelista = new DataSourceLista();
            this.NavigationService.Navigate(datasourcelista);
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            
            if (Datasource.id != 0)
            {
                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                MainWindow.sp.Speak("Conexión Eliminada");
                MainWindow.AlertaExito();
                Conexion conexion = new Conexion();
                conexion.deleteDatasource(Datasource.id);
                DataSourceLista datasourcelista = new DataSourceLista();
                this.NavigationService.Navigate(datasourcelista);
            }
            else {
                MainWindow.sp.Speak("No se ha seleccionado ninguna base de datos");
            }
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
                            case "conexión":

                                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                MainWindow._recognizer.RecognizeAsyncStop();
                                Conexion conexion = new Conexion();
                                conexion.updatedatasource(Datasource.id, txtTitle.Text, txtusername.Text, txtPassword.Password);
                                MainWindow.AlertaExito();
                                MainWindow.sp.Speak("Conexión Guardado");
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
                    case "llenar":
                        switch (value)
                        {
                            case "servidor":
                                txtServer.Focus();
                                MainWindow.sp.Speak("Ingrese el Nombre del Servidor");
                                break;
                            case "nobre":
                                MainWindow.sp.Speak("Ingrese el Título de al Conexión");
                                txtTitle.Focus();
                                break;
                            case "usuario":
                                MainWindow.sp.Speak("Ingrese el Nombre de Usuario");
                                txtusername.Focus();
                                break;
                            case "clave":
                                MainWindow.sp.Speak("Ingrese la clave");
                                txtPassword.Focus();
                                break;
                        }
                        break;
                    case "eliminar":
                        switch (value)
                        {
                            case "conexión":
                                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                MainWindow._recognizer.RecognizeAsyncStop();
                                Conexion conexion = new Conexion();
                                if (Datasource.id != 0)
                                    conexion.deleteDatasource(Datasource.id);
                                MainWindow.AlertaExito();
                                MainWindow.sp.Speak("Conexión Eliminada");
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
                    case "cancelar":
                        switch (value)
                        {
                            case "accion":
                                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                MainWindow._recognizer.RecognizeAsyncStop();
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
                }
            }
        }
    }
}
