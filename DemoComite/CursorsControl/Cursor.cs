using System.Drawing;
using Microsoft.Kinect;
using System;

namespace CursorsControl
{
    public class Cursor
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double dx { get; set; }
        public double dy { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public HandState _handState { get; set; }
        public enumHandType tipoMano { get; set; }

        public Brush lassoColor = Brushes.Red;
        public Brush closedColor = Brushes.RosyBrown;
        public Brush openColor = Brushes.Black;
        public Brush unknownColor = Brushes.Green;

        public void DibujarCursor(Graphics c)
        {
            float x = Convert.ToSingle(X);
            float y = Convert.ToSingle(Y);
            float ancho = Convert.ToSingle(Width);
            float alto = Convert.ToSingle(Height);

            switch (_handState)
            {
                case HandState.Open:
                    c.FillEllipse(openColor, x, y, ancho, alto);
                    break;
                case HandState.Closed:
                    c.FillEllipse(closedColor, x, y, ancho, alto);
                    break;
                case HandState.Lasso:
                    c.FillEllipse(lassoColor, x, y, ancho, alto);
                    break;
                case HandState.Unknown:
                    c.FillEllipse(unknownColor, x, y, ancho, alto);
                    break;
                default:
                    break;
            }
        }

        public void RefreshCursor(double px,double py, HandState stateHand)
        {
            if (px != X)
                dx = X - px;
            if (py != Y)
                dy = Y - py;
            X = px;
            Y = py;
            _handState = stateHand;
        }
    }
}
