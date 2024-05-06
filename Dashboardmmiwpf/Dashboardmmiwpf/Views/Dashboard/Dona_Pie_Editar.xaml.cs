using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
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

namespace Dashboardmmiwpf
{
    /// <summary>
    /// Lógica de interacción para Dona_Pie_Editar.xaml
    /// </summary>
    public partial class Dona_Pie_Editar : Page
    {
        int datasourceid = 0;
        int graficoid = 0;
        int dashboardida = 0;
        Dashboard db;

        DataTable dt = new DataTable();

        public Dona_Pie_Editar(int dashboardid, int idGrafico)
        {
            InitializeComponent();
            DataSource datasource = new DataSource();
            Projects pro = new Projects();
            Conexion conexion = new Conexion();
            Dashboard dash = new Dashboard();

            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("generar", "eliminar", "guardar", "cancelar");
            grammarBuilder.Append(commandChoices);

            Choices valueChoices = new Choices();
            valueChoices.Add("vistaprevia");
            valueChoices.Add("gráfico");
            valueChoices.Add("acción");
            grammarBuilder.Append(valueChoices);

            graficoid = idGrafico;



            pro.DataTabletoListView(conexion.GetDashboardIndex(dashboardid));

            dash = pro.lstDashboard.First();

            datasource = datasource.DataTabletoList(conexion.GetDataSource()).Where(x => x.id == dash.DataSourceId).FirstOrDefault();
            datasourceid = datasource.id;

            datasource.DataTabletoListView(conexion.GetDataViews(dash.DataSourceId));

            cbXdataviews.ItemsSource = datasource.dataview.Select(x => x.DsNombre).ToList();

            cbXdataviews.SelectedIndex = 0;
            MainWindow._recognizer.UnloadAllGrammars();
            MainWindow._recognizer.LoadGrammar(new Grammar(grammarBuilder));
            MainWindow._recognizer.RecognizeAsync(RecognizeMode.Multiple);
            dashboardida = dashboardid;
            Dibujar();

        }

        private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Words.Count == 2)
            {
                string command = e.Result.Words[0].Text.ToLower();
                string value = e.Result.Words[1].Text.ToLower();
                switch (command)
                {
                    case "generar":
                        {
                            switch (value)
                            {
                                case "vistaprevia":
                                    {
                                        dibujar();
                                        MainWindow.sp.Speak("Previsualización del gráfico generada");
                                        MainWindow.AlertaCustom("Previsuaización generada con éxito", "La previsuaización del gráfico se generado satisfactoriamente");
                                    }
                                    break;
                            }
                        }
                        break;
                    case "eliminar":
                        {
                            switch (value)
                            {
                                case "gráfico":
                                    {
                                        try
                                        {
                                            if (graficoid != 0)
                                            {
                                                Conexion cn = new Conexion();

                                                cn.deleteGrafico(graficoid);
                                                MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
                                                MainWindow._recognizer.RecognizeAsyncStop();
                                                MainWindow.sp.Speak("Gráfico Eliminado");
                                                MainWindow.AlertaExito();
                                                DetalleDashBoard datasourcelista = new DetalleDashBoard(dashboardida);
                                                this.NavigationService.Navigate(datasourcelista);
                                                break;
                                            }
                                            else
                                            {
                                                MainWindow.sp.Speak("No hubieron cambios en el gráfico");
                                                MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
                                                MainWindow._recognizer.RecognizeAsyncStop();
                                                DetalleDashBoard datasourcelista = new DetalleDashBoard(dashboardida);
                                                this.NavigationService.Navigate(datasourcelista);
                                                break;
                                            }

                                        }
                                        catch
                                        {
                                            MainWindow.AlertaFaltanDatos("Hubo un problema con su solicitud");
                                        }
                                    }
                                    break;
                            }
                        }
                        break;
                    case "cancelar":
                        {
                            switch (value)
                            {
                                case "acción":
                                    {
                                        if (dashboardida != 0)
                                        {
                                            MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
                                            MainWindow._recognizer.RecognizeAsyncStop();

                                            MainWindow.sp.Speak("Acción Cancelada");
                                            DetalleDashBoard datasourcelista = new DetalleDashBoard(dashboardida);
                                            this.NavigationService.Navigate(datasourcelista);
                                        }

                                    }
                                    break;
                            }
                        }
                        break;
                    case "guardar":
                        {
                            switch (value)
                            {
                                case "gráfico":
                                    {
                                        try
                                        {

                                            int ejexid = EjeX.SelectedIndex;
                                            int ejeyid = EjeY.SelectedIndex;

                                            if (ejexid != -1 && ejeyid != -1)
                                            {
                                                Conexion cn = new Conexion();
                                                DataSource dv = new DataSource();
                                                DataTable dbs = cn.GetDataViews();

                                                dv.DataTabletoListView(dbs);
                                                List<DataView> lstDv = dv.dataview;
                                                DataView dataview = lstDv.Where(x => x.DsNombre.Equals(cbXdataviews.Text)).Single();
                                                MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
                                                MainWindow._recognizer.RecognizeAsyncStop();
                                                MainWindow.AlertaExito();
                                                cn.updateGraphic(dataview.Query, EjeX.Text, EjeY.Text, graficoid);
                                                MainWindow.sp.Speak("El gráfico se guardo exitosamente");
                                                DetalleDashBoard datasourcelista = new DetalleDashBoard(dashboardida);
                                                this.NavigationService.Navigate(datasourcelista);
                                            }
                                            else
                                            {
                                                MainWindow.sp.Speak("No hubieron cambios en el gráfico");
                                                DetalleDashBoard datasourcelista = new DetalleDashBoard(dashboardida);
                                                this.NavigationService.Navigate(datasourcelista);
                                            }

                                        }
                                        catch
                                        {
                                            MainWindow.AlertaFaltanDatos("Hubo un problema con su solicitud");
                                        }
                                    }
                                    break;
                            }
                        }
                        break;

                }
            }
        }

        private void dibujar()
        {
            try
            {
                string ejey = EjeX.Text;
                string ejex = EjeY.Text;


                canvasPreview.Children.Clear();

                switch (1)
                {
                    case 1:
                        SeriesCollection SeriesCollection1 = new SeriesCollection();
                        PieChart chart = new PieChart();
                        foreach (DataRow dr in dt.Rows)
                        {
                            SeriesCollection1.Add(new PieSeries
                            {
                                Title = dr[ejey].ToString(),
                                Values = new ChartValues<ObservableValue> { new ObservableValue(Convert.ToDouble(dr[ejex].ToString())) },
                                DataLabels = true
                            });
                        }

                        chart.Series = SeriesCollection1;
                        canvasPreview.Children.Add(chart);
                        break;
                }
            }
            catch
            {

            }

        }


        private void Dibujar()
        {
            Conexion conexion = new Conexion();
            Grafico graficoTransf = new Grafico();
            DataSource datas = new DataSource();
            List<Grafico> graficoList = new List<Grafico>();
            graficoList = graficoTransf.DataTabletoList(conexion.GetGraphics(dashboardida));
            List<DataSource> ds = datas.DataTabletoList(conexion.GetDataSource());

            datas = ds.Where(x => x.id == datasourceid).FirstOrDefault();
            Grafico graph = new Grafico();
            graph = graph.DataTabletoList(conexion.GetGraficoIndex(graficoid)).First();
            switch (graph.tipoGrafico)
            {
                case 3:
                    {
                        Grid aux = new Grid();
                        Border border = new Border
                        {
                            BorderBrush = Brushes.Gray,
                            BorderThickness = new Thickness(1)
                        };
                        aux.Children.Add(border);
                        aux.Background = Brushes.Black;
                        aux.Width = 250;
                        aux.Height = 250;

                        PieChart chart = new PieChart();
                        ChartValues<Double> valores = new ChartValues<double>();
                        if (graph.ejex != "" && graph.ejey != "" && graph.query != "")
                        {
                            SeriesCollection SeriesCollection1 = new SeriesCollection();
                            List<string> Labels1 = new List<string>();
                            bool error = false;
                            DataTable dt = conexion.getRegistros(graph.query, datas.nombre, ref error);

                            foreach (DataRow dr in dt.Rows)
                            {
                                SeriesCollection1.Add(new PieSeries
                                {
                                    Title = dr[graph.ejex].ToString(),
                                    Values = new ChartValues<ObservableValue> { new ObservableValue(Convert.ToDouble(dr[graph.ejey].ToString())) },
                                    DataLabels = true
                                });
                            }
                            


                            DataContext = this;


                            chart.Series = SeriesCollection1;
                        }
                        else
                        {
                            SeriesCollection SeriesCollection2 = new SeriesCollection {
                                new PieSeries
                                {
                                    Title = "Chrome",
                                    Values = new ChartValues<ObservableValue> { new ObservableValue(8) },
                                    DataLabels = true
                                },
                                new PieSeries
                                {
                                    Title = "Mozilla",
                                    Values = new ChartValues<ObservableValue> { new ObservableValue(6) },
                                    DataLabels = true
                                },
                                new PieSeries
                                {
                                    Title = "Opera",
                                    Values = new ChartValues<ObservableValue> { new ObservableValue(10) },
                                    DataLabels = true
                                },
                                new PieSeries
                                {
                                    Title = "Explorer",
                                    Values = new ChartValues<ObservableValue> { new ObservableValue(4) },
                                    DataLabels = true
                                }

                            };

                            chart.Series = SeriesCollection2;
                        }
                        aux.RenderTransform = new TranslateTransform
                        {
                            X = graph.posicionX,
                            Y = graph.posicionY
                        };

                        canvasPreview.Children.Add(chart);


                    }
                    break;
                case 4:
                    {
                        Grid aux = new Grid();
                        Border border = new Border
                        {
                            BorderBrush = Brushes.Gray,
                            BorderThickness = new Thickness(1)
                        };
                        aux.Children.Add(border);
                        aux.Background = Brushes.Black;
                        aux.Width = 250;
                        aux.Height = 250;

                        PieChart chart = new PieChart();
                        chart.InnerRadius = 60;
                        ChartValues<ObservableValue> valores = new ChartValues<ObservableValue>();
                        if (graph.ejex != "" && graph.ejey != "" && graph.query != "")
                        {
                            SeriesCollection SeriesCollection1 = new SeriesCollection();
                            List<string> Labels1 = new List<string>();
                            bool error = false;
                            DataTable dt = conexion.getRegistros(graph.query, datas.nombre, ref error);

                            foreach (DataRow dr in dt.Rows)
                            {
                                SeriesCollection1.Add(new PieSeries
                                {
                                    Title = dr[graph.ejex].ToString(),
                                    Values = new ChartValues<ObservableValue> { new ObservableValue(Convert.ToDouble(dr[graph.ejey].ToString())) },
                                    DataLabels = true
                                });
                            }



                            DataContext = this;


                            chart.Series = SeriesCollection1;
                        }
                        else
                        {
                            SeriesCollection SeriesCollection2 = new SeriesCollection {
                                new PieSeries
                                {
                                    Title = "Chrome",
                                    Values = new ChartValues<ObservableValue> { new ObservableValue(8) },
                                    DataLabels = true
                                },
                                new PieSeries
                                {
                                    Title = "Mozilla",
                                    Values = new ChartValues<ObservableValue> { new ObservableValue(6) },
                                    DataLabels = true
                                },
                                new PieSeries
                                {
                                    Title = "Opera",
                                    Values = new ChartValues<ObservableValue> { new ObservableValue(10) },
                                    DataLabels = true
                                },
                                new PieSeries
                                {
                                    Title = "Explorer",
                                    Values = new ChartValues<ObservableValue> { new ObservableValue(4) },
                                    DataLabels = true
                                }

                            };

                            chart.Series = SeriesCollection2;
                        }
                        aux.RenderTransform = new TranslateTransform
                        {
                            X = graph.posicionX,
                            Y = graph.posicionY
                        };

                        canvasPreview.Children.Add(chart);


                    }
                    break;
                default:
                    {

                    }
                    break;
            }

        }



        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            //MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.sp.Speak("Acción Cancelada");
            DetalleDashBoard datasourcelista = new DetalleDashBoard(dashboardida);
            this.NavigationService.Navigate(datasourcelista);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            DetalleDashBoard dashboard = new DetalleDashBoard();
            MainWindow.sp.Speak("Acción Cancelada");
            this.NavigationService.Navigate(dashboard);
        }

        private void comboBox_DropDownClosed(object sender, EventArgs e)
        {
            if (cbXdataviews.SelectedIndex != -1)
            {
                Conexion cn = new Conexion();
                DataSource dv = new DataSource();
                DataTable dbs = cn.GetDataViews();

                dv.DataTabletoListView(dbs);
                List<DataView> lstDv = dv.dataview;
                DataView dataview = lstDv.Where(x => x.DsNombre.Equals(cbXdataviews.Text)).Single();

                bool error = false;

                DataTable tabla = cn.getRegistros(dataview.Query, dataview.nombre, ref error);
                dt = tabla;
                List<String> Columns = new List<string>();
                foreach (DataColumn column in tabla.Columns)
                {
                    Columns.Add(column.ColumnName);
                }
                EjeX.ItemsSource = Columns;
                EjeY.ItemsSource = Columns;

                EjeX.SelectedIndex = 1;
                EjeY.SelectedIndex = 1;

                dataGrid.ItemsSource = tabla.DefaultView;
                dataGrid.AutoGenerateColumns = true;
                dataGrid.CanUserAddRows = false;
            }
            else
            {
                MainWindow.sp.Speak("No hay vista de datos Disponibles");
                MainWindow.AlertaCustom("Hubo un problema con la consulta", "No hay vistas de datos disponibles");
            }

        }

        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            dibujar();
            MainWindow.sp.Speak("Previsualización del gráfico generada");
            MainWindow.AlertaCustom("Previsuaización generada con éxito", "La previsuaización del gráfico se generado satisfactoriamente");
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                int ejexid = EjeX.SelectedIndex;
                int ejeyid = EjeY.SelectedIndex;

                if (ejexid != -1 && ejeyid != -1)
                {
                    Conexion cn = new Conexion();
                    DataSource dv = new DataSource();
                    DataTable dbs = cn.GetDataViews();

                    dv.DataTabletoListView(dbs);
                    List<DataView> lstDv = dv.dataview;
                    DataView dataview = lstDv.Where(x => x.DsNombre.Equals(cbXdataviews.Text)).Single();
                    MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
                    MainWindow._recognizer.RecognizeAsyncStop();
                    MainWindow.AlertaExito();
                    cn.updateGraphic(dataview.Query, EjeX.Text, EjeY.Text, graficoid);
                    MainWindow.sp.Speak("El gráfico se guardo exitosamente");
                    DetalleDashBoard datasourcelista = new DetalleDashBoard(dashboardida);
                    this.NavigationService.Navigate(datasourcelista);
                }
                else
                {
                    MainWindow.sp.Speak("No hubieron cambios en el gráfico");
                    DetalleDashBoard datasourcelista = new DetalleDashBoard(dashboardida);
                    this.NavigationService.Navigate(datasourcelista);
                }

            }
            catch
            {
                MainWindow.AlertaFaltanDatos("Hubo un problema con su solicitud");
            }
        }

        private void btnVolver_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (graficoid != 0)
                {
                    Conexion cn = new Conexion();

                    cn.deleteGrafico(graficoid);
                    MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
                    MainWindow._recognizer.RecognizeAsyncStop();
                    MainWindow.sp.Speak("Gráfico Eliminado");
                    MainWindow.AlertaExito();
                    DetalleDashBoard datasourcelista = new DetalleDashBoard(dashboardida);
                    this.NavigationService.Navigate(datasourcelista);
                }
                else
                {
                    MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
                    MainWindow._recognizer.RecognizeAsyncStop();
                    MainWindow.sp.Speak("No hubieron cambios en el gráfico");
                    DetalleDashBoard datasourcelista = new DetalleDashBoard(dashboardida);
                    this.NavigationService.Navigate(datasourcelista);
                }

            }
            catch
            {
                MainWindow.AlertaFaltanDatos("Hubo un problema con su solicitud");
            }
        }
    }
}
