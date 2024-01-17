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
/// 角色表
/// <para>存储向哪些用户分配哪些角色</para>
/// </summary>
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class AspNetUserRoles
{

    [AutoGenerateColumn(Order = 2, TextEllipsis = true, LookupServiceKey = nameof(UserId), ShowSearchWhenSelect = false, IsPopover = true)]
    [DisplayName("用户")]
    [JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
    public string? UserId { get; set; }

    //[JsonProperty, Column(IsIgnore = true)]
    //[AutoGenerateColumn(Visible = false)]
    //[DisplayName("用户")]
    //public string UserName { get => userName ?? (AspNetUserss?.UserName); set => userName = value; }

    //private string userName;

    [AutoGenerateColumn(Order = 1, LookupServiceKey = nameof(RoleId), ShowSearchWhenSelect = false, IsPopover = true)]
    [DisplayName("角色名称")]
    [JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
    public string? RoleId { get; set; }

    [AutoGenerateColumn(Ignore = true)]
    [JsonProperty, Column(IsIgnore = true)]
    [DisplayName("角色名称")]
    public string? RoleName { get => roleName ?? (AspNetRoless?.Name); set => roleName = value; }

    private string? roleName;

    /// <summary>
    /// 角色定义
    /// </summary>
    [DisplayName("角色定义")]
    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(RoleId))]
    public virtual AspNetRoles? AspNetRoless { get; set; }

    /// <summary>
    /// 用户表
    /// </summary>
    [DisplayName("用户表")]
    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(UserId))]
    public virtual AspNetUsers? AspNetUserss { get; set; }

}

