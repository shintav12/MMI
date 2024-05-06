using MahApps.Metro.Controls;
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

namespace Dashboardmmiwpf
{
    public partial class Lista_Dashboard : Page
    {

        private void Clicktile(object sender, RoutedEventArgs e)
        {
            DetalleDashBoard MenuPrincipal = new DetalleDashBoard();
            foreach (Window window in Application.Current.Windows)
            {
                if(window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).frame.NavigationService.Navigate(MenuPrincipal);
                    break;
                }
            }
        }

        public Lista_Dashboard()
        {
            Random x = new Random();
            InitializeComponent();
            Conexion conexion = new Conexion();
            Projects ds = new Projects();
            List<Projects> datasources = ds.DataTabletoList(conexion.GetProjects());

            for (int i = 0; i < datasources.Count; i++)
            {
                datasources[i].DataTabletoListView(conexion.GetDashboards(datasources[i].ProjectId));
            }

            for (int i = 0; i < datasources.Count; i++)
            {
                StackPanel auxnew = new StackPanel();
                auxnew.Name = "DS" + datasources[i].ProjectId.ToString();
                auxnew.Orientation = Orientation.Vertical;
                StackPanel auxnew2 = new StackPanel();
                auxnew2.Orientation = Orientation.Horizontal;
                Image img1 = new Image();
                BitmapImage bm1 = new BitmapImage();
                bm1.BeginInit();
                bm1.UriSource = new Uri(@"/Recursos/icon didactive-01.png", UriKind.Relative);
                bm1.EndInit();
                img1.Stretch = Stretch.Fill;
                img1.Source = bm1;
                img1.Height = 42;
                img1.Width = 42;



                Button auxtct = new Button();
                auxtct.Content = datasources[i].Nombre;
                auxtct.Foreground = Brushes.White;
                auxtct.Background = Brushes.Transparent;
                auxtct.Click += Project_Click;
                auxtct.HorizontalAlignment = HorizontalAlignment.Left;
                auxtct.Name = "DS" + datasources[i].ProjectId.ToString();
                auxtct.FontSize = 28;
                auxtct.FontWeight = FontWeights.Bold;
                auxtct.FontFamily = new FontFamily(new Uri(@"C:\Users\manol\Documents\Nueva carpeta\Dashboardmmiwpf\Dashboardmmiwpf\Resources\Ubuntu-B.ttf", UriKind.Absolute), "Ubuntu");
                auxnew2.Children.Add(auxtct);
                auxnew2.Children.Add(img1);
                auxnew.Children.Add(auxnew2);
                WrapPanel auxwrap = new WrapPanel();
                auxwrap.Height = 400;
                auxwrap.Orientation = Orientation.Vertical;

                for (int j = 0; j < datasources[i].lstDashboard.Count; j++)
                {

                    Tile newtile = new Tile();
                    newtile.Click += new RoutedEventHandler(Tile_Click);
                    newtile.HorizontalContentAlignment = HorizontalAlignment.Center;
                    newtile.Name = "DV" + datasources[i].lstDashboard[j].DashboardId.ToString();
                    StackPanel newstack = new StackPanel();
                    newstack.Orientation = Orientation.Vertical;
                    Image img = new Image();
                    BitmapImage bm = new BitmapImage();
                    bm.BeginInit();
                  
                    bm.UriSource = new Uri("/Recursos/dashboards-0"+ x.Next(1,6).ToString() +".png", UriKind.Relative);
                    bm.EndInit();
                    img.Stretch = Stretch.Fill;
                    img.Source = bm;
                    img.Height = 100;
                    img.Width = 270;
                    TextBlock newtxt = new TextBlock();
                    newtxt.FontSize = 24;
                    newtxt.FontFamily = new FontFamily(new Uri(@"C:\Users\manol\Documents\Nueva carpeta\Dashboardmmiwpf\Dashboardmmiwpf\Resources\Ubuntu-B.ttf", UriKind.Absolute),"Ubuntu");
                    newtxt.Margin = new Thickness(5);
                    newtxt.Text = datasources[i].lstDashboard[j].Nombre;
                    newstack.Children.Add(img);
                    newstack.Children.Add(newtxt);
                    newtile.Content = newstack;
                    auxwrap.Children.Add(newtile);
                }

                auxnew.Children.Add(auxwrap);

                stackContainer.Children.Add(auxnew);
            }
        }

        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            Tile clickedTile = (Tile)sender;
            int id = Convert.ToInt32((clickedTile.Name.ToString().Substring(2)));
            Conexion conexion = new Conexion();
            Dashboard dash = new Dashboard();
            Projects project = new Projects();
            project.DataTabletoListView(conexion.GetDashboardIndex(id));
            dash = project.lstDashboard.First();
            MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.sp.Speak("Vista Previa del Dashboard " + dash.Nombre);
            Dashboard_Preview MenuPrincipal = new Dashboard_Preview(id);
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).frame.NavigationService.Navigate(MenuPrincipal);
                    break;
                }
            }
        }

        private void Project_Click(object sender, RoutedEventArgs e)
        {
            Button clickedTile = (Button)sender;
            int id = Convert.ToInt32((clickedTile.Name.ToString().Substring(2)));
            MainWindow._recognizer.SpeechRecognized -= DashboardLista.speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            string text = clickedTile.Content.ToString();
            MainWindow.sp.Speak("Ingrese los nuevos datos para el grupo " + text);
            NuevoDashboard MenuPrincipal = new NuevoDashboard(id);
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
}
