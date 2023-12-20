// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using System.ComponentModel;

namespace b20Gesture.Components;


public enum EnumGesture
{
    [Description("无手势")]
    None,

    [Description("向左滑动")]
    Left,

    [Description("向右滑动")]
    Right,

    [Description("回退手势")]
    Back,

    [Description("翻页手势")]
    PageUp,

    [Description("刷新手势")]
    Refresh,

    [Description("退出手势")]
    Exit,
}
