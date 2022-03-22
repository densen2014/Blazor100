using System.Text.Json.Serialization;

namespace Blazor100.Components
{
    /// <summary>
    /// 键盘语言布局
    /// </summary>
    public enum KeyboardKeysType
    {
        arabic,
        english,
        french,
        german,
        hungarian,
        persian,
        russian,
        spanish,
        turkish
    }

    /// <summary>
    /// 键盘类型,全键盘 || 字母 || 小数字键盘
    /// </summary>
    public enum KeyboardType
    {
        all,
        keyboard,
        numpad,
    }

    /// <summary>
    /// 对齐, 顶端 || 底部
    /// </summary>
    public enum KeyboardPlacement
    {
        bottom,
        top
    }

    /// <summary>
    /// 特殊符号键盘类型, 默认 || 欧洲 || 自定义
    /// </summary>
    public enum KeyboardSpecialcharacters
    {
        all,
        europe,
        customer
    }

    /// <summary>
    /// 键盘主题
    /// </summary>
    public enum KeyboardTheme
    {
        light,
        dark,
        flat,
        material,
        oldschool
    }

    /// <summary>
    /// 打开或关闭键盘的 CSS 动画样式
    /// </summary>
    public enum KeyboardCssAnimationsStyle
    {
        slide,
        fade,
        flat,
        material,
        oldschool
    }

    public class KeyboardOption
    {

        /// <summary>
        /// 必需:必须为自定义键定义一个对象数组。<para></para>
        /// 提示：每个对象在键盘上创建一个行元素 (HTML)。<para></para>
        /// 例如 [{"key":"value"}, {"key":"value"}] => [{"0":"A","1":"B","2":"C" }, {"0":"D","1":"E","2":"F"}] 
        /// </summary>
        public List<Dictionary<string, string>>? keysArrayOfObjects { get; set; } = null;



        /// <summary>
        /// 键盘类型 <para></para>
        /// arabic || english || french || german || hungarian || persian || russian || spanish || turkish
        /// <para></para>仅当“keysArrayOfObjects”为“null”时才需要设置
        /// </summary>
        [JsonIgnore]
        public KeyboardKeysType KeyboardKeysType { get; set; } = KeyboardKeysType.english;



        /// <summary>
        /// 仅当“keysArrayOfObjects”为“null”时才需要。<para></para>
        /// “kioskboard-keys-${langugage}.json”文件的路径必须设置为“keysJsonUrl”选项。（XMLHttpRequest 从 JSON 文件中获取密钥。）<para></para>
        /// 例如 '/Content/Plugins/KioskBoard/dist/kioskboard-keys-english.json' 
        /// </summary>
        public string? keysJsonUrl { get => keysArrayOfObjects == null ? $"./lib/kioskboard/kioskboard-keys-{KeyboardKeysType}.json" : null; }



        /// <summary>
        /// 特殊符号键盘类型, 默认 || 欧洲 || 自定义
        /// </summary>
        [JsonIgnore]
        public KeyboardSpecialcharacters KeyboardSpecialcharacters { get; set; } = KeyboardSpecialcharacters.all;



        /// <summary>
        /// 自定义特殊符号键盘 , 字符串数组覆盖内置的特殊字符。<para></para>
        /// 例如 ["#", "€", "%", "+", "-", "*"] 
        /// </summary>
        [JsonIgnore]
        public string[]? CustomerKeyboardSpecialcharacters { get; set; }

        string[] KeyboardSpecialcharactersEurope { get; set; } = { "#", "€", "Ñ" };



        /// <summary> 
        /// 可选：自定义特殊符号键盘
        /// </summary>
        public string[]? keysSpecialCharsArrayOfStrings
        {
            get =>
                 CustomerKeyboardSpecialcharacters != null ?
                 CustomerKeyboardSpecialcharacters :
                 KeyboardSpecialcharacters == KeyboardSpecialcharacters.europe ?
                 KeyboardSpecialcharactersEurope :
                 null;
        }



        /// <summary> 
        /// 可选：可以设置一个数字数组来覆盖内置的小键盘键。（从 0 到 9，顺序不限。）
        /// 例如 [1, 2, 3, 4, 5, 6, 7, 8, 9, 0] 
        /// </summary>
        public string? keysNumpadArrayOfNumbers { get; set; } = null;



        /// <summary> 
        ///可选：自定义键的语言代码 (ISO 639-1)（用于语言支持）
        ///<para></para> 例如 "de" || "en" || "fr" || "hu" || "tr" 等...
        /// </summary>
        public string language { get; set; } = "en";



        /// <summary> 
        /// 键盘主题  <para></para>  "light" || "dark" || "flat" || "material" || "oldschool"
        /// </summary>
        [JsonIgnore]
        public KeyboardTheme Theme { get; set; } = KeyboardTheme.light;
        public string theme { get => Theme.ToString(); }



        /// <summary> 
        /// 大写或小写锁定。默认false小写 
        /// </summary>
        public bool capsLockActive { get; set; } = false;



        /// <summary> 
        /// 允许或阻止真实/物理键盘的使用。“false”时被阻止<para></para>
        /// 此外，如果想要使用真实/物理键盘，“allowMobileKeyboard”选项也必须为“true”。
        /// </summary>
        public bool allowRealKeyboard { get; set; } = true;



        /// <summary>
        /// 允许或阻止使用移动键盘。当 "false" 
        /// </summary>
        public bool allowMobileKeyboard { get; set; } = true;

        /// <summary>
        /// 打开或关闭键盘的 CSS 动画
        /// </summary>
        public bool cssAnimations { get; set; } = true;

        /// <summary>
        /// CSS 动画持续时间为毫秒
        /// </summary>
        public int cssAnimationsDuration { get; set; } = 360;

        /// <summary>
        /// 打开或关闭键盘的 CSS 动画样式 => "slide" || "fade" 
        /// </summary>
        [JsonIgnore]
        public KeyboardCssAnimationsStyle CssAnimationsStyle { get; set; } = KeyboardCssAnimationsStyle.slide;
        public string cssAnimationsStyle { get => CssAnimationsStyle.ToString(); }

        /// <summary>
        /// 启用或禁用键盘上的空格键功能。
        /// </summary>
        public bool keysAllowSpacebar { get; set; } = true;

        /// <summary>
        /// 空格键（空格键）的文本。不设置显示为" "
        /// </summary>
        public string keysSpacebarText { get; set; } = "Space";

        /// <summary>
        /// 键的字体系列
        /// </summary>
        public string keysFontFamily { get; set; } = "sans-serif";

        /// <summary>
        /// 按键的字体大小
        /// </summary>
        public string keysFontSize { get; set; } = "22px";

        /// <summary>
        /// 按键的字体粗细
        /// </summary>
        public string keysFontWeight { get; set; } = "normal";

        /// <summary>
        /// 图标键的大小
        /// </summary>
        public string keysIconSize { get; set; } = "25px";

        /// <summary>
        /// 将文档滚动到 input/textarea 元素的顶部或底部（通过放置选项）
        /// </summary>
        public bool autoScroll { get; set; } = false;
    }
}
