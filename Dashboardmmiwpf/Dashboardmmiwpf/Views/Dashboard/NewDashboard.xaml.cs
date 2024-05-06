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
    /// Interaction logic for NewDashboard.xaml
    /// </summary>
    public partial class NewDashboard : Page
    {
        private FloatingTouchScreenKeyboard VKeyboard = new FloatingTouchScreenKeyboard();
        private int  DashboardId = 0;
        public NewDashboard()
        {
            InitializeComponent();
            Conexion conexion = new Conexion();
            DataTable grupos = conexion.GetProjects();
            cbxGrupo.ItemsSource = grupos.Rows.OfType<DataRow>().Select(dr => dr.Field<String>("Nombre")).ToList();
            DataTable DataSource = conexion.GetDataSource();
            cbxBD.ItemsSource = DataSource.Rows.OfType<DataRow>().Select(dr => dr.Field<String>("DataSourceTitle")).ToList();
            MainWindow.page = "NewDashboard";

            txtDataSource.Visibility = Visibility.Hidden;
            txtGrupo.Visibility = Visibility.Hidden;
            cbxBD.Visibility = Visibility.Visible;
            cbxBD.Visibility = Visibility.Visible;
            btnEliminar.Visibility = Visibility.Hidden;
            btnActualizar.Visibility = Visibility.Hidden;
            btnGuardar.Visibility = Visibility.Visible;
            btnGuardar_Copy.Visibility = Visibility.Visible;
            

            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("guardar", "cancelar", "llenar", "abrir", "cerrar");
            grammarBuilder.Append(commandChoices);

            Choices valueChoices = new Choices();
            valueChoices.Add("dashboard");
            valueChoices.Add("accion");
            valueChoices.Add("nombre");
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

        public NewDashboard(int id)
        {
            InitializeComponent();
            Conexion conexion = new Conexion();
            txtDataSource.Visibility = Visibility.Visible;
            txtGrupo.Visibility = Visibility.Visible;
            cbxBD.Visibility = Visibility.Hidden;
            cbxBD.Visibility = Visibility.Hidden;

            DashboardId = id;
            Projects grupo = new Projects();
            Projects grupo_mostrar = new Projects();
            Dashboard dashboard = new Dashboard();
            DataSource datasource = new DataSource();
            grupo.DataTabletoListView(conexion.GetDashboardsIndex(id));
            dashboard = grupo.lstDashboard.First();
            grupo_mostrar = grupo.DataTabletoList(conexion.GetProject(dashboard.ProjectId)).ToList().First();
            datasource = datasource.DataTabletoList(conexion.GetDataSource()).Where(x => x.id == dashboard.DataSourceId).FirstOrDefault();


            txtDataSource.Text = datasource.nombre;
            txtGrupo.Text = grupo_mostrar.Nombre;
            txtNombre.Text = dashboard.Nombre;

            MainWindow.page = "NewDashboard";

            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("guardar", "cancelar", "llenar", "abrir", "cerrar","eliminar");
            grammarBuilder.Append(commandChoices);

            Choices valueChoices = new Choices();
            valueChoices.Add("dashboard");
            valueChoices.Add("accion");
            valueChoices.Add("nombre");
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
            if (DashboardId != 0)
            {
                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                MainWindow.sp.Speak("Acción Cancelada");
                DetalleDashBoard datasourcelista = new DetalleDashBoard(DashboardId);
                this.NavigationService.Navigate(datasourcelista);
            }
            else
            {
                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                MainWindow.sp.Speak("Acción Cancelada");
                DashboardLista datasourcelista = new DashboardLista();
                this.NavigationService.Navigate(datasourcelista);
            }
            
        }

        public bool validar()
        {
            if (txtNombre.Text == "") return false;
            if (cbxGrupo.SelectedIndex == -1) return false;
            if (cbxBD.SelectedIndex == -1) return false;

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
                            case "dashboard":
                                if (txtNombre.Text != "")
                                {
                                    MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                    MainWindow._recognizer.RecognizeAsyncStop();
                                    Conexion conexion = new Conexion();
                                    Projects project = new Projects();
                                    DataSource ds = new DataSource();
                                    List<DataSource> lstds = ds.DataTabletoList(conexion.GetDataSource());
                                    List<Projects> projects = project.DataTabletoList(conexion.GetProjects());
                                    if(DashboardId != 0)
                                    {
                                        conexion.updateDashboard(DashboardId,txtNombre.Text);
                                    }
                                    else if (validar())
                                    {
                                        conexion.GuardarDashboard(txtNombre.Text.ToString(), projects.Where(x => x.Nombre.Equals(cbxGrupo.Text)).ToList().First().ProjectId, lstds.Where(x => x.titulo.Equals(cbxBD.Text)).ToList().First().id);
                                    }
                                    else
                                    {
                                        MainWindow.sp.Speak("Llene todos los datos");
                                        MainWindow.AlertaFaltanDatos();
                                        break;
                                    }

                                    MainWindow.AlertaExito();
                                    DashboardLista datasourcelista = new DashboardLista();
                                    MainWindow.sp.Speak("Dashboard Guardado");
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
                                    MainWindow.AlertaFaltanDatos();
                                    MainWindow.sp.Speak("Llene todos los datos");
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
                    case "eliminar":
                        switch (value)
                        {
                            case "dashboard":
                                if (DashboardId != 0)
                                {
                                    MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                                    MainWindow._recognizer.RecognizeAsyncStop();
                                    DashboardLista listaashboard = new DashboardLista();
                                    Conexion conexion = new Conexion();
                                    conexion.deleteDashboard(DashboardId);
                                    MainWindow.AlertaExito();
                                    MainWindow.sp.Speak("Dashboard Eliminado");
                                    foreach (Window window in Application.Current.Windows)
                                    {
                                        if (window.GetType() == typeof(MainWindow))
                                        {
                                            (window as MainWindow).frame.NavigationService.Navigate(listaashboard);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    MainWindow.sp.Speak("Dashboard no Existe");
                                }
                                break;
                        }
                        break;
                    case "llenar":
                        switch (value)
                        {
                            case "nombre":
                                txtNombre.Focus();
                                MainWindow.sp.Speak("Ingrese el Nombre del Cuadro de Mando");
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
        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (validar())
            {
                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                MainWindow.sp.Speak("Dashboard Guardado");
                MainWindow.AlertaExito();
                Conexion conexion = new Conexion();
                Projects project = new Projects();
                DataSource ds = new DataSource();
                List<DataSource> lstds = ds.DataTabletoList(conexion.GetDataSource());
                List<Projects> projects = project.DataTabletoList(conexion.GetProjects());
                conexion.GuardarDashboard(txtNombre.Text.ToString(), projects.Where(x => x.Nombre.Equals(cbxGrupo.Text)).ToList().First().ProjectId,
                    lstds.Where(x => x.titulo.Equals(cbxBD.Text)).ToList().First().id);
                
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
                MainWindow.AlertaFaltanDatos();
                MainWindow.sp.Speak("Ingrese todos los campos");
            }
        }

        private void btnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (DashboardId != 0)
            {
                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                DashboardLista listaashboard = new DashboardLista();
                Conexion conexion = new Conexion();
                conexion.deleteDashboard(DashboardId);
                MainWindow.AlertaExito();
                MainWindow.sp.Speak("Dashboard Eliminado");
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).frame.NavigationService.Navigate(listaashboard);
                        break;
                    }
                }
            }
            else
            {
                MainWindow.sp.Speak("Dashboard no Existe");
            }
        }

        private void btnActualizar_Click(object sender, RoutedEventArgs e)
        {
            if (DashboardId != 0)
            {
                MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                DetalleDashBoard listaashboard = new DetalleDashBoard(DashboardId);
                Conexion conexion = new Conexion();
                conexion.updateDashboard(DashboardId, txtNombre.Text);
                MainWindow.AlertaExito();
                MainWindow.sp.Speak("Dashboard Guardado");
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.GetType() == typeof(MainWindow))
                    {
                        (window as MainWindow).frame.NavigationService.Navigate(listaashboard);
                        break;
                    }
                }
            }
            else
            {
                MainWindow.sp.Speak("Dashboard no Existe");
            }
        }

        private void btnGuardar_Copy_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.sp.Speak("Acción Cancelada");
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    DashboardLista listaashboard = new DashboardLista();
                    (window as MainWindow).frame.NavigationService.Navigate(listaashboard);
                    break;
                }
            }
        }
    }
}
