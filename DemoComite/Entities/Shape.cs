using System.Drawing;

namespace Entities
{
    public abstract class Shape
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double dx { get; set; }
        public double dy { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool Selected { get; set; }
        public bool Hover { get; set; }
        public int IndexScreen { get; set; }
        public Brush Brocha { get; set; }

        public Brush colorHover = Brushes.LightBlue;
        public Brush colorSeleccion = Brushes.Green;

        public abstract void Dibujar(Graphics c);

        public void Mover()
        {
            X = X + dx;
            Y = Y + dy;
        }

        public bool isContained(double x, double y)
        {
            if (X < x && x < X + Width)
                if (Y < y && y < Y + Height)
                    return true;
            return false;
        }

    }
}
