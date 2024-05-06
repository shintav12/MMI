using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PruebaExtensionPantalla
{
    public partial class Form1 : Form
    {
        int x;
        float posx1;
        float posy1;
        float posx2;
        float posy2;
        int y;
        int ancho;
        bool press;
        int limx1 = -1;
        int limx2 = 1;
        float dx1;
        float dy1;
        float dx2;
        float dy2;
        Cursor CursorDerecha;
        ManejadorPantallas pantallas;
        bool manoDer = false;
        bool manoIzq = false;
        Brush brocha;
        KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        IList<Body> _bodies;
        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CursorDerecha = new Cursor();
            pantallas = new ManejadorPantallas();
            pantallas.AgregarPantallas(-0.85f, 1.4f, 0f, 0.55f, 0.3f, 0f, true);
            //pantallas.AgregarPantallas(4.07f, 1.4f, 0f, 4.07f, 0.3f, 0f, true);
            x = y = 20;
            ancho = 40;
            dx1 = 0;
            dy1 = 0;
            dx2 = 0;
            dy2 = 0;
            posx1 = posy1 = posx2 = posy2 = 0;
            press = false;
            timer1.Enabled = true;
            brocha = Brushes.Blue;
            _sensor = KinectSensor.GetDefault();

            if (_sensor != null)
            {
                _sensor.Open();

                _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Depth | FrameSourceTypes.Infrared | FrameSourceTypes.Body);
                _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Graphics g = this.CreateGraphics();
            dx1 = (posx1 + 0.6f) * this.Width / 1.2f - 15f;
            dy1 = this.Height - ((posy1 + 0.33f) * this.Height / 0.66f) - 15f;
            dx2 = (posx2 + 0.6f) * this.Width / 1.2f - 15f;
            dy2 = this.Height - ((posy2 + 0.33f) * this.Height / 0.66f) - 15f;
            int pantallaindex = pantallas.ValidarInterseccion(CursorDerecha);


            g.Clear(Color.White);
            CursorDerecha.Dibujar(g, this.Width, this.Height, 0.6f, 0.33f, Brushes.Red);
            if (pantallaindex != -1)
                g.FillRectangle(Brushes.Blue, this.Width / 2 - 50, this.Height / 2 - 50, 100, 100);
            else
                g.FillRectangle(Brushes.Red, this.Width / 2 - 50, this.Height / 2 - 50, 100, 100);

            //g.FillRectangle(brocha, 200, 200, 300, 300);
            //g.FillEllipse(Brushes.Brown, dx1, dy1 , 30, 30);
            //g.FillEllipse(Brushes.Blue, dx2, dy2, 30, 30);

        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            press = true;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            press = false;
        }

        private void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();
            // Body
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
                                string rightHandState = "-";
                                string leftHandState = "-";
                                string estado = "--";


                                Joint manoDerecha = body.Joints[JointType.HandRight];
                                Joint codoDerehco = body.Joints[JointType.ElbowRight];
                                Joint hombroDerecho = body.Joints[JointType.ShoulderRight];
                                Joint manoIzquierda = body.Joints[JointType.HandLeft];
                                CursorDerecha.setearValores_Mano_Codo(manoDerecha.Position.X,
                                                                      manoDerecha.Position.Y,
                                                                      manoDerecha.Position.Z,
                                                                      hombroDerecho.Position.X,
                                                                      hombroDerecho.Position.Y,
                                                                      hombroDerecho.Position.Z);
                                posx1 = manoDerecha.Position.X;
                                posy1 = manoDerecha.Position.Y;
                                posx2 = manoIzquierda.Position.X;
                                posy2 = manoIzquierda.Position.Y;
                                label1.Text = manoDerecha.Position.Y.ToString();

                                if (manoDerecha.Position.X > 0.6 || manoDerecha.Position.X < -0.6)
                                {
                                    estado = "Fuera";
                                    brocha = Brushes.Green;
                                }
                                else
                                {
                                    estado = "Dentro";
                                    brocha = Brushes.Black;
                                }

                                label2.Text = estado;





                                switch (body.HandRightState)
                                {
                                    case HandState.Open:
                                        rightHandState = "Open";
                                        manoDer = false;
                                        break;
                                    case HandState.Closed:
                                        rightHandState = "Closed";
                                        manoDer = true;
                                        break;
                                    case HandState.Lasso:
                                        rightHandState = "Lasso";
                                        break;
                                    case HandState.Unknown:
                                        rightHandState = "Unknown...";
                                        break;
                                    case HandState.NotTracked:
                                        rightHandState = "Not tracked";
                                        break;
                                    default:
                                        break;
                                }

                                switch (body.HandLeftState)
                                {
                                    case HandState.Open:
                                        leftHandState = "Open";
                                        manoIzq = false;
                                        break;
                                    case HandState.Closed:
                                        manoIzq = true;
                                        leftHandState = "Closed";
                                        break;
                                    case HandState.Lasso:
                                        leftHandState = "Lasso";
                                        break;
                                    case HandState.Unknown:
                                        leftHandState = "Unknown...";
                                        break;
                                    case HandState.NotTracked:
                                        leftHandState = "Not tracked";
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_reader != null)
            {
                _reader.Dispose();
            }

            if (_sensor != null)
            {
                _sensor.Close();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
