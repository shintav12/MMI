using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Data;
using LiveCharts.Defaults;
using Dashboardmmiwpf.Enums;

namespace Dashboardmmiwpf
{
    /// <summary>
    /// Interaction logic for DetalleDashBoard.xaml
    /// </summary>
    public partial class DetalleDashBoard : Page
    {
        Shape temp;
        Point m_start;
        Vector m_startOffset;
        int GraficoSeleccionadoId = -1;
        List<Grid> graficos;
        List<Grafico> GraficosLisa;
        double pos_x = 0;
        double pos_y = 0;
        private static bool Locked = false;
        private int Type = 0;
        bool shapelock = false;
        IList<Body> _bodies;
        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        Dashboard dasglob = new Dashboard();
        private int posx = 0;
        private int posy = 0;
        private int id = 0;

        int dashboardId = 0;

        public string[] Labels2 { get; set; }

        public DetalleDashBoard()
        {
            InitializeComponent();
            MainWindow.page = "DetalleDashBoard";
            //this.DataContext = new DetalleDashBoard.Shared.TestPageViewModel();
        }

        public DetalleDashBoard(int DashboardId)
        {
            InitializeComponent();
            id = DashboardId;
            graficos = new List<Grid>();
            _sensor = KinectSensor.GetDefault();
            Projects projects = new Projects();
            Dashboard dash = new Dashboard();
            Conexion conexion = new Conexion();
            MainWindow._recognizer.SetInputToDefaultAudioDevice();
            MainWindow._recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;

            GraficosLisa = new List<Grafico>();

            dashboardId = DashboardId;

            if (_sensor != null)
            {
                _sensor.Open();

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("crear","cancelar","eliminar", "cambiar","editar","guardar");
            grammarBuilder.Append(commandChoices);

            Choices valueChoices = new Choices();
            valueChoices.Add("tamaño");
            valueChoices.Add("accion");
            valueChoices.Add("barras","pai","dona","filas","area","medidor","tacometro","linea");
            valueChoices.Add("gráfico");
            grammarBuilder.Append(valueChoices);

            MainWindow._recognizer.UnloadAllGrammars();
            MainWindow._recognizer.LoadGrammar(new Grammar(grammarBuilder));
            MainWindow._recognizer.RecognizeAsync(RecognizeMode.Multiple);
            projects.DataTabletoListView(conexion.GetDashboardIndex(DashboardId));
            dash = projects.lstDashboard.First();
            dasglob = dash;
            MainWindow.page = "DetalleDashBoard";
            DashboardName.Text = dash.Nombre;

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

        private void Dibujar()
        {
            Conexion conexion = new Conexion();
            Grafico graficoTransf = new Grafico();
            DataSource datas = new DataSource();
            List<Grafico> graficoList = new List<Grafico>();
            graficoList = graficoTransf.DataTabletoList(conexion.GetGraphics(dashboardId));
            List<DataSource> ds = datas.DataTabletoList(conexion.GetDataSource());

            datas = ds.Where(x => x.id == dasglob.DataSourceId).FirstOrDefault();

            foreach (Grafico graph in graficoList)
            {
                GraficosLisa.Add(graph);
                switch (graph.tipoGrafico)
                {
                    case (int)ChartTypes.Barras:
                        {
                            Agregar_Barras(graph, conexion, datas);
                        }
                        break;
                    case (int)ChartTypes.Filas:
                        {
                            Agregar_Filas(graph, conexion, datas);
                        }
                        break;
                    case (int)ChartTypes.Dona:
                        {
                            Agregar_Donut(graph, conexion, datas);
                        }
                        break;
                    case (int)ChartTypes.Pie:
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

        private void GraficoGridMouseDown(object sender, MouseButtonEventArgs e)
        {
            Grid aux = (Grid)sender;
            TranslateTransform taux = (TranslateTransform)aux.RenderTransform;

            int id = graficos.IndexOf(aux);
            GraficoSeleccionadoId = id;
            aux.Background = Brushes.Gray;
            for (int i = 0; i < graficos.Count; i++)
            {
                if (id != i)
                {
                    graficos[i].Background = Brushes.White;
                }
            }
            m_start = e.GetPosition(Charts);
            m_startOffset = new Vector(taux.X, taux.Y);
            aux.CaptureMouse();
        }

        private void GraficoGridMouseMove(object sender, MouseEventArgs e)
        {
            Grid aux = (Grid)sender;
            if (aux.IsMouseCaptured && !shapelock)
            {
                double ancho = aux.Width;
                double alto = aux.Height;
                Vector offset = Point.Subtract(e.GetPosition(Charts), m_start);

                TranslateTransform taux = (TranslateTransform)aux.RenderTransform;
                if (m_startOffset.X + offset.X >= -625 && m_startOffset.X + offset.X <= 625)
                    taux.X = m_startOffset.X + offset.X;
                if (m_startOffset.Y + offset.Y >= -252 && m_startOffset.Y + offset.Y <= 250)
                    taux.Y = m_startOffset.Y + offset.Y;
            }
        }

        private void GraficoGridMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!shapelock)
            {
                Grid aux = (Grid)sender;
                Vector offset = Point.Subtract(e.GetPosition(Charts), m_start);
                TranslateTransform taux = (TranslateTransform)aux.RenderTransform;
                double posx = m_startOffset.X + offset.X;
                double posy = m_startOffset.Y + offset.Y;
                double ancho = aux.Width;
                double alto = aux.Height;

                double j = posy / 250;
                double i = posx / 250;

                int neg = 1;
                if (i < 0) neg = -1;

                int iint = Convert.ToInt32(Math.Abs(i));

                iint = (iint + 1) * neg;

                posx = iint * 250 - (neg * 125);

                if (posx < -750) posx = -625;
                if (posx > 750) posx = 625;


                if (j > -1 && j < 1)
                {
                    j = 0;
                    posy = -1;
                }
                else if (j < -1)
                {
                    posy = -252;
                }
                else
                {
                    posy = 250;
                }


                int i_width = Convert.ToInt32(ancho / 250);

                taux.Y = posy;
                taux.X = posx + (i_width - 1) * 125;

                Conexion conexion = new Conexion();
                conexion.updategraphicPosition(taux.X, taux.Y, ancho, alto, GraficosLisa[GraficoSeleccionadoId].GraficoId);

                aux.ReleaseMouseCapture();
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
                    case "crear":

                        switch (value)
                        {
                            case "barras":
                                Crear_GraficoBarras();
                                MainWindow.sp.Speak("Gráfico de barras Creado");
                                break;
                            case "pai":
                                Crear_GraficoPie();
                                MainWindow.sp.Speak("Gráfico de pai Creado");
                                break;
                            case "dona":
                                Crear_GraficoDona();
                                MainWindow.sp.Speak("Gráfico de dona Creado");
                                break;
                            case "filas":
                                Crear_GraficoFilas();
                                MainWindow.sp.Speak("Gráfico de filas Creado");
                                break;
                            case "area":
                                Crear_GraficoFilas();
                                MainWindow.sp.Speak("Gráfico de Área Creado");
                                break;
                            case "medidor":
                                Crear_GraficoFilas();
                                MainWindow.sp.Speak("Medidor Creado");
                                break;
                            case "tacometro":
                                Crear_GraficoFilas();
                                MainWindow.sp.Speak("Tacómetro Creado");
                                break;
                            case "linea":
                                Crear_GraficoFilas();
                                MainWindow.sp.Speak("Gráfico de linea Creado");
                                break;
                        }
                        break;
                    case "eliminar":
                        switch (value)
                        {
                            case "gráfico":
                                try
                                {
                                    Grafico graph = GraficosLisa[GraficoSeleccionadoId];
                                    Grid aux = graficos[GraficoSeleccionadoId];
                                    if (graph.GraficoId != 0)
                                    {
                                        Conexion cn = new Conexion();
                                        Charts.Children.Remove(aux);
                                        graficos.Remove(aux);
                                        cn.deleteGrafico(graph.GraficoId);
                                        MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
                                        MainWindow._recognizer.RecognizeAsyncStop();
                                        MainWindow.sp.Speak("Gráfico Eliminado");
                                        break;
                                    }

                                }
                                catch
                                {
                                    MainWindow.AlertaFaltanDatos("Hubo un problema con su solicitud");
                                }

                                break;
                            default:

                                Locked = false;
                                break;
                        }
                        break;
                    case "cambiar":
                        switch (value)
                        {
                            case "tamaño":
                                shapelock = true;
                                MainWindow.sp.Speak("Edicion Activada");
                                TranslateTransform taux = (TranslateTransform)graficos[GraficoSeleccionadoId].RenderTransform;
                                pos_x = taux.X;
                                pos_y = taux.Y;
                                break;
                        }
                        break;
                    case "guardar":
                        switch (value)
                        {
                            case "tamaño":
                                shapelock = false;
                                MainWindow.sp.Speak("Edicion Terminada");
                                break;
                        }
                        break;
                    case "editar":
                        switch (value)
                        {
                            case "gráfico":
                                if (GraficoSeleccionadoId != -1)
                                {
                                    MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
                                    MainWindow._recognizer.RecognizeAsyncStop();
                                    MainWindow.sp.Speak("Edite la información del gráfico");
                                    DetalleDashboard_Dataview MenuPrincipal = new DetalleDashboard_Dataview(id, GraficosLisa[GraficoSeleccionadoId].GraficoId);
                                    foreach (Window window in Application.Current.Windows)
                                    {
                                        if (window.GetType() == typeof(MainWindow))
                                        {
                                            (window as MainWindow).frame.NavigationService.Navigate(MenuPrincipal);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    MainWindow.sp.Speak("Seleccione un Gráfico");
                                }
                                break;
                        }
                        break;
                }
            }
        }

        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    _bodies = new Body[frame.BodyFrameSource.BodyCount];
                    frame.GetAndRefreshBodyData(_bodies);
                    foreach (var body in _bodies)
                    {
                        if (body != null)
                        {
                            if (body.IsTracked)
                            {
                                Joint izquierda = body.Joints[JointType.HandLeft];
                                Joint derecha = body.Joints[JointType.HandRight];
                                var positionizq = izquierda.Position;
                                var positionder = derecha.Position;


                                if (shapelock && body.HandRightState.Equals(HandState.Closed) && body.HandLeftState.Equals(HandState.Closed))
                                {


                                    int distanciax = Math.Abs(Convert.ToInt32(Math.Pow((positionder.X * 100 - positionizq.X * 100), 2) / 100)) - 4;
                                    int distanciay = Math.Abs(Convert.ToInt32(Math.Pow((positionder.Y * 100 - positionizq.Y * 100), 2) / 100)) - 4;
                                    if (distanciax > 3) distanciax = 3;
                                    if (distanciay > 3) distanciay = 3;

                                    double ancho = distanciax * 250;
                                    double alto = distanciay * 250;
                                    int i_width = Convert.ToInt32(ancho / 250);
                                    TranslateTransform taux = (TranslateTransform)graficos[GraficoSeleccionadoId].RenderTransform;

                                    if (ancho >= 250 && taux.X < 650)
                                    {
                                        graficos[GraficoSeleccionadoId].Width = ancho;
                                    }
                                    if (alto >= 250 && taux.Y < 250)
                                    {
                                        graficos[GraficoSeleccionadoId].Height = alto;
                                    }
                                    taux.X = pos_x + (i_width - 1) * 125;
                                    taux.Y = pos_y;

                                    Conexion conexion = new Conexion();
                                    conexion.updategraphicPosition(taux.X, taux.Y, ancho, alto, GraficosLisa[GraficoSeleccionadoId].GraficoId);
                                }
                            }
                        }
                    }
                }
            }

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= this.speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncCancel();
            MainWindow.sp.Speak("Vista Previa del Dashboard Generada");
            Dashboard_Preview dashboardlista = new Dashboard_Preview(dashboardId);
            this.NavigationService.Navigate(dashboardlista);
        }

        
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            NewDashboard MenuPrincipal = new NewDashboard(id);
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).frame.NavigationService.Navigate(MenuPrincipal);
                    break;
                }
            }
        }

        private void btnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (GraficoSeleccionadoId != -1)
            {
                MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
                MainWindow._recognizer.RecognizeAsyncStop();
                MainWindow.sp.Speak("Edite la información del gráfico");
                if(GraficosLisa[GraficoSeleccionadoId].tipoGrafico == 1)
                {
                    DetalleDashboard_Dataview MenuPrincipal = new DetalleDashboard_Dataview(id, GraficosLisa[GraficoSeleccionadoId].GraficoId);
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.GetType() == typeof(MainWindow))
                        {
                            (window as MainWindow).frame.NavigationService.Navigate(MenuPrincipal);
                            break;
                        }
                    }
                }
                else if(GraficosLisa[GraficoSeleccionadoId].tipoGrafico == 2)
                {
                    Filas_Editar MenuPrincipal = new Filas_Editar(id, GraficosLisa[GraficoSeleccionadoId].GraficoId);
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.GetType() == typeof(MainWindow))
                        {
                            (window as MainWindow).frame.NavigationService.Navigate(MenuPrincipal);
                            break;
                        }
                    }
                }
                else if (GraficosLisa[GraficoSeleccionadoId].tipoGrafico == 3 || GraficosLisa[GraficoSeleccionadoId].tipoGrafico == 4)
                {
                    Dona_Pie_Editar MenuPrincipal = new Dona_Pie_Editar(id, GraficosLisa[GraficoSeleccionadoId].GraficoId);
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.GetType() == typeof(MainWindow))
                        {
                            (window as MainWindow).frame.NavigationService.Navigate(MenuPrincipal);
                            break;
                        }
                    }
                }

            }
            else
            {
                MainWindow.sp.Speak("Seleccione un Gráfico");
            }   
        }

        private void Crear_GraficoBarras()
        {
            Conexion conexion = new Conexion();
            Grid aux = new Grid();
            Border border = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1)
            };
            aux.Children.Add(border);
            aux.MouseDown += GraficoGridMouseDown;
            aux.MouseUp += GraficoGridMouseUp;
            aux.MouseMove += GraficoGridMouseMove;
            aux.Background = Brushes.White;
            aux.Width = 250;
            aux.Height = 250;

            CartesianChart chart = new CartesianChart();
            ChartValues<Double> valores = new ChartValues<double>();

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
            Func<double, string> Formatter = valuee => valuee.ToString("N");
            DataContext = this;

            chart.AxisY = new AxesCollection { new Axis { Title = "TituloY", LabelFormatter = Formatter } };

            chart.Series = SeriesCollection1;
            chart.AxisX = new AxesCollection  {
                                new Axis { Title = "TituloX" , Labels = Labels1 }
                            };

            Point p = Mouse.GetPosition(Charts);

            double posx = p.X - 700;
            double posy = (p.Y - 377) *-1;
            double j = posy / 250;
            double i = posx / 250;

            int neg = 1;
            if (i < 0) neg = -1;

            int iint = Convert.ToInt32(Math.Abs(i));

            iint = (iint + 1) * neg;

            posx = iint * 250 - (neg * 125);

            if (posx < -750) posx = -625;
            if (posx > 750) posx = 625;


            if (j > -1 && j < 1)
            {
                j = 0;
                posy = -1;
            }
            else if (j < -1)
            {
                posy = -252;
            }
            else
            {
                posy = 250;
            }


            aux.RenderTransform = new TranslateTransform
            {
                X = posx,
                Y = -posy
            };



            chart.AxisY = new AxesCollection { new Axis { Title = "Tituloy", LabelFormatter = Formatter } };
            aux.Children.Add(chart);
            graficos.Add(aux);
            Charts.Children.Add(aux);

            conexion.GuardarGrafico(dashboardId, (int)ChartTypes.Barras, -posx, -posy);
            Grafico graficotrans = new Grafico();
            graficotrans = graficotrans.DataTabletoList(conexion.GetGraphics(dashboardId)).Last();
            GraficosLisa.Add(graficotrans);
        }

        private void Crear_GraficoFilas()
        {
            Conexion conexion = new Conexion();
            Grid aux = new Grid();
            Border border = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1)
            };
            aux.Children.Add(border);
            aux.MouseDown += GraficoGridMouseDown;
            aux.MouseUp += GraficoGridMouseUp;
            aux.MouseMove += GraficoGridMouseMove;
            aux.Background = Brushes.White;
            aux.Width = 250;
            aux.Height = 250;

            CartesianChart chart = new CartesianChart();
            ChartValues<Double> valores = new ChartValues<double>();

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
            Func<double, string> Formatter = valuee => valuee.ToString("N");
            DataContext = this;

            chart.AxisX = new AxesCollection { new Axis { Title = "TituloX", Labels = Labels1 }  };

            chart.Series = SeriesCollection1;
            chart.AxisX = new AxesCollection  {
                                new Axis { Title = "TituloY", LabelFormatter = Formatter }
                            };

            Point p = Mouse.GetPosition(Charts);

            double posx = p.X - 700;
            double posy = (p.Y - 377) * -1;
            double j = posy / 250;
            double i = posx / 250;

            int neg = 1;
            if (i < 0) neg = -1;

            int iint = Convert.ToInt32(Math.Abs(i));

            iint = (iint + 1) * neg;

            posx = iint * 250 - (neg * 125);

            if (posx < -750) posx = -625;
            if (posx > 750) posx = 625;


            if (j > -1 && j < 1)
            {
                j = 0;
                posy = -1;
            }
            else if (j < -1)
            {
                posy = -252;
            }
            else
            {
                posy = 250;
            }


            aux.RenderTransform = new TranslateTransform
            {
                X = posx,
                Y = -posy
            };



            chart.AxisY = new AxesCollection { new Axis { Title = "Tituloy", LabelFormatter = Formatter } };
            aux.Children.Add(chart);
            graficos.Add(aux);
            Charts.Children.Add(aux);

            conexion.GuardarGrafico(dashboardId, (int)ChartTypes.Filas, -posx, -posy);
            Grafico graficotrans = new Grafico();
            graficotrans = graficotrans.DataTabletoList(conexion.GetGraphics(dashboardId)).Last();
            GraficosLisa.Add(graficotrans);
        }

        private void Crear_GraficoPie()
        {
            Conexion conexion = new Conexion();
            Grid aux = new Grid();
            Border border = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1)
            };
            aux.Children.Add(border);
            aux.MouseDown += GraficoGridMouseDown;
            aux.MouseUp += GraficoGridMouseUp;
            aux.MouseMove += GraficoGridMouseMove;
            aux.Background = Brushes.White;
            aux.Width = 250;
            aux.Height = 250;


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

            PieChart piechart = new PieChart();
            piechart.Series = SeriesCollection2;

            Point p = Mouse.GetPosition(Charts);

            double posx = p.X - 700;
            double posy = (p.Y - 377) * -1;
            double j = posy / 250;
            double i = posx / 250;

            int neg = 1;
            if (i < 0) neg = -1;

            int iint = Convert.ToInt32(Math.Abs(i));

            iint = (iint + 1) * neg;

            posx = iint * 250 - (neg * 125);

            if (posx < -750) posx = -625;
            if (posx > 750) posx = 625;


            if (j > -1 && j < 1)
            {
                j = 0;
                posy = -1;
            }
            else if (j < -1)
            {
                posy = -252;
            }
            else
            {
                posy = 250;
            }


            aux.RenderTransform = new TranslateTransform
            {
                X = posx,
                Y = -posy
            };

            aux.Children.Add(piechart);
            graficos.Add(aux);
            Charts.Children.Add(aux);

            conexion.GuardarGrafico(dashboardId, (int)ChartTypes.Pie, -posx, -posy);
            Grafico graficotrans = new Grafico();
            graficotrans = graficotrans.DataTabletoList(conexion.GetGraphics(dashboardId)).Last();
            GraficosLisa.Add(graficotrans);
        }

        private void Crear_GraficoDona()
        {
            Conexion conexion = new Conexion();
            Grid aux = new Grid();
            Border border = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1)
            };
            aux.Children.Add(border);
            aux.MouseDown += GraficoGridMouseDown;
            aux.MouseUp += GraficoGridMouseUp;
            aux.MouseMove += GraficoGridMouseMove;
            aux.Background = Brushes.White;
            aux.Width = 250;
            aux.Height = 250;


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

            PieChart piechart = new PieChart();
            piechart.Series = SeriesCollection2;
            piechart.InnerRadius = 50;

            Point p = Mouse.GetPosition(Charts);

            double posx = p.X - 700;
            double posy = (p.Y - 377) * -1;
            double j = posy / 250;
            double i = posx / 250;

            int neg = 1;
            if (i < 0) neg = -1;

            int iint = Convert.ToInt32(Math.Abs(i));

            iint = (iint + 1) * neg;

            posx = iint * 250 - (neg * 125);

            if (posx < -750) posx = -625;
            if (posx > 750) posx = 625;


            if (j > -1 && j < 1)
            {
                j = 0;
                posy = -1;
            }
            else if (j < -1)
            {
                posy = -252;
            }
            else
            {
                posy = 250;
            }


            aux.RenderTransform = new TranslateTransform
            {
                X = posx,
                Y = -posy
            };

            aux.Children.Add(piechart);
            graficos.Add(aux);
            Charts.Children.Add(aux);

            conexion.GuardarGrafico(dashboardId, (int)ChartTypes.Dona, -posx, -posy);
            Grafico graficotrans = new Grafico();
            graficotrans = graficotrans.DataTabletoList(conexion.GetGraphics(dashboardId)).Last();
            GraficosLisa.Add(graficotrans);
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
            aux.MouseDown += GraficoGridMouseDown;
            aux.MouseUp += GraficoGridMouseUp;
            aux.MouseMove += GraficoGridMouseMove;
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
            graficos.Add(aux);
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
            aux.MouseDown += GraficoGridMouseDown;
            aux.MouseUp += GraficoGridMouseUp;
            aux.MouseMove += GraficoGridMouseMove;
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
            graficos.Add(aux);
            Charts.Children.Add(aux);
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
            aux.MouseDown += GraficoGridMouseDown;
            aux.MouseUp += GraficoGridMouseUp;
            aux.MouseMove += GraficoGridMouseMove;
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
            graficos.Add(aux);
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
            aux.MouseDown += GraficoGridMouseDown;
            aux.MouseUp += GraficoGridMouseUp;
            aux.MouseMove += GraficoGridMouseMove;
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
            graficos.Add(aux);
            Charts.Children.Add(aux);
        }



















        private void button_Click(object sender, RoutedEventArgs e)
        {
            //    DetalleDashboard_Dataview dashboardataview = new DetalleDashboard_Dataview();
            //    this.NavigationService.Navigate(dashboardataview);
        }

        private void Circular_Click(object sender, RoutedEventArgs e)
        {
            Shape shape = new Ellipse();
            shape.AllowDrop = true;
            shape.Fill = Brushes.Blue;
            shape.Height = 75;
            shape.Width = 75;
            shape.CaptureMouse();

            //DragDropElement grafico = new DragDropElement();
            //Grafico barra = new Grafico(grafico, "CIRCULAR");
            //grafico.AllowDrop = true;
            //workSpace.Children.Add(grafico);
        }

        private void Barra_Click(object sender, RoutedEventArgs e)
        {
            //DragDropElement grafico = new DragDropElement();
            //Grafico barra = new Grafico(grafico, "BARRAS");
            //grafico.AllowDrop = true;
            //workSpace.Children.Add(grafico);
        }

        private void Embudo_Click(object sender, RoutedEventArgs e)
        {
            //DragDropElement grafico = new DragDropElement();
            //Grafico barra = new Grafico(grafico, "EMBUDO");
            //grafico.AllowDrop = true;
            //workSpace.Children.Add(grafico);
        }

        private void Donut_Click(object sender, RoutedEventArgs e)
        {
            //DragDropElement grafico = new DragDropElement();
            //Grafico barra = new Grafico(grafico, "CIRCULAR");
            //grafico.AllowDrop = true;
            //workSpace.Children.Add(grafico);
        }

        private void Apiladas_Click(object sender, RoutedEventArgs e)
        {
            //DragDropElement grafico = new DragDropElement();
            //Grafico barra = new Grafico(grafico, "APILADAS");
            //grafico.AllowDrop = true;
            //workSpace.Children.Add(grafico);
        }

        private void Area_Click(object sender, RoutedEventArgs e)
        {
            //dragging = true;
            //Shape shape = new Ellipse();
            //shape.AllowDrop = true;
            //shape.Fill = Brushes.Blue;
            //shape.Height = 75;
            //shape.Width = 75;
            //shape.MouseLeftButtonDown += new MouseButtonEventHandler(ShapeMouseLeftButtonDown);
            //shape.MouseLeftButtonUp += new MouseButtonEventHandler(ShapeMouseLeftButtonUp);
            //shape.MouseMove += new MouseEventHandler(ShapeMouseMove);
            //tipoGrafico = "circulo";

            //DragDropElement grafico = new DragDropElement();
            //Grafico barra = new Grafico(grafico, "ÁREA");
            //grafico.AllowDrop = true;
            //workSpace.Children.Add(shape);
        }

        private void EllipseCanvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void EllipseCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // get the position of the mouse relative to the Canvas

        }

        private void EllipseCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void SquareCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void CircleCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void RectangleCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void btnGuardar_Copy_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < GraficosLisa.Count; i++)
            {
                TranslateTransform taux = (TranslateTransform)graficos[i].RenderTransform;
                Conexion conexion = new Conexion();
                conexion.updategraphicPosition(taux.X, taux.Y, graficos[i].Width, graficos[i].Height, GraficosLisa[i].GraficoId);
            }
            MainWindow.sp.Speak("Dashboard Guardado");
        }
    }
}
