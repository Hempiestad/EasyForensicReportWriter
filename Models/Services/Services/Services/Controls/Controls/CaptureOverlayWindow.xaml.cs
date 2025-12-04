using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace EasyForensicReportWriter.Controls
{
    public partial class CaptureOverlayWindow : Window
    {
        private Point? startPoint;
        private Rectangle captureRect = new(0, 0, 0, 0);
        public Bitmap? CapturedBitmap { get; private set; }

        public CaptureOverlayWindow()
        {
            InitializeComponent();
            Opacity = 0.3;
            KeyDown += (s, e) => { if (e.Key == Key.Escape) DialogResult = false; Close(); };
        }

        private void StartCapture(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(CaptureCanvas);
            captureRect = new Rectangle();
            CaptureCanvas.CaptureMouse();
        }

        private void DuringCapture(object sender, MouseEventArgs e)
        {
            if (startPoint.HasValue)
            {
                var currentPoint = e.GetPosition(CaptureCanvas);
                captureRect = new Rectangle(
                    (int)Math.Min(startPoint.Value.X, currentPoint.X),
                    (int)Math.Min(startPoint.Value.Y, currentPoint.Y),
                    (int)Math.Abs(currentPoint.X - startPoint.Value.X),
                    (int)Math.Abs(currentPoint.Y - startPoint.Value.Y));
                CaptureCanvas.InvalidateVisual();
            }
        }

        private void EndCapture(object sender, MouseButtonEventArgs e)
        {
            CaptureCanvas.ReleaseMouseCapture();
            if (captureRect.Width > 5 && captureRect.Height > 5)
            {
                CapturedBitmap = new Bitmap(captureRect.Width, captureRect.Height);
                using var g = Graphics.FromImage(CapturedBitmap);
                g.CopyFromScreen(captureRect.Location, Point.Empty, captureRect.Size);
            }
            DialogResult = true;
            Close();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (captureRect.Width > 0)
            {
                var rect = new Rect(captureRect.X, captureRect.Y, captureRect.Width, captureRect.Height);
                drawingContext.DrawRectangle(Brushes.Transparent, new Pen(Brushes.Red, 2), rect);
            }
        }
    }
}