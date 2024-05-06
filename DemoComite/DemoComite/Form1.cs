using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Speech.Recognition;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using CursorsControl;
using LightBuzz.Vitruvius;
using Entities;

namespace DemoComite
{
    public partial class Form1 : Form
    {
        IList<Body> _bodies;
        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        static SpeechRecognitionEngine _recognizer = null;
        CursorManager cursores;
        ShapeManager shapes;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _sensor = KinectSensor.GetDefault();
            cursores = new CursorManager();
            shapes = new ShapeManager();
            CursorsControl.Cursor derecha = new CursorsControl.Cursor();
            derecha.tipoMano = enumHandType.Right;
            derecha.Width = derecha.Height = 15;
            CursorsControl.Cursor izquierda = new CursorsControl.Cursor();
            izquierda.Height = izquierda.Width = 15;
            izquierda.tipoMano = enumHandType.Left;
            cursores.addCursor(izquierda);
            cursores.addCursor(derecha);
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker2.RunWorkerAsync();
            shapes.addShape(250, 500, 60, 60, Brushes.OrangeRed, enumShapes.Square);



            DoubleBuffered = true;

            if (_sensor != null)
            {
                _sensor.Open();

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }
        }

        public void SpeechRecognitionWithDictationGrammar()
        {
            _recognizer = new SpeechRecognitionEngine();
            _recognizer.LoadGrammar(new Grammar(new GrammarBuilder("exit")));
            _recognizer.LoadGrammar(new DictationGrammar());
            _recognizer.SpeechRecognized += speechRecognitionWithDictationGrammar_SpeechRecognized;
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void speechRecognitionWithDictationGrammar_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Speaker sp = new Speaker();
            String r = e.Result.Text.ToUpper();
            switch (r)
            {
                case "CUADRADO":
                    shapes.addShape(cursores.cursores[1].X, cursores.cursores[1].Y, 60, 60, Brushes.OrangeRed, enumShapes.Square);
                    sp.Speak("CUADRADO CREADO");
                    break;
                case "CÍRCULO":
                    shapes.addShape(cursores.cursores[1].X, cursores.cursores[1].Y, 60, 60, Brushes.DarkMagenta, enumShapes.Circle);
                    sp.Speak("CÍRCULO CREADO");
                    break;
                case "ELIMINAR":
                    shapes.deleteShape();
                    sp.Speak("FIGURAS ELIMINADAS");
                    break;
                default:
                    sp.Speak("No entendí, ¿podrías repetirlo?");
                    break;

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

                                var pointColor = positionizq.ToPoint(Visualization.Color);
                                var pointColorder = positionder.ToPoint(Visualization.Color);

                                CameraSpacePoint cameraPoint = izquierda.Position;
                                CameraSpacePoint cameraPoint2 = derecha.Position;

                                shapes.changeSize(cursores.cursores);


                                ColorSpacePoint colorPoint = _sensor.CoordinateMapper.MapCameraPointToColorSpace(cameraPoint);
                                ColorSpacePoint colorPoint2 = _sensor.CoordinateMapper.MapCameraPointToColorSpace(cameraPoint2);

                                /* cursores.RefreshCursors(cameraPoint2.X*cameraPoint2.Z*,
                                                         cameraPoint2.Y*cameraPoint2.Z * 384+384,
                                                         cameraPoint.X*cameraPoint.Z*1000,
                                                         cameraPoint.Y*cameraPoint.Z - 1000,
                                                         body.HandRightState, body.HandLeftState);*/
                                //double xi = (izquierda.Position.X*izquierda.Position.Z / 1.5) * 1920;
                                //double xd = (derecha.Position.X * derecha.Position.Z / 1.5) * 1920;
                                //double yi = (izquierda.Position.Y * izquierda.Position.Z / 1.5) * -1020;
                                //double yd = (derecha.Position.Y * derecha.Position.Z / 1.5) * -1020;

                                shapes.detectHovering(cursores.cursores);

                                double xi = (izquierda.Position.X  / 1.5) * 1920*1.1;
                                double xd = (derecha.Position.X  / 1.5) * 1920*1.1;
                                double yi = ((izquierda.Position.Y - 0.4) / 1) * -1020*2;
                                double yd = ((derecha.Position.Y - 0.4) / 1) * -1020*2;
                                
                                cursores.RefreshCursors(xd,yd,xi, yi,body.HandRightState, body.HandLeftState);
                            }
                        }
                    }
                }
            }

        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            SpeechRecognitionWithDictationGrammar();
        }

        private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();

            g.Clear(Color.White);
            shapes.drawShapes(g);
            cursores.Dibujar(g);
        }
    }
}
