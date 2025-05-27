using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MiOcr;

public partial class ScreenCaptureWindow : Window
{
    public Rect SelectedRect { get; private set; }
    public BitmapSource? CapturedImage { get; private set; }

    private System.Windows.Point? _start;
    private Rectangle? _rectShape;

    public ScreenCaptureWindow()
    {
        InitializeComponent();
        MouseLeftButtonDown += OnMouseDown;
        MouseMove += OnMouseMove;
        MouseLeftButtonUp += OnMouseUp;
        Cursor = Cursors.Cross;
        PreviewKeyDown += ScreenCaptureWindow_PreviewKeyDown;
        Focusable = true;
        Loaded += (s, e) => Keyboard.Focus(this);
    }

    private void ScreenCaptureWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape)
        {
            CapturedImage = null;
            DialogResult = false;
            Close();
        }
    }

    private void PositionTextBlocks(double x, double y, double w, double h)
    {
        double margin = 8;
        double canvasWidth = CaptureCanvas.ActualWidth;
        double canvasHeight = CaptureCanvas.ActualHeight;

        // 先测量文本大小
        StartCoordText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        SizeText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        double startW = StartCoordText.DesiredSize.Width;
        double startH = StartCoordText.DesiredSize.Height;
        double sizeW = SizeText.DesiredSize.Width;
        double sizeH = SizeText.DesiredSize.Height;

        // 1. 左上优先
        double startX = x - startW - margin;
        double startY = y - startH - margin;
        if (startX >= 0 && startY >= 0)
        {
            Canvas.SetLeft(StartCoordText, startX);
            Canvas.SetTop(StartCoordText, startY);
            Canvas.SetLeft(SizeText, startX);
            Canvas.SetTop(SizeText, startY + startH + 4);
            return;
        }

        // 2. 右上
        startX = x + w + margin;
        startY = y - startH - margin;
        if (startX + startW <= canvasWidth && startY >= 0)
        {
            Canvas.SetLeft(StartCoordText, startX);
            Canvas.SetTop(StartCoordText, startY);
            Canvas.SetLeft(SizeText, startX);
            Canvas.SetTop(SizeText, startY + startH + 4);
            return;
        }

        // 3. 左下
        startX = x - startW - margin;
        startY = y + h + margin;
        if (startX >= 0 && startY + startH + sizeH + 4 <= canvasHeight)
        {
            Canvas.SetLeft(StartCoordText, startX);
            Canvas.SetTop(StartCoordText, startY);
            Canvas.SetLeft(SizeText, startX);
            Canvas.SetTop(SizeText, startY + startH + 4);
            return;
        }

        // 4. 右下
        startX = x + w + margin;
        startY = y + h + margin;
        if (startX + startW <= canvasWidth && startY + startH + sizeH + 4 <= canvasHeight)
        {
            Canvas.SetLeft(StartCoordText, startX);
            Canvas.SetTop(StartCoordText, startY);
            Canvas.SetLeft(SizeText, startX);
            Canvas.SetTop(SizeText, startY + startH + 4);
            return;
        }

        // 5. 屏幕内兜底
        Canvas.SetLeft(StartCoordText, Math.Max(margin, Math.Min(canvasWidth - startW - margin, x)));
        Canvas.SetTop(StartCoordText, Math.Max(margin, Math.Min(canvasHeight - startH - margin, y)));
        Canvas.SetLeft(SizeText, Math.Max(margin, Math.Min(canvasWidth - sizeW - margin, x)));
        Canvas.SetTop(SizeText, Math.Max(margin, Math.Min(canvasHeight - sizeH - margin, y + startH + 4)));
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _start = e.GetPosition(this);
        _rectShape = new Rectangle
        {
            Stroke = Brushes.Red,
            StrokeThickness = 2,
            Fill = new SolidColorBrush(Color.FromArgb(40, 0, 0, 255))
        };
        CaptureCanvas.Children.Add(_rectShape);
        Canvas.SetLeft(_rectShape, _start.Value.X);
        Canvas.SetTop(_rectShape, _start.Value.Y);

        StartCoordText.Text = $"起点: ({(int)_start.Value.X}, {(int)_start.Value.Y})";
        StartCoordText.Visibility = Visibility.Visible;
        CurrentCoordText.Text = $"当前: ({(int)_start.Value.X}, {(int)_start.Value.Y})";
        CurrentCoordText.Visibility = Visibility.Visible;
        SizeText.Text = $"大小: 0 x 0";
        SizeText.Visibility = Visibility.Visible;
        // 初始位置
        PositionTextBlocks(_start.Value.X, _start.Value.Y, 0, 0);
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (_start.HasValue && _rectShape != null)
        {
            var pos = e.GetPosition(this);
            double x = Math.Min(_start.Value.X, pos.X);
            double y = Math.Min(_start.Value.Y, pos.Y);
            double w = Math.Abs(_start.Value.X - pos.X);
            double h = Math.Abs(_start.Value.Y - pos.Y);
            Canvas.SetLeft(_rectShape, x);
            Canvas.SetTop(_rectShape, y);
            _rectShape.Width = w;
            _rectShape.Height = h;

            // 更新当前点坐标
            CurrentCoordText.Text = $"当前: ({(int)pos.X}, {(int)pos.Y})";
            Canvas.SetLeft(CurrentCoordText, pos.X + 2);
            Canvas.SetTop(CurrentCoordText, pos.Y + 2);

            // 更新区域大小
            SizeText.Text = $"大小: {(int)w} x {(int)h}";
            // 动态调整文本位置
            PositionTextBlocks(x, y, w, h);
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_start.HasValue && _rectShape != null)
        {
            var end = e.GetPosition(this);
            double x = Math.Min(_start.Value.X, end.X);
            double y = Math.Min(_start.Value.Y, end.Y);
            double w = Math.Abs(_start.Value.X - end.X);
            double h = Math.Abs(_start.Value.Y - end.Y);
            SelectedRect = new Rect(x, y, w, h);

            // 隐藏坐标
            StartCoordText.Visibility = Visibility.Collapsed;
            CurrentCoordText.Visibility = Visibility.Collapsed;

            // 隐藏区域大小
            SizeText.Visibility = Visibility.Collapsed;

            // 截图
            CapturedImage = CaptureScreenArea(SelectedRect);
            DialogResult = true;
            Close();
        }
    }

    private BitmapSource CaptureScreenArea(Rect rect)
    {
        double dpiScale = NativeMethods.GetDpiScale(this);

        int x = (int)(rect.X * dpiScale);
        int y = (int)(rect.Y * dpiScale);
        int w = (int)(rect.Width * dpiScale);
        int h = (int)(rect.Height * dpiScale);

        IntPtr hdcSrc = NativeMethods.GetDC(IntPtr.Zero);
        IntPtr hdcDest = NativeMethods.CreateCompatibleDC(hdcSrc);
        IntPtr hBitmap = NativeMethods.CreateCompatibleBitmap(hdcSrc, w, h);
        IntPtr hOld = NativeMethods.SelectObject(hdcDest, hBitmap);

        NativeMethods.BitBlt(hdcDest, 0, 0, w, h, hdcSrc, x, y, 0x00CC0020); // SRCCOPY

        NativeMethods.SelectObject(hdcDest, hOld);
        NativeMethods.DeleteDC(hdcDest);
        NativeMethods.ReleaseDC(IntPtr.Zero, hdcSrc);

        try
        {
            var source = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            source.Freeze();
            return source;
        }
        finally
        {
            NativeMethods.DeleteObject(hBitmap);
        }
    }

    //private BitmapSource CaptureScreenArea(Rect rect)
    //{
    //    int x = (int)rect.X;
    //    int y = (int)rect.Y;
    //    int w = (int)rect.Width;
    //    int h = (int)rect.Height;
    //    using var bmp = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
    //    using (var g = Graphics.FromImage(bmp))
    //    {
    //        g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(w, h), CopyPixelOperation.SourceCopy);
    //    }
    //    var hBitmap = bmp.GetHbitmap();
    //    try
    //    {
    //        return Imaging.CreateBitmapSourceFromHBitmap(
    //            hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
    //    }
    //    finally
    //    {
    //        NativeMethods.DeleteObject(hBitmap);
    //    }
    //}


}
