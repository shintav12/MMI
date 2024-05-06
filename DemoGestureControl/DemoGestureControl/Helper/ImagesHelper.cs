using DemoGestureControl.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
namespace DemoGestureControl.Helper
{
    public static class ImagesHelper
    {

        public static Image BuildImageFromImageComponent(ImageComponent imageComponent,
                            double ScreenActualHeight, double ScreenActualWidth,
                            double LeftProperty, double RightProperty)
        {
            var image = new Image();
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(imageComponent.Url, UriKind.Relative);
            bitmap.EndInit();

            image.Source = bitmap;
            image.Height = ScreenActualHeight;
            image.Width = ScreenActualWidth;
            image.SetValue(Canvas.LeftProperty, LeftProperty);
            image.SetValue(Canvas.RightProperty, RightProperty);
            image.SetValue(Panel.ZIndexProperty, 0);
            
            return image;

        }
    }
}
