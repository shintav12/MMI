namespace ScreenControl
{
    public class LogicScreen
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public enumScreensTypes  Type {get;set;}
        public int Index { get; set; }

        public LogicScreen(int x,int y, int width, int height, enumScreensTypes type, int index)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Type = type;
            Index = index;
        }

        public bool isContained(int x,int width,int y, int height)
        {
            if (X < x + width && x < X + Width)
                if (Y < y + height && y < Y + Height)
                    return true;
            return false;
        }
    }
}
