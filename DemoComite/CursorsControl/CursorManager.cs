using System.Collections.Generic;
using System.Drawing;
using Microsoft.Kinect;

namespace CursorsControl
{
    public class CursorManager
    {
        public List<Cursor> cursores { get; }

        public CursorManager()
        {
            cursores = new List<Cursor>();
        }

        public void addCursor(Cursor _cursor)
        {
            cursores.Add(_cursor);
        }
        
        public void Dibujar(Graphics g)
        {
            foreach(Cursor c in cursores)
            {
                c.DibujarCursor(g);
            }
        }

        public void RefreshCursors(double RightX, double RightY, double LeftX, double LeftY, HandState rightState, HandState leftState)
        {
            foreach (Cursor c in cursores)
            {
                switch(c.tipoMano)
                {
                    case enumHandType.Right:
                        c.RefreshCursor(RightX, RightY, rightState);
                        break;
                    case enumHandType.Left:
                        c.RefreshCursor(LeftX, LeftY, leftState);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
