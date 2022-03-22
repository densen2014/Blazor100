// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************


using System;
using System.ComponentModel;

namespace Blazor100.Components
{

    /// <summary>
    /// Baidu定位数据类
    /// </summary>
    public class BaiduItem
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


    }
}
