using JovenApi;
using Microsoft.Win32;
using Sdcb.OpenVINO.PaddleOCR;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace MiOcr;
public partial class MainWindow : Window
{
    private BitmapImage? _currentImage;
    private byte[]? _currentImageBytes;
    private List<(Geometry geometry, string text)> _ocrRegions = new();
    private System.Windows.Point? _selectStart;
    private System.Windows.Point? _selectEnd;
    private Rectangle _selectionRect = new Rectangle();
    private List<string> _selectedTexts = new();
    private bool DrawTextOnOcr => DrawTextCheckBox?.IsChecked == true;
    private PaddleOcrResult? _lastOcrResult;

    // DPI缩放因子
    double? dpiScale;

    public MainWindow()
    {
        InitializeComponent();
        this.PreviewDragOver += (s, e) => e.Handled = true; // 允许拖放
        this.PreviewKeyDown += Window_KeyDown;
        MainImage.SizeChanged += MainImage_SizeChanged;
    }

    /// <summary>
    /// 在窗口或控件尺寸变化时，自动同步 OverlayCanvas 的大小为 MainImage 的实际显示区域（即 ActualWidth 和 ActualHeight）
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainImage_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        OverlayCanvas.Width = MainImage.ActualWidth;
        OverlayCanvas.Height = MainImage.ActualHeight;
    }

    // 拖放图片
    private async void Window_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0 && IsImageFile(files[0]))
            {
                await LoadAndOcrImage(files[0]);
            }
        }
    }

    // Border区域拖放
    private  void ImageBorder_Drop(object sender, DragEventArgs e)
    {
         Window_Drop(sender, e);
    }

    // 粘贴图片
    private async void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.V && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
        {
            if (Clipboard.ContainsImage())
            {
                var img = Clipboard.GetImage();
                if (img != null)
                {
                    var bmp = BitmapFromClipboard(img);
                    using var ms = new MemoryStream();
                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bmp));
                    encoder.Save(ms);
                    await LoadAndOcrImage(ms.ToArray());
                }
            }
        }
    }

    // 点击选择图片
    private async void SelectImageButton_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif"
        };
        if (dlg.ShowDialog() == true)
        {
            await LoadAndOcrImage(dlg.FileName);
        }
    }

    //// 点击图片区域也可选择图片
    ///<Border ... MouseLeftButtonUp="ImageBorder_MouseLeftButtonUp" ...>
    //private  void ImageBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    //{
    //     SelectImageButton_Click(sender, e);
    //}

    // 加载图片并调用OCR
    private async Task LoadAndOcrImage(string filePath)
    {
        ClearOverlayAndSelection();

        _currentImageBytes = await File.ReadAllBytesAsync(filePath);
        _currentImage = new BitmapImage(new Uri(filePath));
        MainImage.Source = _currentImage;
        await RunOcrAndDraw(_currentImageBytes);
    }

    private void ClearOverlayAndSelection()
    {
        // 清空选框状态和覆盖层
        _selectStart = null;
        _selectEnd = null;
        OverlayCanvas.Children.Clear();
    }

    // 加载图片并调用OCR（字节流）
    private async Task LoadAndOcrImage(byte[] imageBytes)
    {
        ClearOverlayAndSelection();

        _currentImageBytes = imageBytes;
        using var ms = new MemoryStream(imageBytes);
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.StreamSource = ms;
        bmp.EndInit();
        bmp.Freeze();
        _currentImage = bmp;
        MainImage.Source = _currentImage;
        await RunOcrAndDraw(imageBytes);
    }


    // OCR并绘制
    private async Task RunOcrAndDraw(byte[] imageBytes)
    {
        // 先显示图片
        ProcessingText.Visibility = Visibility.Visible;
        await Task.Delay(100); // 确保UI刷新

        var ocr = new PaddleOCRService();
        var results = await ocr.StartOCR(imageBytes, msg =>
        {
            // 友好提示，确保在UI线程
            Dispatcher.Invoke(() =>
            {
                ProcessingText.Text = msg;
                ProcessingText.Visibility = Visibility.Visible;
            });
        });

        ProcessingText.Visibility = Visibility.Collapsed;

        if (results.strings == null || results.strings.Count == 0)
        {
            OcrTextBox.Text = "未识别到文本";
            return;
        }

        OcrTextBox.Text = string.Join(Environment.NewLine, results.result.Regions.Select(r => r.Text));
        _lastOcrResult = results.result; // 缓存OCR结果
        var drawed = DrawOcrResultsOnImage(_currentImage, results.result);
        MainImage.Source = drawed;
    }

    // 判断文件是否为图片
    private bool IsImageFile(string file)
    {
        var ext = System.IO.Path.GetExtension(file).ToLower();
        return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif";
    }

    // 从ClipboardBitmapSource转BitmapSource
    private BitmapSource BitmapFromClipboard(BitmapSource src)
    {
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(src));
        using var ms = new MemoryStream();
        encoder.Save(ms);
        ms.Position = 0;
        var bmp = new BitmapImage();
        bmp.BeginInit();
        bmp.CacheOption = BitmapCacheOption.OnLoad;
        bmp.StreamSource = ms;
        bmp.EndInit();
        bmp.Freeze();
        return bmp;
    }

    // 绘制OCR结果到图片
    private BitmapSource DrawOcrResultsOnImage(BitmapImage? image, PaddleOcrResult result)
    {
        if (image == null) return null!;
        int width = image.PixelWidth;
        int height = image.PixelHeight;

        _ocrRegions.Clear(); // 清空旧的区域

        var visual = new DrawingVisual();
        using (var dc = visual.RenderOpen())
        {
            dc.DrawImage(image, new Rect(0, 0, width, height));
            var pen = new Pen(Brushes.Red, 2);
            var typeface = new Typeface("Arial");
            foreach (var region in result.Regions)
            {
                var vertices = region.Rect.Points();
                var points = vertices.Select(p => new System.Windows.Point(p.X, p.Y)).ToArray();

                // 绘制多边形
                var figure = new PathFigure(points[0], new[] { new PolyLineSegment(points.Skip(1), true) }, true);
                var geometry = new PathGeometry(new[] { figure });
                dc.DrawGeometry(null, pen, geometry);

                // 记录区域和文本
                _ocrRegions.Add((geometry, region.Text));

                if (DrawTextOnOcr)
                {
                    // 绘制文字（更大字体、带背景、下移）
                    var formattedText = new FormattedText(
                        region.Text,
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        typeface,
                        32, // 字体更大
                        Brushes.Black,
                        1.25);

                    // 计算文本位置（下移10像素）
                    var textPos = new System.Windows.Point(region.Rect.Center.X, region.Rect.Center.Y + 10);

                    // 绘制半透明白色背景
                    var bgRect = new Rect(
                        textPos.X,
                        textPos.Y,
                        formattedText.Width + 12,
                        formattedText.Height + 6);
                    dc.DrawRectangle(new SolidColorBrush(Color.FromArgb(180, 255, 255, 255)), null, bgRect);

                    // 绘制文字（居中显示）
                    dc.DrawText(formattedText, new System.Windows.Point(textPos.X + 6, textPos.Y + 3));
                }
            }
        }
        var bmp = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        bmp.Render(visual);
        bmp.Freeze();
        return bmp;
    }
    private void MainImage_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
        var pos = e.GetPosition(MainImage);

        // 需要将控件坐标映射到图片像素坐标
        if (_currentImage == null || _ocrRegions.Count == 0) return;

        // 计算缩放比例和偏移
        var imgRect = new Rect(0, 0, _currentImage.PixelWidth, _currentImage.PixelHeight);
        var ctrlRect = new Rect(0, 0, MainImage.ActualWidth, MainImage.ActualHeight);
        var scale = Math.Min(ctrlRect.Width / imgRect.Width, ctrlRect.Height / imgRect.Height);
        var offsetX = (ctrlRect.Width - imgRect.Width * scale) / 2;
        var offsetY = (ctrlRect.Height - imgRect.Height * scale) / 2;
        var imgX = (pos.X - offsetX) / scale;
        var imgY = (pos.Y - offsetY) / scale;
        var imgPoint = new System.Windows.Point(imgX, imgY);

        // 命中测试
        foreach (var (geometry, text) in _ocrRegions)
        {
            if (geometry.FillContains(imgPoint))
            {
                // 弹出右键菜单
                var menu = new ContextMenu();
                var item = new MenuItem { Header = "复制文本" };
                item.Click += (s, args) => Clipboard.SetText(text);
                menu.Items.Add(item);
                menu.IsOpen = true;
                break;
            }
        }
    }

    private void MainImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_currentImage == null) return;

        _selectStart = ClampToImageRect(e.GetPosition(MainImage));
        _selectEnd = null;
        _selectedTexts.Clear();
        OverlayCanvas.Children.Clear();
        MainImage.CaptureMouse();
    }

    private void MainImage_MouseMove(object sender, MouseEventArgs e)
    {
        if (_selectStart.HasValue && e.LeftButton == MouseButtonState.Pressed)
        {
            _selectEnd = ClampToImageRect(e.GetPosition(MainImage));
            DrawSelectionRectangle();
        }
    }


    private void MainImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_selectStart.HasValue && _selectEnd.HasValue)
        {
            var rect = GetImageRectFromControlRect(new Rect(_selectStart.Value, _selectEnd.Value));
            _selectedTexts = _ocrRegions
                .Where(r => r.geometry.Bounds.IntersectsWith(rect))
                .Select(r => r.text)
                .ToList();

            if (_selectedTexts.Count > 0)
            {
                var menu = new ContextMenu();
                var item = new MenuItem { Header = $"复制所选文本（{_selectedTexts.Count}）" };
                item.Click += (s, args) => Clipboard.SetText(string.Join(Environment.NewLine, _selectedTexts));
                menu.Items.Add(item);
                menu.IsOpen = true;
            }
        }
        _selectStart = null;
        _selectEnd = null;
        OverlayCanvas.Children.Clear();
        MainImage.ReleaseMouseCapture();
    }

    private void DrawSelectionRectangle()
    {
        OverlayCanvas.Children.Clear();
        if (_selectStart.HasValue && _selectEnd.HasValue)
        {
            var imgRect = GetImageDisplayRect();
            // 限定选框在图片显示区域内
            var start = ClampToImageRect(_selectStart.Value);
            var end = ClampToImageRect(_selectEnd.Value);
            var rect = new Rect(start, end);

            // 只在图片显示区域内绘制
            var intersect = Rect.Intersect(imgRect, rect);
            if (intersect.IsEmpty) return;

            var r = new System.Windows.Shapes.Rectangle
            {
                Stroke = Brushes.Blue,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(40, 0, 0, 255)),
                Width = intersect.Width,
                Height = intersect.Height
            };
            Canvas.SetLeft(r, intersect.Left);
            Canvas.SetTop(r, intersect.Top);
            OverlayCanvas.Children.Add(r);

            // 获取选框对应图片坐标的区域
            var imgSelRect = GetImageRectFromControlRect(intersect);
            var selectedTexts = _ocrRegions
                .Where(region => region.geometry.Bounds.IntersectsWith(imgSelRect))
                .Select(region => region.text)
                .ToList();

            if (selectedTexts.Count > 0)
            {
                var text = string.Join(Environment.NewLine, selectedTexts);
                var tb = new TextBlock
                {
                    Text = text,
                    Foreground = Brushes.Black,
                    Background = new SolidColorBrush(Color.FromArgb(180, 255, 255, 255)),
                    FontSize = 16,
                    TextWrapping = TextWrapping.Wrap,
                    Width = Math.Max(1, r.Width - 8),
                    Padding = new Thickness(4)
                };
                Canvas.SetLeft(tb, intersect.Left + 4);
                Canvas.SetTop(tb, intersect.Top + 4);
                OverlayCanvas.Children.Add(tb);
            }
        }
    }


    private Rect GetImageRectFromControlRect(Rect ctrlRect)
    {
        if (_currentImage == null) return Rect.Empty;
        var imgRect = new Rect(0, 0, _currentImage.PixelWidth, _currentImage.PixelHeight);
        var ctrlActual = new Rect(0, 0, MainImage.ActualWidth, MainImage.ActualHeight);
        var scale = Math.Min(ctrlActual.Width / imgRect.Width, ctrlActual.Height / imgRect.Height);
        var offsetX = (ctrlActual.Width - imgRect.Width * scale) / 2;
        var offsetY = (ctrlActual.Height - imgRect.Height * scale) / 2;

        // 映射到图片坐标
        double x1 = (ctrlRect.Left - offsetX) / scale;
        double y1 = (ctrlRect.Top - offsetY) / scale;
        double x2 = (ctrlRect.Right - offsetX) / scale;
        double y2 = (ctrlRect.Bottom - offsetY) / scale;
        return new Rect(new System.Windows.Point(x1, y1), new System.Windows.Point(x2, y2));
    }

    /// <summary>
    /// 计算图片在控件中的实际显示区域
    /// </summary>
    /// <returns></returns>
    private Rect GetImageDisplayRect()
    {
        if (_currentImage == null) return Rect.Empty;
        double imgWidth = _currentImage.PixelWidth;
        double imgHeight = _currentImage.PixelHeight;
        double ctrlWidth = MainImage.ActualWidth;
        double ctrlHeight = MainImage.ActualHeight;

        double scale = Math.Min(ctrlWidth / imgWidth, ctrlHeight / imgHeight);
        double displayWidth = imgWidth * scale;
        double displayHeight = imgHeight * scale;
        double offsetX = (ctrlWidth - displayWidth) / 2;
        double offsetY = (ctrlHeight - displayHeight) / 2;

        return new Rect(offsetX, offsetY, displayWidth, displayHeight);
    }

    /// <summary>
    ///  鼠标事件中，选框坐标限定在图片显示区域
    /// </summary>
    /// <param name="pt"></param>
    /// <returns></returns>
    private Point ClampToImageRect(Point pt)
    {
        // 获取图片在控件中的实际显示区域（含缩放和中心偏移）
        var rect = GetImageDisplayRect();
        double x = Math.Max(rect.Left, Math.Min(rect.Right, pt.X));
        double y = Math.Max(rect.Top, Math.Min(rect.Bottom, pt.Y));
        return new Point(x, y);
    }

    private void DrawTextCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        // 只有在有图片和OCR结果时才刷新
        if (_currentImage != null && _ocrRegions.Count > 0 && _currentImageBytes != null)
        {
            // 重新绘制图片（不需要重新OCR，只需重绘）
            var ocr = new PaddleOCRService();
            // 这里直接用上次的OCR结果即可，不必重新识别
            // 假设你有上次的OCR结果对象，可以缓存下来
            // 这里假设你有一个 _lastOcrResult 字段
            if (_lastOcrResult != null)
            {
                MainImage.Source = DrawOcrResultsOnImage(_currentImage, _lastOcrResult);
            }
        }
    }
    private async void CaptureScreenButton_Click(object sender, RoutedEventArgs e)
{
    var win = new ScreenCaptureWindow();
    if (win.ShowDialog() == true && win.CapturedImage != null)
    {
        // 将截图转为字节流
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(win.CapturedImage));
        using var ms = new MemoryStream();
        encoder.Save(ms);
        ms.Position = 0;
        await LoadAndOcrImage(ms.ToArray());
    }
}

    public async Task TaskAsync()
    {
        var OCRService = new PaddleOCRService();
        var httpClient = new HttpClient();
        var saveDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
        Directory.CreateDirectory(saveDir);

        for (int i = 3; i <= 60; i++)
        {
            var fileNameD = i.ToString("D4") + ".jpg";
            var url = $"https://miyako.es/images/carta/ALE/{fileNameD}";
            var savePath = Path.Combine(saveDir, fileNameD);

            try
            {
                var bytes = await httpClient.GetByteArrayAsync(url);
                await File.WriteAllBytesAsync(savePath, bytes);
                Console.WriteLine($"下载成功: {fileNameD}");
                var res = await OCRService.StartOCR(bytes);
                if (res.strings != null && res.strings.Count > 0)
                {
                    File.AppendAllText(Path.Combine(saveDir, "menu.txt"), string.Join("\n", res) + "\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"下载失败: {fileNameD}，原因: {ex.Message}");
            }
        }
    }
}

 
