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
/// 角色定义
/// </summary>
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class AspNetRoles
{

    [AutoGenerateColumn(Visible = false, Editable = false, Order = 1, Width = 30, TextEllipsis = true)]
    [DisplayName("ID")]
    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string? Id { get; set; }

    [DisplayName("角色")]
    [JsonProperty, Column(StringLength = -2)]
    public string? Name { get; set; }

    [DisplayName("标准化名称")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? NormalizedName { get; set; }

    [DisplayName("并发票据")]
    [AutoGenerateColumn(Visible = false, Editable = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? ConcurrencyStamp { get; set; }

    //导航属性
    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(AspNetUserRoles.RoleId))]
    [DisplayName("角色表")]
    public virtual List<AspNetUserRoles>? AspNetUserRoless { get; set; }

}
