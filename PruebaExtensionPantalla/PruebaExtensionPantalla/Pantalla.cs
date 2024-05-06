using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaExtensionPantalla
{
    public class Pantalla
    {
        public float limx { get; set; }
        public float limy { get; set; }
        public Punto T1 { get; set; } //Izquierda Superior
        public Punto T2 { get; set; } //Derecha Superior
        public float[] Ecuacion { get; set; }
        public Punto T3 { get; set; } //Derecha Inferior
        public Punto T4 { get; set; } //Izquierda Inferior  
        public Punto T5 { get; set; } //Punto Medio
        public float altura { get; set; }
        public float ancho { get; set; }
        public bool valida { get; set; }
        public Pantalla(float x1,float y1,float z1,float x3,float y3, float z3,bool _valida)
        {
            T1 = new Punto();
            T2 = new Punto();
            T3 = new Punto();
            T4 = new Punto();
            T5 = new Punto();
            valida = _valida;
            altura = y1 - y3;
            ancho = x1 - x3;
            T1.x = x1;
            T1.y = y1;
            T1.z = z1;
            T3.x = x3;
            T3.y = y3;
            T3.z = z3;
            T2.x = T3.x;
            T2.y = T1.y;
            T2.z = T3.z;
            T4.x = T1.x;
            T4.y = T3.y;
            T4.z = T1.z;
            T5.x = (x1 + x3) / 2;
            T5.y = (y1 + y3) / 2;
            T5.z = (z1 + z3) / 2;
            Punto vectorAB = new Punto();
            vectorAB.x =  T5.x - T1.x;
            vectorAB.y =  T5.y - T2.y;
            vectorAB.z = T5.z - T1.z;
            Punto vectorBC = new Punto();
            vectorBC.x =  T5.x  - T3.x;
            vectorBC.y =  T5.y - T3.y;
            vectorBC.z = T5.z - T3.z;

            float[,] Matriz = { {T5.x,T5.y,T5.z }, 
                                   {vectorAB.x,vectorAB.y,vectorAB.z }, 
                                   {vectorBC.x,vectorBC.y,vectorBC.z } };

            float CoeficienteX = Matriz[1, 1] * Matriz[2, 2] - Matriz[2, 1] * Matriz[1, 2];
            float CoeficienteY = Matriz[1, 2] * Matriz[2, 0] - Matriz[2, 2] * Matriz[1, 0];
            float CoeficienteZ = Matriz[1, 0] * Matriz[2, 1] - Matriz[2, 0] * Matriz[1, 1];
            float ComplementoD = CoeficienteX * Matriz[0, 0] + CoeficienteY * Matriz[0, 1] + CoeficienteZ * Matriz[0, 2];
            float[] ecuacionAux = { CoeficienteX,CoeficienteY*(-1f),CoeficienteZ,ComplementoD };
            Ecuacion = ecuacionAux;
        }

        public bool ValidarInterseccionCoordenadas(Punto _interseccion)
        {
            if (T1.x <= _interseccion.x && T3.x >= _interseccion.x)
            {
                if (T1.y >= _interseccion.y && T3.y <= _interseccion.y)
                {
                    //if (T1.z <= _interseccion.z && T3.z >= _interseccion.z)
                    //{
                        return true;
                    //}
                    //return false;
                }
                return false;
            }
            return false;
        }
        public Punto intersecta(Punto vector, Punto punto)
        {
            Punto interseccion = new Punto();
            try
            {
                Punto vectorXecuacion = new Punto();
                vectorXecuacion.x = vector.x * Ecuacion[0];
                vectorXecuacion.y = vector.y * Ecuacion[1];
                vectorXecuacion.z = vector.z * Ecuacion[2];
                Punto puntoXecuacion = new Punto();
                puntoXecuacion.x = punto.x * Ecuacion[0];
                puntoXecuacion.y = punto.y * Ecuacion[1];
                puntoXecuacion.z = punto.z * Ecuacion[2];
                float coeficienteT = vectorXecuacion.x + vectorXecuacion.y + vectorXecuacion.z;
                float igualdad = puntoXecuacion.x + puntoXecuacion.y + puntoXecuacion.z + Ecuacion[3] ;
                float T = (igualdad*-1) / coeficienteT;
                
                interseccion.x = punto.x + vector.x * T;
                interseccion.y = punto.y + vector.y * T;
                interseccion.z = punto.z + vector.z * T;
                
            }
            catch (Exception)
            {
                interseccion.x = -100000;
                interseccion.y = -100000;
                interseccion.z = -100000;
            }

            return interseccion;
        }

        
    }

}
