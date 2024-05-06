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
using System.Speech.Recognition;
using WpfKb.Controls;

namespace Dashboardmmiwpf
{
    /// <summary>
    /// Interaction logic for DetalleDataviewListaDatos.xaml
    /// </summary>
   
    
    public partial class DetalleDataviewListaDatos : Page
    {
        String Query;
        String NombreDataSource;
        int DsID;
        int DvID;
        private FloatingTouchScreenKeyboard VKeyboard = new FloatingTouchScreenKeyboard();
        public DetalleDataviewListaDatos()
        {
            InitializeComponent();
        }

        public DetalleDataviewListaDatos(DataTable datos, String Query, String NombreDataSource, int DsID,String titulo = "",int DvId = -1)
        {
            InitializeComponent();
            InitializeComponent();
            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("guardar","volver","cancelar","abrir", "cerrar");
            grammarBuilder.Append(commandChoices);
            this.NombreDataSource = NombreDataSource;
            this.Query = Query;
            this.DsID = DsID;
            this.DvID = DvId;
            txtName.Text = titulo;
            dGResult.ItemsSource = datos.DefaultView;
            dGResult.AutoGenerateColumns = true;
            dGResult.CanUserAddRows = false;
            Choices valueChoices = new Choices();
            valueChoices.Add("paso");
            valueChoices.Add("dataview");
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
                            case "dataview":
                                if (txtName.Text != "")
                                {
                                    MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                    MainWindow._recognizer.RecognizeAsyncStop();
                                    Conexion conexion = new Conexion();
                                    if (DvID != -1)
                                        conexion.updateDataView(this.Query, this.NombreDataSource, txtName.Text, DsID, DvID);
                                    else
                                        conexion.GuardarDataView(this.Query, this.NombreDataSource, txtName.Text, DsID);

                                    DataSourceLista detalledataview = new DataSourceLista();
                                    this.NavigationService.Navigate(detalledataview);
                                }
                                else
                                {
                                    MainWindow.sp.Speak("Ingrese el titulo del dataview");
                                }
                                break;
                        }
                        break;
                    case "volver":
                        switch (value)
                        {
                            case "paso":
                                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                MainWindow._recognizer.RecognizeAsyncStop();
                                DetalleDataview listaashboard = new DetalleDataview();
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
                    case "cancelar":
                        switch (value)
                        {
                            case "dataview":
                                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                MainWindow._recognizer.RecognizeAsyncStop();
                                DataSourceLista listaashboard = new DataSourceLista();
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

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            DetalleDataview detalledataview = new DetalleDataview();
            this.NavigationService.Navigate(detalledataview);
        }

        private void btnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text != "")
            {
                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                Conexion conexion = new Conexion();
                if (DvID != -1)
                    conexion.updateDataView(this.Query, this.NombreDataSource, txtName.Text, DsID, DvID);
                else
                    conexion.GuardarDataView(this.Query, this.NombreDataSource, txtName.Text, DsID);

                DataSourceLista detalledataview = new DataSourceLista();
                this.NavigationService.Navigate(detalledataview);
            }
            else
            {
                MainWindow.sp.Speak("Ingrese el titulo del dataview");
            }
            
        }
    }
}
