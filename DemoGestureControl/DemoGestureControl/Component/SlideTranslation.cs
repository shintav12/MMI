using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Threading;

namespace DemoGestureControl.Component
{
    public class SlideTranslation
    {
        private double ScreenActualWidth { get; set; }
        private List<Image> Images { get; set; }
        private double DeltaX { get; set; }
        private int ExtremeLeft { get; set; }
        private int ExtremeRight { get; set; }
        public int CurrentImage { get; set; }
        private bool CarouselMode { get; set; }

        public SlideTranslation(double screenActualWidth, int extremeLeft, int extremeRight, List<Image> images, double deltaX, bool carouselMode = true)
        {
            this.ScreenActualWidth = screenActualWidth;
            this.ExtremeLeft = extremeLeft;
            this.ExtremeRight = extremeRight;
            this.Images = images;
            this.CarouselMode = carouselMode;
            this.SetLastImageLikePreviousImageOfFirstImage();
            this.DeltaX = deltaX;
        }

        public void ResetExtremeValues()
        {
            this.ExtremeLeft = 0;
            this.ExtremeRight = this.Images.Count() - 1;

        }

        public void MoveImageByLeftHandGesture()
        {

            DispatcherTimer timer = this.GetConfigureDispatcherTimerInMiliSeconds();

            timer.Tick += (s, a) =>
            {
                for (int i = 0; i < Images.Count(); i++)
                {
                    double leftCurrentImage = Double.Parse(Images[i].GetValue(Canvas.LeftProperty).ToString());
                    double rightCurrentImage = Double.Parse(Images[i].GetValue(Canvas.RightProperty).ToString());

                    double leftPropertyCurrentImage = (Double.Parse(Images[CurrentImage].GetValue(Canvas.LeftProperty).ToString()));

                    if (leftPropertyCurrentImage < ScreenActualWidth &&
                            leftPropertyCurrentImage >= 0.0)
                    {
                        if (i == CurrentImage)
                        {
                            leftCurrentImage = (DeltaX + leftCurrentImage) > ScreenActualWidth ? ScreenActualWidth : leftCurrentImage + DeltaX;
                            rightCurrentImage = (DeltaX + leftCurrentImage) > ScreenActualWidth ? 2 * ScreenActualWidth : rightCurrentImage + DeltaX;
                        }
                        else
                        {
                            rightCurrentImage = rightCurrentImage + DeltaX;
                            leftCurrentImage = leftCurrentImage + DeltaX;
                        }

                        Images[i].SetValue(Canvas.RightProperty, rightCurrentImage);
                        Images[i].SetValue(Canvas.LeftProperty, leftCurrentImage);
                    }
                    else
                    {
                        if (CurrentImage <= 0)
                        {
                            CurrentImage = Images.Count() - 1;
                        }
                        else
                        {
                            CurrentImage--;
                        }

                        SetLastImageIndexLeftGesture();
                        timer.Stop();
                        break;
                    }

                }


            };

            timer.Start();

        }
        public void MoveImageByRightHandGesture()
        {
            DispatcherTimer timer = this.GetConfigureDispatcherTimerInMiliSeconds();

            timer.Tick += (s, a) =>
            {
                for (int i = 0; i < Images.Count(); i++)
                {
                    double leftCurrentImage = Double.Parse(Images[i].GetValue(Canvas.LeftProperty).ToString());
                    double rightCurrentImage = Double.Parse(Images[i].GetValue(Canvas.RightProperty).ToString());

                    double rightPropertyCurrentImage = (Double.Parse(Images[CurrentImage].GetValue(Canvas.RightProperty).ToString()));

                    if (rightPropertyCurrentImage <= ScreenActualWidth &&
                        rightPropertyCurrentImage > 0.0)
                    {
                        if (i == CurrentImage)
                        {
                            rightCurrentImage = DeltaX > rightCurrentImage ? 0.0 : rightCurrentImage - DeltaX;
                            leftCurrentImage = DeltaX > rightCurrentImage ? -ScreenActualWidth : leftCurrentImage - DeltaX;
                        }
                        else
                        {
                            rightCurrentImage = rightCurrentImage - DeltaX;
                            leftCurrentImage = leftCurrentImage - DeltaX;
                        }

                        Images[i].SetValue(Canvas.RightProperty, rightCurrentImage);
                        Images[i].SetValue(Canvas.LeftProperty, leftCurrentImage);
                    }
                    else
                    {
                        if (CurrentImage < (Images.Count() - 1))
                        {
                            CurrentImage++;
                        }
                        else
                        {
                            CurrentImage = 0;
                        }


                        SetLastImageIndexRightGesture();
                        timer.Stop();
                        break;

                    }

                }
            };
            timer.Start();

        }

        public void SetLastImageIndexRightGesture()
        {

            Image extremeLeftImage = Images[ExtremeLeft];

            Image extremeRightImage = Images[ExtremeRight];

            ExtremeRight = ExtremeLeft;
            ExtremeLeft = ExtremeLeft + 1 >= (Images.Count()) ? 0 : ExtremeLeft + 1;

            Images[CurrentImage].SetValue(Canvas.LeftProperty, 0.00);
            Images[CurrentImage].SetValue(Canvas.RightProperty, ScreenActualWidth);

            int previousExtremeRight = ExtremeRight - 1 < 0 ? Images.Count() - 1 : ExtremeRight - 1;

            Double newLeftProperty = Double.Parse(Images[previousExtremeRight].GetValue(Canvas.RightProperty).ToString());

            Double newRightProperty = Double.Parse(Images[previousExtremeRight].GetValue(Canvas.RightProperty).ToString()) + ScreenActualWidth;

            extremeLeftImage.SetValue(Canvas.LeftProperty, newLeftProperty);
            extremeLeftImage.SetValue(Canvas.RightProperty, newRightProperty);

        }

        private void SetLastImageIndexLeftGesture()
        {

            Image extremeLeftImage = Images[ExtremeLeft];

            Image extremeRightImage = Images[ExtremeRight];

            Double newLeftProperty = Double.Parse(extremeLeftImage.GetValue(Canvas.LeftProperty).ToString()) - ScreenActualWidth;

            Double newRightProperty = Double.Parse(extremeLeftImage.GetValue(Canvas.LeftProperty).ToString());

            extremeRightImage.SetValue(Canvas.LeftProperty, newLeftProperty);
            extremeRightImage.SetValue(Canvas.RightProperty, newRightProperty);

            ExtremeLeft = ExtremeRight;
            ExtremeRight = ExtremeRight - 1 < 0 ? Images.Count() - 1 : ExtremeRight - 1;

            Images[CurrentImage].SetValue(Canvas.LeftProperty, 0.00);
            Images[CurrentImage].SetValue(Canvas.RightProperty, ScreenActualWidth);


        }

        private void SetLastImageLikePreviousImageOfFirstImage()
        {
            double leftProperty = Double.Parse(Images[CurrentImage].GetValue(Canvas.LeftProperty).ToString());

            if (PropertyIsInScreenRange(leftProperty) && CurrentImage == 0)
            {
                Image rightExtremelyImage = Images[Images.Count() - 1];

                rightExtremelyImage.SetValue(Canvas.LeftProperty, ScreenActualWidth * (-1.00));
                rightExtremelyImage.SetValue(Canvas.RightProperty, 0.0);

                this.ExtremeLeft = Images.Count() - 1;
            }

            SetLastImageIndexLeftGesture();
        }

        private Boolean PropertyIsInScreenRange(Double value)
        {
            if (value <= ScreenActualWidth &&
               value > 0.0)
            {
                return true;
            }

            return false;
        }

        private DispatcherTimer GetConfigureDispatcherTimerInMiliSeconds()
        {
            DispatcherTimer timer = new DispatcherTimer();

            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);

            return timer;
        }


    }
}
