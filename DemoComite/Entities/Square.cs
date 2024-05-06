using System.Drawing;
using System;

namespace Entities
{
    public class Square : Shape
    {
        public Square(double x, double y, double width, double height, Brush brocha)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Brocha = brocha;
            dx = dy = 0;
            Selected = false;
            Hover = false;
            
        }

        public override void Dibujar(Graphics c)
        {
            float x = Convert.ToSingle(X);
            float y = Convert.ToSingle(Y);
            float ancho = Convert.ToSingle(Width);
            float alto = Convert.ToSingle(Height);

            if (Selected || (Selected == true && Hover == true))
            {
                c.FillRectangle(colorSeleccion, x, y, ancho, alto);
            }
            else if(Hover && Selected == false)
            {
                c.FillRectangle(colorHover, x, y, ancho, alto);
            }
            else if(Hover == false && Selected == false)
            {
                c.FillRectangle(Brocha, x, y, ancho, alto);
            }
        }
    }
}
