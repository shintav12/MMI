using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GyrationDemo.Component
{
    public class SpacePoint
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public SpacePoint()
        {
            this.X = float.NaN;
            this.Y = float.NaN;
            this.Z = float.NaN;
        }

        public void SettingPoints(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public bool NoSettingYet()
        {
            if (float.IsNaN(this.X) || float.IsNaN(this.Y) || float.IsNaN(this.Z)) return true;

            return false;
        }

        public void ResetPoint()
        {
            this.X = float.NaN;
            this.Y = float.NaN;
            this.Z = float.NaN;

        }

        public bool IsSamePoint(float x, float y)
        {
            if (this.X == x && this.Y == y)
                return true;

            return false;
        }
    }
}
