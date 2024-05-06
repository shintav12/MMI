using KinectV2MouseControl;
using System;
using System.Reflection;
using System.Speech.Recognition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfKb.Controls;

namespace DemoWpf
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    /// 
    

    public partial class MainWindow : Window
    {
        KinectControl kinectCtrl = new KinectControl();
        static SpeechRecognitionEngine _recognizer;
        FloatingTouchScreenKeyboard VKeyboard = new FloatingTouchScreenKeyboard();
        public MainWindow()
        {
            InitializeComponent();
            _recognizer = new SpeechRecognitionEngine();    
            _recognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("cajadetexto", "cambiardecolor","abrir","cerrar");
            grammarBuilder.Append(commandChoices);
            VKeyboard.IsOpen = false;
            VKeyboard.Width = 900;
            VKeyboard.Height = 400;
            VKeyboard.Placement = System.Windows.Controls.Primitives.PlacementMode.Center;
            VKeyboard.AreAnimationsEnabled = true;
            VKeyboard.PlacementTarget = this;
            Choices valueChoices = new Choices();
            valueChoices.Add("uno", "dos","tres","cuatro","cinco");
            valueChoices.Add("rojo", "verde","azul");
            valueChoices.Add("teclado");
            valueChoices.Add("teclado");
            grammarBuilder.Append(valueChoices);

            _recognizer.LoadGrammar(new Grammar(grammarBuilder));
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }
        private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (true)
            {
                string command = e.Result.Words[0].Text.ToLower();
                string value = e.Result.Words[1].Text.ToLower();
                switch (command)
                {
                    case "cajadetexto":
                        switch (value)
                        {
                            case "uno":
                                textBox1.Focus();
                                break;

                            case "dos":
                                textBox2.Focus();
                                break;

                            case "tres":
                                textBox3.Focus();
                                break;

                            case "cuatro":
                                textBox4.Focus();
                                break;

                            case "cinco":
                                textBox5.Focus();
                                break;
                        }
                        break;
                    case "cambiardecolor":
                        switch (value)
                        {
                            case "rojo":
                                grid.Background = Brushes.DarkRed;
                                break;

                            case "verde":
                                grid.Background = Brushes.DarkSeaGreen;
                                break;

                            case "azul":
                                grid.Background = Brushes.AliceBlue;
                                break;
                        }
                        break;
                    case "abrir":
                        switch (value)
                        {
                            case "teclado":
                                VKeyboard.IsOpen = true;
                                btnTeclado.Content = "Cerrar Teclado";
                                break;
                        }
                        break;
                    case "cerrar":
                        switch (value)
                        {
                            case "teclado":
                                VKeyboard.IsOpen = false;
                                btnTeclado.Content = "Abrir Teclado";
                                break;
                        }
                        break;

                }
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = comboBox.Text;
            switch (selected)
            {
                case "Red":
                    grid.Background = Brushes.DarkRed;
                    break;

                case "Green":
                    grid.Background = Brushes.DarkSeaGreen;
                    break;

                case "Blue":
                    grid.Background = Brushes.AliceBlue;
                    break;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Brush result = Brushes.Transparent;

            Random rnd = new Random();

            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties();

            int random = rnd.Next(properties.Length);
            result = (Brush)properties[random].GetValue(null, null);
            Rectangulo.Fill = result;
        }

        private void btnTeclado_Click(object sender, RoutedEventArgs e)
        {
            if (VKeyboard.IsOpen)
            {
                VKeyboard.IsOpen = false;
                btnTeclado.Content = "Abrir Teclado";
            }
            else
            {
                VKeyboard.IsOpen = true;
                btnTeclado.Content = "Cerrar Teclado";
            }
        }
    }


}
