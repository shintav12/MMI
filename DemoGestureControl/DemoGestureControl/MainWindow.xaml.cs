using DemoGestureControl.Component;
using DemoGestureControl.Helper;
using DemoGestureControl.Mock;
using LightBuzz.Vitruvius;
using Microsoft.Kinect;
using Microsoft.Kinect.Wpf.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DemoGestureControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor _sensor;
        private double screenActualWidth = 0.0;
        private double screenActualHeight = 0.0;
        private int currentImageCategory;
        private int extremeLeft;
        private int extremeRight;
        private double deltaAxisX;
        private List<Image> images;
        private SlideTranslation slideTranslation;
        private MultiSourceFrameReader _reader;
        private GestureController _gestureController;
        private List<ImageComponent> imageComponents;
        private ServiceMock serviceMock;
        private DispatcherTimer timer;
        private int remainingTimer;

        public MainWindow()
        {
            InitializeComponent();
            this.serviceMock = new ServiceMock();

            _sensor = KinectSensor.GetDefault();
            InitializeGestureKinectComponents(_sensor);
            remainingTimer = 2;
        }



        private void InitializeGestureKinectComponents(KinectSensor sensor)
        {
            if (sensor != null)
            {
                sensor.Open();

                KinectRegion.SetKinectRegion(this, kinectRegion);

                _reader = sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

                _gestureController = new GestureController();
                _gestureController.GestureRecognized += GestureController_GestureRecognized;

            }

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.screenActualHeight = this.ActualHeight;
            this.screenActualWidth = this.ActualWidth;
            this.deltaAxisX = this.screenActualWidth / 50.00;
            this.images = new List<Image>();
            this.currentImageCategory = 0;
            this.FillImages();
            this.extremeLeft = 0;
            this.extremeRight = images.Count() - 1;
            this.slideTranslation = new SlideTranslation(this.screenActualWidth, this.extremeLeft, this.extremeRight, this.images, this.deltaAxisX);
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);

            timer.Tick += (s, a) =>
            {
                if (remainingTimer < 0) {
                    remainingTimer = 2;
                    timer.Stop();
                    this.canvasOverlay.Visibility = Visibility.Hidden;
                    this.lbMessage.Visibility = Visibility.Hidden;
                }

                remainingTimer--;


            };


        }
        private void FillImages()
        {
            imageComponents = this.serviceMock.GetListCategoryComponents();

            for (int i = 0; i < imageComponents.Count(); i++)
            {

                var image = ImagesHelper.BuildImageFromImageComponent(imageComponents[i], this.screenActualHeight, this.screenActualWidth,
                                                                      this.screenActualWidth * i, this.screenActualWidth * (i + 1 ));

                this.images.Add(image);

                this.imageCanvas.Children.Add(image);

            }
        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {

                }
            }

            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    Body body = frame.Bodies().Closest();

                    if (body != null)
                    {
                        _gestureController.Update(body);
                    }
                }
            }
        }

        void GestureController_GestureRecognized(object sender, GestureEventArgs e)
        {
            String gesture = e.GestureType.ToString();

                if (gesture.Equals("SwipeLeft"))
                {

                    this.slideTranslation.MoveImageByRightHandGesture();

                }
                else
                {
                    if (gesture.Equals("SwipeRight"))
                    {
                        this.slideTranslation.MoveImageByLeftHandGesture();

                    }

                }
            
        }

        private void ClickImage(object sender, RoutedEventArgs e)
        {
            int currentImage = slideTranslation.CurrentImage;
            
            string message = "Has elegido la imagen de {0}";

            String.Format(message, imageComponents[currentImage].Name);

            this.canvasOverlay.Visibility = Visibility.Visible;
            this.lbMessage.Content = message;
            this.lbMessage.Visibility = Visibility.Visible;

            timer.Start();
        }

        


    }
}
