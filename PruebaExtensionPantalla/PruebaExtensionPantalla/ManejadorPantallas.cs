using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaExtensionPantalla
{
    public class ManejadorPantallas
    {
        List<Pantalla> pantallas;
        Cursor _cursor;
        public ManejadorPantallas()
        {
            pantallas = new List<Pantalla>();
            //_cursor = new Cursor();
        }
        public void AgregarPantallas(float x1, float y1, float z1, float x3, float y3, float z3, bool _valida)
        {
            pantallas.Add(new Pantalla(x1, y1, z1, x3, y3, z3,_valida));
        }

        public int ValidarInterseccion(Cursor _cursor)
        {
            Punto interseccion;
            foreach (Pantalla pantalla in pantallas)
            {
                interseccion = pantalla.intersecta(_cursor.HallarVector(), _cursor.Mano);
                if (interseccion.x == -1000000)
                {
                    return -1;
                }
                else
                {
                    if (pantalla.ValidarInterseccionCoordenadas(interseccion))
                        return 1;// pantallas.IndexOf(pantalla);
                }
            }
            return -1;
        }
    }
}
