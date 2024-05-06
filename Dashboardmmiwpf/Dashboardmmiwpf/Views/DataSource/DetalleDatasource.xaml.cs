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
using System.Data;
using System.ComponentModel;
using System.Speech.Recognition;
using MahApps.Metro.Controls.Dialogs;

using WpfKb.Controls;

namespace Dashboardmmiwpf
{
    /// <summary>
    /// Interaction logic for DetalleDatasource.xaml
    /// </summary>
    public partial class DetalleDatasource : Page
    {

        public Boolean bloqueado = false;
        private FloatingTouchScreenKeyboard VKeyboard = new FloatingTouchScreenKeyboard();
        public DetalleDatasource()
        {
            MainWindow.page = "DetalleDataSource";
            InitializeComponent();
            Conexion conexion = new Conexion();
            
            DataTable dbs = conexion.GetDBs();
            CbxBD.ItemsSource = dbs.Rows.OfType<DataRow>().Select(dr => dr.Field<String>("name")).ToList();
            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("guardar", "cancelar","llenar", "abrir", "cerrar","probar");
            grammarBuilder.Append(commandChoices);

            Choices valueChoices = new Choices();
            valueChoices.Add("conexión");
            valueChoices.Add("acción");
            valueChoices.Add("servidor", "nombre", "usuario", "clave");
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

        public void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            bool validacion = validar();
            if (validacion)
            {
                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                Conexion conexion = new Conexion();
                DataSourceLista datasourcelista = new DataSourceLista();
                MainWindow.sp.Speak("Conexión Guardada");
                MainWindow.AlertaExito();

                this.NavigationService.Navigate(datasourcelista);
                conexion.GuardarDataSource(txtTitle.Text, txtPassword.Password, txtusername.Text, CbxBD.Text, txtServer.Text);
            }
            else
            {
                MainWindow.AlertaFaltanDatos();
                MainWindow.sp.Speak("Ingrese todos los datos");
            }
            
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.sp.Speak("Conexión Eliminada");
            MainWindow.AlertaExito();
            DataSourceLista datasourcelista = new DataSourceLista();
            this.NavigationService.Navigate(datasourcelista);

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.sp.Speak("Acción Cancelada");
            DataSourceLista datasourcelista = new DataSourceLista();
            this.NavigationService.Navigate(datasourcelista);
        }

        
        public bool validar()
        {
            if (txtPassword.Password == "") return false;
            if (txtusername.Text == "") return false;
            if (txtServer.Text == "") return false;
            if (txtTitle.Text == "") return false;
            if (CbxBD.SelectedIndex == -1) return false;

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
                            case "conexión":
                                if (validar())
                                {
                                    Conexion conexion = new Conexion();
                                    MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                    MainWindow._recognizer.RecognizeAsyncStop();
                                    conexion.GuardarDataSource(txtTitle.Text, txtPassword.Password, txtusername.Text, CbxBD.Text, txtServer.Text);
                                    MainWindow.sp.Speak("Conexión Guardado");
                                    DataSourceLista listadatasource = new DataSourceLista();
                                    MainWindow.AlertaExito();
                                    foreach (Window window in Application.Current.Windows)
                                    {
                                        if (window.GetType() == typeof(MainWindow))
                                        {
                                            (window as MainWindow).frame.NavigationService.Navigate(listadatasource);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    MainWindow.AlertaFaltanDatos();
                                    MainWindow.sp.Speak("Ingrese todos los datos");
                                }
                                break;
                        }
                        break;
                    case "cancelar": 
                        switch (value)
                        {
                            case "acción":
                                MainWindow.sp.Speak("Accion Cancelada");
                                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                MainWindow._recognizer.RecognizeAsyncStop();
                                DataSourceLista listaashboard = new DataSourceLista();
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
                            case "servidor":
                                txtServer.Focus();
                                MainWindow.sp.Speak("Ingrese el Nombre del Servidor");
                                break;
                            case "nombre":
                                MainWindow.sp.Speak("Ingrese el nombre de la conexión");
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

                    case "probar":
                        switch (value)
                        {
                            case "conexión":
                                if (validar())
                                {
                                    MainWindow.testConnection(CbxBD.Text, txtServer.Text);
                                }
                                else
                                {
                                    MainWindow.sp.Speak("Profavor llene todos los datos");
                                    MainWindow.AlertaFaltanDatos();
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

        private void btnGuardar_Click_1(object sender, RoutedEventArgs e)
        {
            if (validar())
            {
                MainWindow.testConnection(CbxBD.Text, txtServer.Text);
            }
            else
            {
                MainWindow.sp.Speak("porfavor llene todos los datos");
                MainWindow.AlertaFaltanDatos();
            }
        }
    }
}
