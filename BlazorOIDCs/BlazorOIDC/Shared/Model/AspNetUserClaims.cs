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
/// 用户声明
/// </summary>
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class AspNetUserClaims
{

    [AutoGenerateColumn(Visible = false, Order = 1, Width = 30, TextEllipsis = true)]
    [DisplayName("ID")]
    [JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
    public int Id { get; set; }

    [AutoGenerateColumn(Order = 2, TextEllipsis = true, LookupServiceKey = nameof(UserId), ShowSearchWhenSelect = false, IsPopover = true)]
    [DisplayName("用户ID")]
    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string? UserId { get; set; }

    [DisplayName("声明类型")]
    [JsonProperty, Column(StringLength = -2)]
    public string? ClaimType { get; set; }

    [DisplayName("值")]
    [JsonProperty, Column(StringLength = -2)]
    public string? ClaimValue { get; set; }

    /// <summary>
    /// 用户
    /// </summary>
    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(UserId))]
    public virtual AspNetUsers? AspNetUsers { get; set; }

}
