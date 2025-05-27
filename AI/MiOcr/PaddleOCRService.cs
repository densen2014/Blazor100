// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR;
using Sdcb.OpenVINO.PaddleOCR.Models;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace JovenApi;

//<PackageReference Include="OpenCvSharp4.runtime.win" Version="4.11.0.20250507" />
//<PackageReference Include="Sdcb.OpenVINO.PaddleOCR" Version="0.6.8" />
//<PackageReference Include="Sdcb.OpenVINO.PaddleOCR.Models.Online" Version="0.6.2" />
//<PackageReference Include="Sdcb.OpenVINO.runtime.win-x64" Version="2025.0.0" />

public class PaddleOCRService
{

    public static bool IsUrl(string filename)
    {
        return Uri.TryCreate(filename, UriKind.Absolute, out var uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }

    public async Task<(List<string> strings, PaddleOcrResult result)> StartOCR(string filename, Action<string>? onStatusChanged = null, int timeoutMs = 1500)
    {
        Mat src;

        if (string.IsNullOrEmpty(filename))
        {
            throw new ArgumentNullException(nameof(filename));
        }

        if (IsUrl(filename))
        {
            src = Cv2.ImDecode(await new HttpClient().GetByteArrayAsync(filename), ImreadModes.Color);
        }
        else
        {
            src = Cv2.ImRead(filename);
        }

        return await StartOCR(src, onStatusChanged, timeoutMs);
    }

    public async Task<(List<string> strings, PaddleOcrResult result)> StartOCR(byte[] imageData, Action<string>? onStatusChanged = null, int timeoutMs = 1500)
    {
        Mat src;

        ArgumentNullException.ThrowIfNull(imageData);

        src = Cv2.ImDecode(imageData, ImreadModes.Color);
        return await StartOCR(src, onStatusChanged, timeoutMs);
    }

    public async Task<(List<string> strings, PaddleOcrResult result)> StartOCR(Mat src, Action<string>? onStatusChanged = null, int timeoutMs = 1500)
    {
        PaddleOcrResult result;
        var resultText = new List<string>();
        var modelInfo = OnlineFullModels.ChineseV4;

        // 启动超时提示任务
        using var cts = new CancellationTokenSource();
        var timeoutTask = Task.Delay(timeoutMs, cts.Token)
            .ContinueWith(t =>
            {
                if (!t.IsCanceled)
                {
                    onStatusChanged?.Invoke("正在初始化OCR模型，请稍候...");
                }
            });
        // 下载模型（如果已存在会跳过下载）
        FullOcrModel model = await modelInfo.DownloadAsync();
        cts.Cancel(); // 下载完成，取消超时提示

        using (PaddleOcrAll all = new(model)
        {
            AllowRotateDetection = true,
            Enable180Classification = true,
        })
        {
            result = all.Run(src);
            foreach (PaddleOcrResultRegion region in result.Regions)
            {
                resultText.Add(region.Text);
            }
        }
        src.Dispose();
        return (resultText, result);
    }

}
