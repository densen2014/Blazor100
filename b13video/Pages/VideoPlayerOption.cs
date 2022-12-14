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
        public int Width { get; set; } = 600;

        [JsonPropertyName("height")]
        public int Height { get; set; } = 300;

        [JsonPropertyName("controls")]
        public bool Controls { get; set; } = true;

        [JsonPropertyName("autoplay")]
        public bool Autoplay { get; set; } = true;

        [JsonPropertyName("preload")]
        public string Preload { get; set; } = "auto";

        [JsonPropertyName("poster")]
        public string Poster { get; set; } = "//vjs.zencdn.net/v/oceans.png";

        [JsonPropertyName("sources")]
        public List<VideoSources> Sources { get; set; } = new List<VideoSources>();

    }

    public class VideoSources
    {
        public VideoSources() { }

        public VideoSources(string type, string src)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.src = src ?? throw new ArgumentNullException(nameof(src));
        }

        /// <summary>
        /// video/mp4,application/x-mpegURL
        /// </summary>
        public string type { get; set; } = "application/x-mpegURL";

        /// <summary>
        /// application/x-mpegURL,video/mp4
        /// </summary>
        public string src { get; set; } = "application/x-mpegURL";
    }

}
