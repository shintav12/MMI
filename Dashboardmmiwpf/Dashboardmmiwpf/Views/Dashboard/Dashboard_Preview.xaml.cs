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
using WpfKb.Controls;

namespace Dashboardmmiwpf
{
    /// <summary>
    /// Lógica de interacción para Dashboard_Preview.xaml
    /// </summary>
    public partial class Dashboard_Preview : Page
    {
        private FloatingTouchScreenKeyboard VKeyboard = new FloatingTouchScreenKeyboard();
        private int DashboardId = 0;
        private Dashboard dasglob = new Dashboard();
        public Dashboard_Preview(int dashId)
        {
            InitializeComponent();
            DashboardId = dashId;
            Projects projects = new Projects();
            Dashboard dash = new Dashboard();
            Conexion conexion = new Conexion();
            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;

            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("editar", "eliminar", "cancelar");
            grammarBuilder.Append(commandChoices);

            Choices valueChoices = new Choices();
            valueChoices.Add("dashboard");
            valueChoices.Add("accion");
            grammarBuilder.Append(valueChoices);
            DashboardName.Text = dash.Nombre;
            MainWindow._recognizer.UnloadAllGrammars();
            MainWindow._recognizer.LoadGrammar(new Grammar(grammarBuilder));
            MainWindow._recognizer.RecognizeAsync(RecognizeMode.Multiple);
            projects.DataTabletoListView(conexion.GetDashboardIndex(DashboardId));
            dash = projects.lstDashboard.First();
            DashboardName.Text = dash.Nombre;
            dasglob = dash;
            Grilla();
            Dibujar();
        }

        public void Grilla()
        {
            for (int i = 0; i < 7; i++)
            {
                Line line = new Line();

                line.Visibility = System.Windows.Visibility.Visible;
                line.StrokeThickness = 2;
                line.Stroke = System.Windows.Media.Brushes.Black;
                line.X1 = (250 * i) + (0.5 * i);
                line.X2 = (250 * i) + (0.5 * i);
                line.Y1 = 0;
                line.Y2 = 757; //606
                Charts.Children.Add(line);
            }

            for (int i = 0; i < 3; i++)
            {
                Line line = new Line();

                line.Visibility = System.Windows.Visibility.Visible;
                line.StrokeThickness = 2;
                line.Stroke = System.Windows.Media.Brushes.Black;
                line.X1 = 0;
                line.X2 = 1503;
                line.Y1 = (250 * i) + (0.5 * i);
                line.Y2 = (250 * i) + (0.5 * i);
                Charts.Children.Add(line);
            }
        }

        public void Agregar_Barras(Grafico graph, Conexion conexion, DataSource datas)
        {
            Grid aux = new Grid();
            Border border = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };
            aux.Children.Add(border);
            aux.Background = Brushes.White;
            aux.Width = graph.ancho;
            aux.Height = graph.alto;

            CartesianChart chart = new CartesianChart();

            ChartValues<Double> valores = new ChartValues<double>();
            Func<double, string> Formatter = valuee => valuee.ToString("N");

            if (graph.ejex != "" && graph.ejey != "" && graph.query != "")
            {
                SeriesCollection SeriesCollection1 = new SeriesCollection();
                List<string> Labels1 = new List<string>();
                bool error = false;
                DataTable dt = conexion.getRegistros(graph.query, datas.nombre, ref error);

                foreach (DataRow dr in dt.Rows)
                {

                    valores.Add(Convert.ToDouble(dr[graph.ejey].ToString()));
                    Labels1.Add(dr[graph.ejex].ToString());
                }
                SeriesCollection1.Add(new ColumnSeries
                {
                    Values = valores
                });


                DataContext = this;

                chart.AxisY = new AxesCollection { new Axis { Title = "", LabelFormatter = Formatter } };

                chart.Series = SeriesCollection1;
                chart.AxisX = new AxesCollection  {
                                    new Axis { Title = "" , Labels = Labels1 }
                                };
            }
            else
            {
                SeriesCollection SeriesCollection1 = new SeriesCollection
                                {
                                    new ColumnSeries
                                    {
                                        Title = "2015",
                                        Values = new ChartValues<double> { 10, 50, 39, 50 }
                                    },

                                    new ColumnSeries
                                    {
                                        Title = "2016",
                                        Values = new ChartValues<double> { 11, 56, 42, 48 }
                                    }
                                };

                string[] Labels1 = new[] { "Maria", "Susan", "Charles", "Frida" };

                SeriesCollection1.Add(new ColumnSeries
                {
                    Values = valores
                });

                DataContext = this;

                chart.AxisY = new AxesCollection { new Axis { Title = "TituloY", LabelFormatter = Formatter } };

                chart.Series = SeriesCollection1;
                chart.AxisX = new AxesCollection  {
                                    new Axis { Title = "TituloX" , Labels = Labels1 }
                                };
            }


            aux.RenderTransform = new TranslateTransform
            {
                X = graph.posicionX,
                Y = graph.posicionY
            };

            chart.AxisY = new AxesCollection { new Axis { Title = "", LabelFormatter = Formatter } };
            aux.Children.Add(chart);
            Charts.Children.Add(aux);
        }
        public void Agregar_Filas(Grafico graph, Conexion conexion, DataSource datas)
        {
            Grid aux = new Grid();
            Border border = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };
            aux.Children.Add(border);
            aux.Background = Brushes.White;
            aux.Width = graph.ancho;
            aux.Height = graph.alto;

            CartesianChart chart = new CartesianChart();

            ChartValues<Double> valores = new ChartValues<double>();
            Func<double, string> Formatter = valuee => valuee.ToString("N");

            if (graph.ejex != "" && graph.ejey != "" && graph.query != "")
            {
                SeriesCollection SeriesCollection1 = new SeriesCollection();
                List<string> Labels1 = new List<string>();
                bool error = false;
                DataTable dt = conexion.getRegistros(graph.query, datas.nombre, ref error);

                foreach (DataRow dr in dt.Rows)
                {

                    valores.Add(Convert.ToDouble(dr[graph.ejey].ToString()));
                    Labels1.Add(dr[graph.ejex].ToString());
                }
                SeriesCollection1.Add(new RowSeries
                {
                    Values = valores
                });


                DataContext = this;

                chart.AxisY = new AxesCollection { new Axis { Title = "", LabelFormatter = Formatter } };

                chart.Series = SeriesCollection1;
                chart.AxisX = new AxesCollection  {
                                    new Axis { Title = "" , Labels = Labels1 }
                                };
            }
            else
            {
                SeriesCollection SeriesCollection1 = new SeriesCollection
                                {
                                    new RowSeries
                                    {
                                        Title = "2015",
                                        Values = new ChartValues<double> { 10, 50, 39, 50 }
                                    },

                                    new RowSeries
                                    {
                                        Title = "2016",
                                        Values = new ChartValues<double> { 11, 56, 42, 48 }
                                    }
                                };

                string[] Labels1 = new[] { "Maria", "Susan", "Charles", "Frida" };

                SeriesCollection1.Add(new RowSeries
                {
                    Values = valores
                });

                DataContext = this;

                chart.AxisX = new AxesCollection { new Axis { Title = "TituloY", LabelFormatter = Formatter } };

                chart.Series = SeriesCollection1;
                chart.AxisY = new AxesCollection  {
                                    new Axis { Title = "TituloX" , Labels = Labels1 }
                                };
            }


            aux.RenderTransform = new TranslateTransform
            {
                X = graph.posicionX,
                Y = graph.posicionY
            };

            chart.AxisY = new AxesCollection { new Axis { Title = "", LabelFormatter = Formatter } };
            aux.Children.Add(chart);
            Charts.Children.Add(aux);
        }
        public void Agregar_Pie(Grafico graph, Conexion conexion, DataSource datas)
        {
            Grid aux = new Grid();
            Border border = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };
            aux.Children.Add(border);
            aux.Background = Brushes.White;
            aux.Width = graph.ancho;
            aux.Height = graph.alto;

            PieChart chart = new PieChart();

            bool error = true;
            DataTable dt = conexion.getRegistros(graph.query, datas.nombre, ref error);


            if (graph.ejex != "" && graph.ejey != "" && graph.query != "")
            {
                SeriesCollection SeriesCollection1 = new SeriesCollection();

                foreach (DataRow dr in dt.Rows)
                {
                    SeriesCollection1.Add(new PieSeries
                    {
                        Title = dr[graph.ejex].ToString(),
                        Values = new ChartValues<ObservableValue> { new ObservableValue(Convert.ToDouble(dr[graph.ejey].ToString())) },
                        DataLabels = true
                    });
                }

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
            aux.Children.Add(chart);
            Charts.Children.Add(aux);
        }
        public void Agregar_Donut(Grafico graph, Conexion conexion, DataSource datas)
        {
            Grid aux = new Grid();
            Border border = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1)
            };
            aux.Children.Add(border);
            aux.Background = Brushes.White;
            aux.Width = graph.ancho;
            aux.Height = graph.alto;

            PieChart chart = new PieChart();
            chart.InnerRadius = 50;
            bool error = true;
            DataTable dt = conexion.getRegistros(graph.query, datas.nombre, ref error);


            if (graph.ejex != "" && graph.ejey != "" && graph.query != "")
            {
                SeriesCollection SeriesCollection1 = new SeriesCollection();

                foreach (DataRow dr in dt.Rows)
                {
                    SeriesCollection1.Add(new PieSeries
                    {
                        Title = dr[graph.ejex].ToString(),
                        Values = new ChartValues<ObservableValue> { new ObservableValue(Convert.ToDouble(dr[graph.ejey].ToString())) },
                        DataLabels = true
                    });
                }

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
            aux.Children.Add(chart);
            Charts.Children.Add(aux);
        }

        private void Dibujar()
        {
            Conexion conexion = new Conexion();
            Grafico graficoTransf = new Grafico();
            DataSource datas = new DataSource();
            List<Grafico> graficoList = new List<Grafico>();
            graficoList = graficoTransf.DataTabletoList(conexion.GetGraphics(DashboardId));
            List<DataSource> ds = datas.DataTabletoList(conexion.GetDataSource());

            datas = ds.Where(x => x.id == dasglob.DataSourceId).FirstOrDefault();

            foreach (Grafico graph in graficoList)
            {
                switch (graph.tipoGrafico)
                {
                    case 1:
                        {
                            Agregar_Barras(graph, conexion, datas);
                        }
                        break;
                    case 2:
                        {
                            Agregar_Filas(graph, conexion, datas);
                        }
                        break;
                    case 4:
                        {
                            Agregar_Donut(graph, conexion, datas);
                        }
                        break;
                    case 3:
                        {
                            Agregar_Pie(graph, conexion, datas);
                        }
                        break;
                    default:
                        {

                        }
                        break;
                }
            }
            DataContext = this;
        }

        private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Words.Count == 2)
            {
                string command = e.Result.Words[0].Text.ToLower();
                string value = e.Result.Words[1].Text.ToLower();
                switch (command)
                {
                    case "editar":
                        switch (value)
                        {
                            case "dashboard":

                                break;
                        }
                        break;
                    case "eliminar":
                        switch (value)
                        {
                            case "dashboard":

                                break;
                        }
                        break;
                    case "cancelar":
                        switch (value)
                        {
                            case "acción":

                                break;
                        }
                        break;
                }
            }
        }

        private void btnEditar_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.sp.Speak("Modifique el dashboard según lo necesite");
            DetalleDashBoard datasourcelista = new DetalleDashBoard(DashboardId);
            this.NavigationService.Navigate(datasourcelista);
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

        

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.sp.Speak("Acción Cancelada");
            DashboardLista datasourcelista = new DashboardLista();
            this.NavigationService.Navigate(datasourcelista);
        }
    }
}
