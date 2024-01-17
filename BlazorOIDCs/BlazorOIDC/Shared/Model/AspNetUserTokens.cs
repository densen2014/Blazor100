// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel; 

namespace Densen.Models.ids;

/// <summary>
/// 用户令牌
/// </summary>
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class AspNetUserTokens
{

    [AutoGenerateColumn(Order = 1, TextEllipsis = true, LookupServiceKey = nameof(UserId), ShowSearchWhenSelect = false, IsPopover = true)]
    [DisplayName("用户ID")]
    [JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
    public string? UserId { get; set; }

    [DisplayName("名称")]
    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string? Name { get; set; }

    [DisplayName("外部登录提供程序")]
    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string? LoginProvider { get; set; }

    [DisplayName("值")]
    [JsonProperty, Column(StringLength = -2)]
    public string? Value { get; set; }

    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(UserId))]
    public virtual AspNetUsers? AspNetUsers { get; set; }

}
