// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************


using System.ComponentModel;

namespace Blazor100.Components
{

    /// <summary>
    /// 定位数据类
    /// </summary>
    public class Geolocationitem
    {
        /// <summary>
        /// 状态
        /// </summary>
        /// <returns></returns>
        [DisplayName("状态")]
        public string? Status { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        /// <returns></returns>
        [DisplayName("纬度")]
        public decimal Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        /// <returns></returns>
        [DisplayName("经度")]
        public decimal Longitude { get; set; }

        /// <summary>
        /// 准确度(米)<para></para>
        /// 将以m指定维度和经度值与实际位置的差距，置信度为95%.
        /// </summary>
        [DisplayName("准确度(米)")]
        public decimal Accuracy { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [DisplayName("时间戳")]
        public long Timestamp { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        [DisplayName("时间")]
        public DateTime LastUpdateTime { get => UnixTimeStampToDateTime(Timestamp); }

        /// <summary>
        /// 移动距离
        /// </summary>
        [DisplayName("移动距离")]
        public decimal CurrentDistance { get; set; } = 0.0M;

        /// <summary>
        /// 总移动距离
        /// </summary>
        [DisplayName("总移动距离")]
        public decimal TotalDistance { get; set; } = 0.0M;

        /// <summary>
        /// 最后一次获取到的纬度
        /// </summary>
        [DisplayName("最后一次获取到的纬度")]
        public decimal LastLat { get; set; }

        /// <summary>
        /// 最后一次获取到的经度
        /// </summary>
        [DisplayName("最后一次获取到的经度")]
        public decimal LastLong { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="unixTimeStamp"></param>
        /// <returns></returns>
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

    }
}
