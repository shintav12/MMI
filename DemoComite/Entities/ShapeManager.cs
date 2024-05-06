using CursorsControl;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Entities
{
    public class ShapeManager
    {
        double anchoOrigi = 0;
        double altoOrigi = 0;
        bool seleccionado = true;

        public List<Shape> shapes { get; }

        public ShapeManager()
        {
            shapes = new List<Shape>();
        }

        public void addShape(double x,double y, double width,double height,Brush brocha,enumShapes type)
        {
            switch(type)
            {
                case enumShapes.Circle:
                    shapes.Add(new Circle(x, y, width, height, brocha));
                    break;
                case enumShapes.Square:
                    shapes.Add(new Square(x, y, width, height, brocha));
                    break;
                default:
                    break;
            }
        }

        public void drawShapes(Graphics g)
        {
            foreach(Shape c in shapes)
            {
                c.Dibujar(g);
            }
        }  
        
        public void deleteShape()
        {
            foreach(Shape s in shapes)
            {
                if(s.Selected)
                {
                    shapes.Remove(s);
                }
            }
        }
        public void changeSize(List<Cursor> cursores)
        {
            foreach (Shape s in shapes)
            {
                if (cursores[0]._handState == Microsoft.Kinect.HandState.Closed && cursores[1]._handState == Microsoft.Kinect.HandState.Closed)
                {
                    if(s.Selected)
                    {
                        s.Width = Math.Abs(Math.Abs(cursores[0].X) - Math.Abs(cursores[1].X));
                        s.Height = Math.Abs(Math.Abs(cursores[0].Y) - Math.Abs(cursores[1].Y));
                    }
                }
            }
        }

        public void detectHovering(List<Cursor> cursores)
        {
            foreach (Cursor c in cursores)
            {
                foreach (Shape s in shapes)
                {
                    if (s.isContained(c.X, c.Y))
                    {
                        s.Hover = true;
                    }
                    else
                    {
                        s.Hover = false;
                    }

                    if(s.Hover && seleccionado && c._handState == Microsoft.Kinect.HandState.Closed && c.tipoMano == enumHandType.Right)
                    {
                        seleccionado = false;
                        s.Selected = !s.Selected;
                    }
                    else if(c._handState != Microsoft.Kinect.HandState.Closed && c.tipoMano == enumHandType.Right)
                    {
                        seleccionado = true;
                    }

                    if(c._handState == Microsoft.Kinect.HandState.Closed && s.Hover)
                    {
                        s.X = c.X - s.Width/2;
                        s.Y = c.Y - s.Height/2;
                    }
                }
            }
        }
    }
}
