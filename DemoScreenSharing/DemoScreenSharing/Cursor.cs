using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace DemoScreenSharing
{
    public class Cursor
    {
        public int x { get; set; }
        public int y { get; set; }
        public int velocityX { get; set; }
        public int velocityY { get; set; }
        public HandState estado { get; set; }
        int sec = 0;
        public Cursor()
        {
            x = 0;
            y = 0;
            estado = HandState.NotTracked;
        }

        public void ActualizarCoord(ColorSpacePoint cursor,float Z)
        {
            if (float.IsInfinity(cursor.X)) cursor.X = 0;
            if (float.IsInfinity(cursor.Y)) cursor.Y = 0;

            int xAux = Convert.ToInt32(cursor.X);
            int yAux = Convert.ToInt32(cursor.Y);
            sec++;
            if (sec == 2)
            {
                velocityX = xAux - x;
                velocityY = yAux - y;
                sec = 0;
            }

            x = xAux;
            y = yAux;
        }
        public void ActualizarEstado(HandState handState)
        {
            estado = handState;
        }

        public void pintar(Secundarios form)
        {
                int pantalla = Convert.ToInt32(form.Tag);
                Graphics G = form.CreateGraphics();
                 G.FillEllipse(Brushes.Firebrick, x , y , 15, 15);
        }
    }
}
