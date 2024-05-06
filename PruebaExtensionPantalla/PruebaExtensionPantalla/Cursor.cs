using System;
using System.Drawing;

namespace PruebaExtensionPantalla
{
    public class Cursor
    {
        public Punto Mano { get; set; }
        public Punto Codo { get; set; } 

        public Cursor()
        {
            Mano = new Punto();
            Codo = new Punto();
            Mano.x = 0;
            Mano.y = 0;
            Mano.z = 0;
            Codo.x = 0;
            Codo.y = 0;
            Codo.z = 0;
        }

        public void setearValores_Mano_Codo(float _xMano, float _yMano, float _zMano, float _xCodo, float _yCodo, float _zCodo)
        {
            Mano.x = _xMano;
            Mano.y = _yMano;
            Mano.z = _zMano;
            Codo.x = _xCodo;
            Codo.y = _yCodo;
            Codo.z = _zCodo;
        }

        public Punto HallarVector()
        {
            Punto vectorPuntero = new Punto();
            vectorPuntero.x = Mano.x - Codo.x;
            vectorPuntero.y = Mano.y - Codo.y;
            vectorPuntero.z = Mano.z - Codo.z;

            return vectorPuntero;
        }

        public void Dibujar(Graphics g,float anchoPantalla,float altoPantalla, float limx, float limy, Brush color)
        {
            float xEscalado = (Mano.x + limx) * anchoPantalla / (limx*2) - 15f;
            float yEscalado = altoPantalla - ((Mano.y + limy) * altoPantalla / (limy*2)) - 15f;
            float xEscaladoCodo = (Codo.x + limx) * anchoPantalla / (limx * 2) - 15f;
            float yEscaladoCodo = altoPantalla - ((Codo.y + limy) * altoPantalla / (limy * 2)) - 15f;
            g.FillEllipse(color, xEscalado, yEscalado, 30, 30);
        }


    }
}
