using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Kinect.Input;
using Microsoft.Kinect.Toolkit.Input;
using Microsoft.Kinect.Wpf.Controls;

namespace Dashboardmmiwpf
{
    public class DragDropElementController : IKinectManipulatableController
    {
        private ManipulatableModel _inputModel;
        private KinectRegion _kinectRegion;
        private DragDropElement _dragDropElement;
        private bool _disposedValue;

        public DragDropElementController(IInputModel inputModel, KinectRegion kinectRegion)
        {
            _inputModel = inputModel as ManipulatableModel;
            _kinectRegion = kinectRegion;
            _dragDropElement = _inputModel.Element as DragDropElement;

            _inputModel.ManipulationStarted += OnManipulationStarted;
            _inputModel.ManipulationUpdated += OnManipulationUpdated;
            _inputModel.ManipulationCompleted += OnManipulationCompleted;
        }

        private void OnManipulationCompleted(object sender,
            KinectManipulationCompletedEventArgs kinectManipulationCompletedEventArgs)
        {
            var parent = _dragDropElement.Parent as Canvas;
        }

        private void OnManipulationUpdated(object sender, KinectManipulationUpdatedEventArgs e)
        {
            var parent = _dragDropElement.Parent as Canvas;
            
            if (parent != null)
            {
                var d = e.Delta.Translation;
                var y = Canvas.GetTop(_dragDropElement);
                var x = Canvas.GetLeft(_dragDropElement);

                if (double.IsNaN(y)) y = 0;
                if (double.IsNaN(x)) x = 0;

                // Delta value is between 0.0 and 1.0 so they need to be scaled within the kinect region.
                var yD = d.Y * _kinectRegion.ActualHeight;
                var xD = d.X * _kinectRegion.ActualWidth;

                if(yD + y > 0 && yD+ y <= parent.ActualHeight - 200)
                    Canvas.SetTop(_dragDropElement, y + yD);
                if (xD + x > 0 && xD + x <= parent.Width - 200)
                    Canvas.SetLeft(_dragDropElement, x + xD);
            }
        }

        private void OnManipulationStarted(object sender, KinectManipulationStartedEventArgs e)
        {

        }

        ManipulatableModel IKinectManipulatableController.ManipulatableInputModel
        {
            get { return _inputModel; }
        }

        FrameworkElement IKinectController.Element
        {
            get { return _inputModel.Element as FrameworkElement; }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                _kinectRegion = null;
                _inputModel = null;
                _dragDropElement = null;

                _inputModel.ManipulationStarted -= OnManipulationStarted;
                _inputModel.ManipulationUpdated -= OnManipulationUpdated;
                _inputModel.ManipulationCompleted -= OnManipulationCompleted;

                _disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }
    }
}