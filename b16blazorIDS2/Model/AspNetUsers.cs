using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel;
#nullable disable

namespace b16blazorIDS2.Models.ids;

[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
public partial class AspNetUsers
{

    [JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
    public string Id { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string LockoutEnd { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty]
    public int TwoFactorEnabled { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty]
    public int PhoneNumberConfirmed { get; set; }

    [DisplayName("电话")]
    [JsonProperty, Column(StringLength = -2)]
    public string PhoneNumber { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string ConcurrencyStamp { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string SecurityStamp { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string PasswordHash { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty]
    public int EmailConfirmed { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string NormalizedEmail { get; set; }

    [JsonProperty, Column(StringLength = -2)]
    public string Email { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [DisplayName("标准化用户名")]
    [JsonProperty, Column(StringLength = -2)]
    public string NormalizedUserName { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty]
    public int LockoutEnabled { get; set; }

    [JsonProperty, Column(StringLength = -2)]
    public string UserName { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string Country { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string Province { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string City { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string County { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string Zip { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string Street { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string TaxNumber { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string provider { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string UUID { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string DOB { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string Name { get; set; }

    [DisplayName("用户组")]
    [JsonProperty, Column(StringLength = -2)]
    public string UserRole { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty]
    public int AccessFailedCount { get; set; }

    [JsonProperty, Column(IsIgnore = true)]
    [DisplayName("Role")]
    public string RoleName { get=> roleName ?? AspNetUserRoles?.First().RoleId; set=> roleName=value; }
    string roleName;

    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(AspNetUserClaims.UserId))]
    [DisplayName("Roles")]
    public virtual List<AspNetUserRoles> AspNetUserRoles { get; set; }

    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(AspNetUserClaims.UserId))]
    [DisplayName("Claims")]
    public virtual List<AspNetUserClaims> AspNetUserClaimss { get; set; }

    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(AspNetUserLogins.UserId))]
    [DisplayName("UserLogins")]
    public virtual List<AspNetUserLogins> AspNetUserLoginss { get; set; }

} 
