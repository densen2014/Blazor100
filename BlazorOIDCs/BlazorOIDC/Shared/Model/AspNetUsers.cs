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
/// 用户表
/// </summary>
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
public partial class AspNetUsers
{

    [AutoGenerateColumn(Visible = false, Order = 1, Width = 30, TextEllipsis = true)]
    [DisplayName("用户ID")]
    [JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
    public string? Id { get; set; }

    [JsonProperty, Column(StringLength = -2)]
    [DisplayName("用户名")]
    public string? UserName { get; set; }

    [JsonProperty, Column(IsIgnore = true)]
    [DisplayName("角色")]
    public string RoleName { get => roleName ?? (AspNetUserRoless != null ? string.Join(",", AspNetUserRoless?.Select(a => a.RoleName ?? a.RoleId).ToList()) : ""); set => roleName = value; }

    private string? roleName;

    [JsonProperty, Column(StringLength = -2)]
    public string? Email { get; set; }

    [DisplayName("电话")]
    [JsonProperty, Column(StringLength = -2)]
    public string? PhoneNumber { get; set; }

    [DisplayName("自定义名称")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? Name { get; set; }

    [DisplayName("自定义角色")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? UserRole { get; set; }

    [DisplayName("密码哈希")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? PasswordHash { get; set; }

    [DisplayName("电子邮件已确认")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty]
    [Column(MapType = typeof(int))]
    public IntToBool EmailConfirmed { get; set; }

    [DisplayName("电话号码已确认")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty]
    [Column(MapType = typeof(int))]
    public IntToBool PhoneNumberConfirmed { get; set; }

    [DisplayName("锁定结束")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? LockoutEnd { get; set; }

    [DisplayName("启用双因素登录")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty]
    [Column(MapType = typeof(int))]
    public IntToBool TwoFactorEnabled { get; set; }

    [DisplayName("并发票据")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? ConcurrencyStamp { get; set; }

    [DisplayName("防伪印章")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? SecurityStamp { get; set; }

    [DisplayName("标准化电子邮件")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? NormalizedEmail { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [DisplayName("标准化用户名")]
    [JsonProperty, Column(StringLength = -2)]
    public string? NormalizedUserName { get; set; }

    [DisplayName("启用锁定")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty]
    [Column(MapType = typeof(int))]
    public IntToBool LockoutEnabled { get; set; }

    [DisplayName("国家")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? Country { get; set; }

    [DisplayName("省")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? Province { get; set; }

    [DisplayName("城市")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? City { get; set; }

    [DisplayName("县")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? County { get; set; }

    [DisplayName("邮编")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? Zip { get; set; }

    [DisplayName("街道")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? Street { get; set; }

    [DisplayName("税号")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? TaxNumber { get; set; }

    [DisplayName("提供者")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? provider { get; set; }

    [DisplayName("UUID")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string? UUID { get; set; }

    [DisplayName("生日")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public DateTime? DOB { get; set; }

    [DisplayName("访问失败次数")]
    [AutoGenerateColumn(Visible = false)]
    [JsonProperty]
    public int AccessFailedCount { get; set; }

    //导航属性

    /// <summary>
    /// 角色表
    /// </summary>
    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(AspNetUserRoles.UserId))]
    [DisplayName("角色表")]
    public virtual List<AspNetUserRoles>? AspNetUserRoless { get; set; }

    /// <summary>
    /// 用户声明
    /// </summary>
    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(AspNetUserClaims.UserId))]
    [DisplayName("用户声明")]
    public virtual List<AspNetUserClaims>? AspNetUserClaimss { get; set; }

    /// <summary>
    /// 用户登录
    /// </summary>
    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(AspNetUserLogins.UserId))]
    [DisplayName("用户登录")]
    public virtual List<AspNetUserLogins>? AspNetUserLoginss { get; set; }

    /// <summary>
    /// 1st角色
    /// </summary>
    [JsonProperty, Column(IsIgnore = true)]
    [DisplayName("1st角色")]
    public string? RoleName1st { get => roleName1st ?? ((AspNetUserRoless != null && AspNetUserRoless.Any()) ? AspNetUserRoless?.Select(a => a.RoleName ?? a.RoleId ?? "").First() : ""); set => roleName1st = value; }

    private string? roleName1st;

}
