// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using Densen.Identity;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel; 

namespace Densen.Models.ids;

/// <summary>
/// 密钥
/// </summary>
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class Keys
{

    [DisplayName("ID")]
    [JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
    public string? Id { get; set; }

    [DisplayName("版本")]
    [JsonProperty]
    public int Version { get; set; }

    [DisplayName("创建")]
    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string? Created { get; set; }

    [DisplayName("使用")]
    [JsonProperty, Column(StringLength = -2)]
    public string? Use { get; set; }

    [DisplayName("算法")]
    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string? Algorithm { get; set; }

    [DisplayName("是X509证书")]
    [JsonProperty]

    [Column(MapType = typeof(int))]
    public IntToBool IsX509Certificate { get; set; }

    [DisplayName("数据保护")]
    [JsonProperty]

    [Column(MapType = typeof(int))]
    public IntToBool DataProtected { get; set; }

    [DisplayName("数据")]
    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    [AutoGenerateColumn(TextEllipsis = true, ComponentType = typeof(BootstrapBlazor.Components.Textarea))]
    public string? Data { get; set; }

}
