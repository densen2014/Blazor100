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
/// 角色声明
/// </summary>
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class AspNetRoleClaims
{

    [AutoGenerateColumn(Visible = false, Order = 1, Width = 30, TextEllipsis = true)]
    [DisplayName("ID")]
    [JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
    public int Id { get; set; }

    [AutoGenerateColumn(Order = 1, LookupServiceKey = nameof(RoleId), ShowSearchWhenSelect = false, IsPopover = true)]
    [DisplayName("角色ID")]
    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string? RoleId { get; set; }

    [DisplayName("角色声明")]
    [JsonProperty, Column(StringLength = -2)]
    public string? ClaimType { get; set; }

    [DisplayName("值")]
    [JsonProperty, Column(StringLength = -2)]
    public string? ClaimValue { get; set; }

    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(RoleId))]
    public virtual AspNetRoles? AspNetRoles { get; set; }

}
