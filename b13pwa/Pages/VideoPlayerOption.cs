// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using System.Text.Json.Serialization;

namespace b13video.Pages
{
    public class VideoPlayerOption
    {
        [JsonPropertyName("width")]
        public int Width { get; set; } = 300;

        [JsonPropertyName("height")]
        public int Height { get; set; } = 200;

        [JsonPropertyName("controls")]
        public bool Controls { get; set; } = true;

        [JsonPropertyName("autoplay")]
        public bool Autoplay { get; set; } = true;

        [JsonPropertyName("preload")]
        public string Preload { get; set; } = "auto";

        /// <summary>
        /// 播放资源
        /// </summary>
        [JsonPropertyName("sources")]
        public List<VideoSources> Sources { get; set; } = new List<VideoSources>();

        /// <summary>
        /// 设置封面
        /// </summary>
        [JsonPropertyName("poster")]
        public string? Poster { get; set; } 

        //[JsonPropertyName("enableSourceset")]
        //public bool EnableSourceset { get; set; }

        //[JsonPropertyName("techOrder")]
        //public string? TechOrder { get; set; } = "['html5', 'flash']";


    }


    /// <summary>
    /// 播放资源
    /// </summary>
    public class VideoSources
    {
        public VideoSources() { }

        public VideoSources(string? type, string? src)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.Src = src ?? throw new ArgumentNullException(nameof(src));
        }

        /// <summary>
        /// 资源类型<para></para>video/mp4<para></para>application/x-mpegURL<para></para>video/youtube
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "application/x-mpegURL";

        /// <summary>
        /// 资源地址
        /// </summary>
        [JsonPropertyName("src")]
        public string Src { get; set; } = "application/x-mpegURL";
    }

}
