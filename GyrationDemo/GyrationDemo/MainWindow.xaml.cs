using GyrationDemo.Component;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace GyrationDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double HandSize = 30;

        private const double JointThickness = 3;

        private const double ClipBoundsThickness = 10;

        private const float InferredZPositionClamp = 0.1f;

        private readonly Brush handClosedBrush = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));

        private readonly Brush handOpenBrush = new SolidColorBrush(Color.FromArgb(128, 0, 255, 0));

        private readonly Brush handLassoBrush = new SolidColorBrush(Color.FromArgb(128, 0, 0, 255));

        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));
     
        private readonly Brush inferredJointBrush = Brushes.Yellow;
      
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);

        private DrawingGroup drawingGroup;

        private DrawingImage imageSource;

        private KinectSensor kinectSensor = null;

        private CoordinateMapper coordinateMapper = null;

        private BodyFrameReader bodyFrameReader = null;

        private Body[] bodies = null;

        private List<Tuple<JointType, JointType>> bones;

        private int displayWidth;

        private int displayHeight;

        private List<Pen> bodyColors;

        private string statusText = null;

        private SpacePoint beginPoint;

        private SpacePoint finalPoint;

        public MainWindow()
        {
           
            InitializeComponent();
            InitializeKinectComponents();
        }


        public void InitializeKinectComponents()
        {

            this.kinectSensor = KinectSensor.GetDefault();

         
            this.coordinateMapper = this.kinectSensor.CoordinateMapper;

            FrameDescription frameDescription = this.kinectSensor.DepthFrameSource.FrameDescription;

            this.displayWidth = frameDescription.Width;
            this.displayHeight = frameDescription.Height;

            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            this.bones = new List<Tuple<JointType, JointType>>();


            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            this.bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));


            this.bodyColors = new List<Pen>();

            this.bodyColors.Add(new Pen(Brushes.Red, 6));
            this.bodyColors.Add(new Pen(Brushes.Orange, 6));
            this.bodyColors.Add(new Pen(Brushes.Green, 6));
            this.bodyColors.Add(new Pen(Brushes.Blue, 6));
            this.bodyColors.Add(new Pen(Brushes.Indigo, 6));
            this.bodyColors.Add(new Pen(Brushes.Violet, 6));

            this.kinectSensor.Open();

            this.DataContext = this;

            this.beginPoint = new SpacePoint();
            this.finalPoint = new SpacePoint();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.FrameArrived += this.Reader_FrameArrived;
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (this.bodyFrameReader != null)
            {
            
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }

            if (this.kinectSensor != null)
            {
                this.kinectSensor.Close();
                this.kinectSensor = null;
            }
        }

        private void Reader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }

                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {

                    foreach (Body body in this.bodies)
                    {


                        if (body.IsTracked)
                        {
                           
                            IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

                            Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();

                            CameraSpacePoint jointLeftHand = joints[JointType.HandLeft].Position;
                            CameraSpacePoint jointRightHand = joints[JointType.HandRight].Position;

                            this.ShowDistanceWhenHandsAreClosed(body.HandLeftState, body.HandRightState, jointLeftHand, jointRightHand);


                        }
                    }

                
            }
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var horizontalCurrentValue = double.Parse(this.lbHorizontal.Content.ToString());
            var verticalCurrentValue = double.Parse(this.lbVertical.Content.ToString());

            if (e.Key == Key.Right)
            {
                horizontalCurrentValue -= 0.75;

            }else if(e.Key == Key.Left)
            {
                horizontalCurrentValue += 0.75;
            }

            if (e.Key == Key.Up)
            {
                verticalCurrentValue -= 0.75;

            }
            else if (e.Key == Key.Down)
            {
                verticalCurrentValue += 0.75;
            }

            this.lbHorizontal.Content = horizontalCurrentValue.ToString();
            this.lbVertical.Content = verticalCurrentValue.ToString();

        }

        private void ShowDistanceWhenHandsAreClosed(HandState handLeftState, HandState handRightState, CameraSpacePoint handLeftPosition, CameraSpacePoint handRightPosition)
        {

            if (handLeftState == HandState.Closed && handRightState == HandState.Closed)
            {
                var ZPositionDifference = GetDifferenceWithAbsoluteValue(handLeftPosition.Z, handRightPosition.Z);
                var YPositionDifference = GetDifferenceWithAbsoluteValue(handLeftPosition.Y, handRightPosition.Y);
                var XPositionDifference = GetDifferenceWithAbsoluteValue(handLeftPosition.X, handRightPosition.X);


                if (IsInInterval((float)0.20, (float)0.60, XPositionDifference) &&
                    IsInInterval((float)0.0, (float)0.05, YPositionDifference) &&
                    IsInInterval((float)0.0, (float)0.25, ZPositionDifference))
                {
                  

                    if (this.beginPoint.NoSettingYet())
                    {
                        this.beginPoint.SettingPoints(handRightPosition.X, handRightPosition.Y, handRightPosition.Z);
                    }
                    else
                    {
                        if (!beginPoint.IsSamePoint(handRightPosition.X, handRightPosition.Y))
                        {
                            var XDifference = this.GetDifferenceWithoutAbsoluteValue(beginPoint.X, handRightPosition.X);
                            var YDifference = this.GetDifferenceWithoutAbsoluteValue(beginPoint.Y, handRightPosition.Y);

                            var horizontalCurrentValue = float.Parse(this.lbHorizontal.Content.ToString());
                            var verticalCurrentValue = float.Parse(this.lbVertical.Content.ToString());

                            horizontalCurrentValue += ((XDifference *100)*((float)-1.0));
                            verticalCurrentValue += (YDifference *100);

                            this.lbHorizontal.Content = horizontalCurrentValue.ToString();
                            this.lbVertical.Content = verticalCurrentValue.ToString();

                            this.beginPoint.SettingPoints(handRightPosition.X, handRightPosition.Y, handRightPosition.Z);

                        }

                    }

                }
           

            }else{
                    this.beginPoint.ResetPoint();

                }



        }


        private void ShowCoordinateXYZOfPointInLabel(CameraSpacePoint position, Label label)
        {

            String coordinatePattern = "X:{0}, Y:{1}, Z:{2}";
            String coordinate = String.Format(coordinatePattern, Math.Round((decimal)position.X, 2),
                                                                 Math.Round((decimal)position.Y, 2),
                                                                 Math.Round((decimal)position.Z, 2));
            label.Content = coordinate;

        }

        private float GetDifferenceWithAbsoluteValue(float initialPosition, float finalPosition)
        {
            var difference = finalPosition - initialPosition;
            difference = (float)Math.Round((decimal)difference, 2);
            return Math.Abs(difference);

        }

        private float GetDifferenceWithoutAbsoluteValue(float initialPosition, float finalPosition)
        {
            var difference = finalPosition - initialPosition;
            difference = (float)Math.Round((decimal)difference, 2);
            return difference;

        }

        private bool IsInInterval(float minimumValue, float maximunValue, float value)
        {

            if (value <= maximunValue && value >= minimumValue) return true;

            return false;
        }       
    }
}
