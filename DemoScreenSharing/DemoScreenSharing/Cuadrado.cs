using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoScreenSharing
{
    public class Cuadrado
    {
        public List<Point> limites;
        int dx = 40;
        int dy = 35;
        int ancho = 65;
        int alto = 65;

        public int x { get; set; }
        public int y { get; set; }


        
        public Cuadrado(List<Point> bounds)
        {
            limites = bounds;
            x = 600;
            y = 1500;
        }

        //1 izq sup
        //2 der sup
        //3 izq inf
        //4 der inf
        public void mover()
        {
            if (0 > x + dx || 6800 < x + dx + ancho)
                dx = dx * -1;
            if (0 > y + dy || 2160 < y + dy + alto)
                dy = dy * -1;
            x = x + dx;
            y = y + dy;
        }

        public void pintar(Secundarios form)
        {
            Graphics G = form.CreateGraphics();
            if (detectar(form))
            {
                int pantalla = Convert.ToInt32(form.Tag);

                G.Clear(Color.White);
                if (pantalla != 1)
                    G.FillRectangle(Brushes.Red, x - form.Location.X, y - form.Location.Y, ancho, alto);
                else
                    G.FillRectangle(Brushes.Red, x - form.Location.X - 2880, y - form.Location.Y, ancho, alto);
            }
            else
                G.Clear(Color.White);
        }

        public bool detectar(Secundarios form)
        {
            int fx = form.Location.X;
            int pantalla = Convert.ToInt32(form.Tag);
            if (pantalla == 1)
                fx = form.Location.X + 2880;
            if (fx != limites[0].X)
                fx = fx - ancho;
            int fy = form.Location.Y;
            if (fy != limites[0].Y)
                fy = fy - alto;

            if (fx <= x && fx + form.Width >= x)
                if (fy <= y && fy + form.Height >= y)
                    return true;


            return false;
        }
    }
}
