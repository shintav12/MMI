using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Data;
using System.Speech.Recognition;
using System.Windows;
using WpfKb.Controls;

namespace Dashboardmmiwpf
{
    /// <summary>
    /// Interaction logic for DetalleDataview.xaml
    /// </summary>
    public partial class DetalleDataview : Page
    {
        private FloatingTouchScreenKeyboard VKeyboard = new FloatingTouchScreenKeyboard();
        int DvId;
        public DetalleDataview()
        {
            InitializeComponent();
            Conexion conexion = new Conexion();
            btnEliminar.Visibility = Visibility.Hidden;
            btnCancelar.Visibility = Visibility.Visible;
            DataTable dbs = conexion.GetDataSource();
            DvId = -1;
            cBxDB.ItemsSource = dbs.Rows.OfType<DataRow>().Select(dr => dr.Field<String>("DataSourceTitle")).ToList();
            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("guardar", "probar", "cancelar", "llenar", "comando", "abrir", "cerrar");
            grammarBuilder.Append(commandChoices);

            Choices valueChoices = new Choices();
            valueChoices.Add("vista");
            valueChoices.Add("consulta");
            valueChoices.Add("query");
            valueChoices.Add("accion");
            valueChoices.Add("seleccionar", "from");
            valueChoices.Add("teclado");
            valueChoices.Add("nombre");
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

        public DetalleDataview(int DvId)
        {
            InitializeComponent();
            btnEliminar.Visibility = Visibility.Visible;
            btnCancelar.Visibility = Visibility.Hidden;
            Conexion conexion = new Conexion();
            DataTable dbs = conexion.GetDataSource();
            this.DvId = DvId;
            cBxDB.ItemsSource = dbs.Rows.OfType<DataRow>().Select(dr => dr.Field<String>("DataSourceTitle")).ToList();
            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("guardar", "probar","cancelar" ,"llenar", "comando", "abrir", "cerrar","eliminar");
            grammarBuilder.Append(commandChoices);
            DataSource ds = new DataSource();


            ds.DataTabletoListView(conexion.GetDataViewsIndex(DvId));
            DataView dv = ds.dataview.First();
            List<DataSource> lstds = ds.DataTabletoList(dbs);
            DataSource ds1 = lstds.Where(x => x.id.Equals(dv.DsId)).ToList().SingleOrDefault();
            int indice = lstds.IndexOf(ds1);
            cBxDB.ItemsSource = dbs.Rows.OfType<DataRow>().Select(dr => dr.Field<String>("DataSourceTitle")).ToList();
            cBxDB.SelectedIndex = indice;
            txtQuery.Text = dv.Query;
            txtName.Text = dv.DsNombre;
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

            List<DataSource> ds3 = ds.DataTabletoList(conexion.GetDataSource());
            bool error = true;
            DataTable tabla = conexion.getRegistros(txtQuery.Text, ds3[cBxDB.SelectedIndex].nombre, ref error);

            dGResult.ItemsSource = tabla.DefaultView;
            dGResult.AutoGenerateColumns = true;
            dGResult.CanUserAddRows = false;

            Choices valueChoices = new Choices();
            valueChoices.Add("vista");
            valueChoices.Add("consulta");
            valueChoices.Add("query","nombre");
            valueChoices.Add("accion");
            valueChoices.Add("seleccionar", "from");
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

        private void btnCancelar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.sp.Speak("Acción Cancelada");
            DataSourceLista detalledataview = new DataSourceLista();
            this.NavigationService.Navigate(detalledataview);
        }

        private void btnEliminar_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DvId != 0)
            {
                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                MainWindow.sp.Speak("Vista de Datos Eliminada");
                MainWindow.AlertaExito();
                Conexion conexion = new Conexion();
                conexion.deleteDataview(DvId);
                DataSourceLista datasourcelista = new DataSourceLista();
                this.NavigationService.Navigate(datasourcelista);
            }
            else
            {
                MainWindow.sp.Speak("No se ha seleccionado ninguna base de datos");
            }
        }

        private void btnSiguiente_Click(object sender, System.Windows.RoutedEventArgs e)
        {
         
            if (txtQuery.Text != "" && txtName.Text != "" && cBxDB.SelectedIndex != -1)
            {
                Conexion conexion = new Conexion();
                DataSource datas = new DataSource();
                List<DataSource> ds = datas.DataTabletoList(conexion.GetDataSource());
                if (DvId != -1)
                {
                    conexion.updateDataView(txtQuery.Text, ds[cBxDB.SelectedIndex].nombre, txtName.Text, ds[cBxDB.SelectedIndex].id, DvId);
                    MainWindow.sp.Speak("Visa de datos Actualizada");
                }
                    
                else
                {
                    conexion.GuardarDataView(txtQuery.Text, ds[cBxDB.SelectedIndex].nombre, txtName.Text, ds[cBxDB.SelectedIndex].id);
                    MainWindow.sp.Speak("Vista de Datos Creada");
                }
                    

                MainWindow.AlertaExito();
                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                DataSourceLista detalledataview = new DataSourceLista();
                this.NavigationService.Navigate(detalledataview);
            }
            else
            {
                MainWindow.AlertaFaltanDatos();
                MainWindow.sp.Speak("Ingrese todos los datos");
            }
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

        private void btnBack_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.sp.Speak("Acción Cancelada");
            DataSourceLista datasourcelista = new DataSourceLista();
            this.NavigationService.Navigate(datasourcelista);
        }

        private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Words.Count == 2)
            {
                string command = e.Result.Words[0].Text.ToLower();
                string value = e.Result.Words[1].Text.ToLower();
                switch (command)
                {
                    case "probar":
                        switch (value)
                        {
                            case "consulta":
                                {
                                    if (txtQuery.Text != "")
                                    {
                                        Conexion conexion = new Conexion();
                                        DataSource datas = new DataSource();
                                        List<DataSource> ds = datas.DataTabletoList(conexion.GetDataSource());
                                        bool error = true;
                                        DataTable tabla = conexion.getRegistros(txtQuery.Text, ds[cBxDB.SelectedIndex].nombre, ref error);

                                        if (error)
                                        {
                                            MainWindow.sp.Speak("Error de Sintaxis");
                                            MainWindow.AlertaFaltanDatos("Hay un error en la sintaxis");
                                        }
                                        else
                                        {
                                            dGResult.ItemsSource = tabla.DefaultView;
                                            dGResult.AutoGenerateColumns = true;
                                            dGResult.CanUserAddRows = false;
                                            MainWindow.sp.Speak("Tu consulta fue exitosa");
                                            MainWindow.AlertaCustom("Consulta Exitosa","La consula ha base de datos se realizó satisfactoriamente");
                                        }
                                    }
                                    else
                                    {
                                        MainWindow.sp.Speak("Ingrese el comando");
                                        MainWindow.AlertaFaltanDatos("No ha ingresado nigun comando en el cuadro de texto");
                                    }
                                }
                                break;
                        }
                        break;
                    case "guardar":
                        switch (value)
                        {
                            case "vista":

                                if (txtQuery.Text != "" && txtName.Text != "" && cBxDB.SelectedIndex != -1)
                                {
                                    Conexion conexion = new Conexion();
                                    DataSource datas = new DataSource();
                                    List<DataSource> ds = datas.DataTabletoList(conexion.GetDataSource());
                                    if (DvId != -1)
                                        conexion.updateDataView(txtQuery.Text, ds[cBxDB.SelectedIndex].nombre, txtName.Text, ds[cBxDB.SelectedIndex].id, DvId);
                                    else
                                        conexion.GuardarDataView(txtQuery.Text, ds[cBxDB.SelectedIndex].nombre, txtName.Text, ds[cBxDB.SelectedIndex].id);

                                    MainWindow.AlertaExito();
                                    MainWindow.sp.Speak("Se guardó la vista de datos satisfactoriamente");
                                    MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                    MainWindow._recognizer.RecognizeAsyncStop();
                                    DataSourceLista detalledataview = new DataSourceLista();
                                    this.NavigationService.Navigate(detalledataview);
                                }
                                else
                                {
                                    MainWindow.AlertaFaltanDatos();
                                    MainWindow.sp.Speak("Ingrese todos los datos");
                                }
                                break;
                        }
                        break;
                    case "eliminar":
                        switch (value)
                        {
                            case "vista":

                                if (DvId != 0)
                                {
                                    MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                    MainWindow._recognizer.RecognizeAsyncStop();
                                    MainWindow.sp.Speak("vista de datos Eliminada");
                                    MainWindow.AlertaExito();
                                    Conexion conexion = new Conexion();
                                    conexion.deleteDataview(DvId);
                                    DataSourceLista datasourcelista = new DataSourceLista();
                                    this.NavigationService.Navigate(datasourcelista);
                                }
                                else
                                {
                                    MainWindow.sp.Speak("No se ha seleccionado ninguna base de datos");
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
                                MainWindow.sp.Speak("Acción Cancelada");
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
                                MainWindow.sp.Speak("Ingrese su consulta");
                                txtQuery.Focus();
                                break;
                            case "nombre":
                                MainWindow.sp.Speak("Ingrese el nombre de la vista de datos");
                                txtName.Focus();
                                break;
                        }
                        break;
                    case "comando":
                        switch (value)
                        {
                            case "seleccionar":
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
                                break;
                        }
                        break;
                    case "cerrar":
                        switch (value)
                        {
                            case "teclado":
                                VKeyboard.IsOpen = false;
                                break;
                        }
                        break;
                }
            }
        }

        private void btnSiguiente_Copy_Click(object sender, RoutedEventArgs e)
        {

            if (txtQuery.Text != "")
            {
                Conexion conexion = new Conexion();
                DataSource datas = new DataSource();
                List<DataSource> ds = datas.DataTabletoList(conexion.GetDataSource());
                bool error = true;
                DataTable tabla = conexion.getRegistros(txtQuery.Text, ds[cBxDB.SelectedIndex].nombre, ref error);

                if (error)
                {
                    MainWindow.sp.Speak("Error de Sintaxis");
                    MainWindow.AlertaFaltanDatos("Hay un error en la sintaxis");
                }
                else
                {
                    dGResult.ItemsSource = tabla.DefaultView;
                    dGResult.AutoGenerateColumns = true;
                    dGResult.CanUserAddRows = false;
                    MainWindow.sp.Speak("Tu consulta fue exitosa");
                    MainWindow.AlertaCustom("Consulta Exitosa", "La consula ha base de datos se realizó satisfactoriamente");
                }
            }
            else
            {
                MainWindow.sp.Speak("Ingrese el comando");
                MainWindow.AlertaFaltanDatos("No ha ingresado nigun comando en el cuadro de texto");
            }

        }
    }
}
