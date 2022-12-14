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

//     <source src = "rtmp://rtc.qiscus.com:2935/live360p/testing123&testing.mp4" type="rtmp/mp4">
//  </source>

//    rtmp/mp4
//11 :   opus: 'video/ogg',
//12 :   ogv: 'video/ogg',
//13 :   mp4: 'video/mp4',
//14 :   mov: 'video/mp4',
//15 :   m4v: 'video/mp4',
//16 :   mkv: 'video/x-matroska',
//17 :   m4a: 'audio/mp4',
//18 :   mp3: 'audio/mpeg',
//19 :   aac: 'audio/aac',
//20 :   caf: 'audio/x-caf',
//21 :   flac: 'audio/flac',
//22 :   oga: 'audio/ogg',
//23 :   wav: 'audio/wav',
//24 :   m3u8: 'application/x-mpegURL',
//25 :   mpd: 'application/dash+xml',
//26 :   jpg: 'image/jpeg',
//27 :   jpeg: 'image/jpeg',
//28 :   gif: 'image/gif',
//29 :   png: 'image/png',
//30 :   svg: 'image/svg+xml',
//31 :   webp: 'image/webp'
}
