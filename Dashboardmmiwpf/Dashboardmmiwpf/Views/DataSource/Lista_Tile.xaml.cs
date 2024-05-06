using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Dashboardmmiwpf
{
    /// <summary>
    /// Interaction logic for Lista_Tile.xaml
    /// </summary>
    public partial class Lista_Tile : Page
    {
        int dsView;
        public Lista_Tile()
        {
            InitializeComponent();
            Conexion conexion = new Conexion();
            DataSource ds = new DataSource();
            List<DataSource> datasources = ds.DataTabletoList(conexion.GetDataSource());

            for (int i = 0; i < datasources.Count; i++)
            {
                datasources[i].DataTabletoListView(conexion.GetDataViews(datasources[i].id));
            }

            for (int i = 0; i < datasources.Count; i++)
            {
                StackPanel auxnew = new StackPanel();
                auxnew.Name = "DS" + datasources[i].id.ToString();
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
                auxtct.Name = "DS" + datasources[i].id.ToString();
                auxtct.FontSize = 28;
                auxtct.FontWeight = FontWeights.Bold;
                auxtct.FontFamily = new FontFamily(new Uri(@"C:\Users\manol\Documents\Nueva carpeta\Dashboardmmiwpf\Dashboardmmiwpf\Resources\Ubuntu-B.ttf", UriKind.Absolute), "Ubuntu");
                auxtct.HorizontalAlignment = HorizontalAlignment.Left;
                auxtct.Content = datasources[i].titulo;
                auxtct.Foreground = Brushes.White;
                auxtct.Background = Brushes.Transparent;
                auxtct.Click += textBlock_Click;
                
                auxnew2.Children.Add(auxtct);
                auxnew2.Children.Add(img1);
                auxnew.Children.Add(auxnew2);
                WrapPanel auxwrap = new WrapPanel();
                auxwrap.Height = 400;
                auxwrap.Orientation = Orientation.Vertical;

                for (int j = 0; j < datasources[i].dataview.Count; j++)
                {
                    
                    Tile newtile = new Tile();
                    newtile.Name = "DV" + datasources[i].dataview[j].id.ToString();
                    newtile.Click += new RoutedEventHandler(Tile_Click);
                    StackPanel newstack = new StackPanel();
                    newstack.Orientation = Orientation.Horizontal;
                    Image img = new Image();
                    BitmapImage bm = new BitmapImage();
                    bm.BeginInit();
                    bm.UriSource = new Uri("/Recursos/dbNegro.png", UriKind.Relative);
                    bm.EndInit();
                    img.Stretch = Stretch.Fill;
                    img.Source = bm;
                    img.Height = 42;
                    img.Width = 48;
                    TextBlock newtxt = new TextBlock();
                    newtxt.FontSize = 24;
                    newtxt.Margin = new Thickness(5);   
                    newtxt.Text = datasources[i].dataview[j].DsNombre;
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
            MainWindow._recognizer.SpeechRecognized -= DataSourceLista.speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.sp.Speak("Ingrese los nuevos datos para la vista de datos");
            DetalleDataview MenuPrincipal = new DetalleDataview(id);
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).frame.NavigationService.Navigate(MenuPrincipal);
                    break;
                }
            }
        }

        private void textBlock_Click(object sender, RoutedEventArgs e)
        {
            MainWindow._recognizer.SpeechRecognized -= DataSourceLista.speechRecognizer_SpeechRecognized;
            MainWindow._recognizer.RecognizeAsyncStop();
            MainWindow.sp.Speak("Ingrese los nuevos datos de la conexión");
            Button clickedTile = (Button)sender;
            int id = Convert.ToInt32((clickedTile.Name.ToString().Substring(2)));
            EditarEliminar MenuPrincipal = new EditarEliminar(id);  
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
    