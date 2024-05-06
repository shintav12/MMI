using Entities;
using System.Collections.Generic;

namespace ScreenControl
{
    public class LogicScreenManager
    {
        List<LogicScreen> LogicScreens { get; }

        public LogicScreenManager()
        {
            LogicScreens = new List<LogicScreen>();
        }

        public void addLogicScreen(int x, int y, int width,int height,enumScreensTypes existe,int index)
        {
            LogicScreens.Add(new LogicScreen(x, y, width, height, existe, index));
        }

        public void detectObjectInScreen(List<Shape> objetos)
        {
            foreach(LogicScreen c in LogicScreens)
            {
                foreach(Shape o in objetos)
                {
                    o.IndexScreen = c.Index;
                }
            }
        }
    }
}
