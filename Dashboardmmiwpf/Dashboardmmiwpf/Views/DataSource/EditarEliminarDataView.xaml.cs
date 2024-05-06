using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Speech.Recognition;
using System.Windows;
using System.Windows.Controls;
using WpfKb.Controls;

namespace Dashboardmmiwpf
{
    /// <summary>
    /// Interaction logic for EditarEliminarDataView.xaml
    /// </summary>
    public partial class EditarEliminarDataView : Page
    {
        public EditarEliminarDataView()
        {
            InitializeComponent();
        }
        string Titulo;
        private FloatingTouchScreenKeyboard VKeyboard = new FloatingTouchScreenKeyboard();
        int DvId;
        public EditarEliminarDataView(int dataviewIndex)
        {
           
            InitializeComponent();
            Conexion conexion = new Conexion();
            DataSource ds = new DataSource();
            ds.DataTabletoListView(conexion.GetDataViewsIndex(dataviewIndex));
            DataView dv = ds.dataview.First();

            DataTable dbs = conexion.GetDataSource();
            List<DataSource> lstds = ds.DataTabletoList(dbs);
            DataSource ds1 = lstds.Where(x => x.id.Equals(dv.DsId)).ToList().SingleOrDefault();
            int indice = lstds.IndexOf(ds1);
            cBxDB.ItemsSource = dbs.Rows.OfType<DataRow>().Select(dr => dr.Field<String>("DataSourceTitle")).ToList();
            cBxDB.SelectedIndex = indice;
            txtQuery.Text = dv.Query;
            DvId = dataviewIndex;
            Titulo = dv.DsNombre;
            if (cBxDB.SelectedIndex != -1)
            {
                Conexion conect = new Conexion();

                DataSource datas = new DataSource();
                List<DataSource> ds2 = datas.DataTabletoList(conect.GetDataSource());

                DataTable dt = conect.getTables(ds2[cBxDB.SelectedIndex].nombre);

                dGColumnas.ItemsSource = dt.DefaultView;
                dGColumnas.AutoGenerateColumns = true;
                dGColumnas.CanUserAddRows = false;
            }
            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("siguiente", "ir a", "llenar","comando");
            grammarBuilder.Append(commandChoices);

            Choices valueChoices = new Choices();
            valueChoices.Add("paso");
            valueChoices.Add("listado datasource");
            valueChoices.Add("query");
            valueChoices.Add("select","from");
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

        private void btnSiguiente_Click(object sender, RoutedEventArgs e)
        {
            Conexion conexion = new Conexion();
            DataSource datas = new DataSource();
            List<DataSource> ds = datas.DataTabletoList(conexion.GetDataSource());
            bool error = true;
            DataTable tabla = conexion.getRegistros(txtQuery.Text, ds[cBxDB.SelectedIndex].nombre, ref error);

            if (error)
            {
                txtQuery.Text = "Hay un error de sintaxis";
            }
            else
            {
                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                DetalleDataviewListaDatos datos = new DetalleDataviewListaDatos(tabla, txtQuery.Text, ds[cBxDB.SelectedIndex].nombre, ds[cBxDB.SelectedIndex].id,Titulo,DvId);
                this.NavigationService.Navigate(datos, tabla);
            }
        }
        private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (true)
            {
                string command = e.Result.Words[0].Text.ToLower();
                string value = e.Result.Words[1].Text.ToLower();
                switch (command)
                {
                    case "siguiente":
                        switch (value)
                        {
                            case "paso":

                                Conexion conexion = new Conexion();
                                DataSource datas = new DataSource();
                                List<DataSource> ds = datas.DataTabletoList(conexion.GetDataSource());
                                bool error = true;
                                if (cBxDB.Text == "")
                                {
                                    MainWindow.sp.Speak("Porfavor elegir un datasource");
                                    break;
                                }
                                DataTable tabla = conexion.getRegistros(txtQuery.Text, ds[cBxDB.SelectedIndex].nombre, ref error);

                                if (error)
                                {
                                    txtQuery.Text = txtQuery.Text + "Hay un error de sintaxis";
                                }
                                else
                                {
                                    MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                    MainWindow._recognizer.RecognizeAsyncStop();
                                    DetalleDataviewListaDatos datos = new DetalleDataviewListaDatos(tabla, txtQuery.Text, ds[cBxDB.SelectedIndex].nombre, ds[cBxDB.SelectedIndex].id);
                                    foreach (Window window in Application.Current.Windows)
                                    {
                                        if (window.GetType() == typeof(MainWindow))
                                        {
                                            (window as MainWindow).frame.NavigationService.Navigate(datos, tabla);
                                            break;
                                        }
                                    }
                                }

                                break;
                        }
                        break;
                    case "ir a":
                        switch (value)
                        {
                            case "listado datasource":
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
                            case "query":
                                MainWindow.sp.Speak("Inserte la consulta");
                                txtQuery.Focus();

                                break;
                        }
                        break;
                    case "comando":
                        switch (value)
                        {
                            case "select":
                                MainWindow.sp.Speak("Comando Ingresado");
                                txtQuery.Text = txtQuery.Text + " select";
                                txtQuery.Focus();
                                break;
                            case "from":
                                MainWindow.sp.Speak("Comando Ingresado");
                                txtQuery.Text = txtQuery.Text + " from";
                                txtQuery.Focus();
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

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            DataSourceLista detalledataview = new DataSourceLista();
            this.NavigationService.Navigate(detalledataview);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            DataSourceLista datasourcelista = new DataSourceLista();
            this.NavigationService.Navigate(datasourcelista);
        }

        private void cBxDB_DropDownClosed(object sender, EventArgs e)
        {
            if (cBxDB.SelectedIndex != -1)
            {
                Conexion conect = new Conexion();

                DataSource datas = new DataSource();
                List<DataSource> ds = datas.DataTabletoList(conect.GetDataSource());

                DataTable dt = conect.getTables(ds[cBxDB.SelectedIndex].nombre);

                dGColumnas.ItemsSource = dt.DefaultView;
                dGColumnas.AutoGenerateColumns = true;
                dGColumnas.CanUserAddRows = false;
            }
        }
    }
    
}
