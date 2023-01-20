using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel;
#nullable disable

namespace b16blazorIDS2.Models.ids;

/// <summary>
/// 角色表
/// </summary>
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class AspNetUserRoles
{

    [DisplayName("用户ID")]
    [JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
    public string UserId { get; set; }

    [DisplayName("角色ID")]
    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string RoleId { get; set; }

    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(RoleId))]
    public virtual AspNetRoles AspNetRoles { get; set; }

    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(UserId))]
    public virtual AspNetUsers AspNetUsers { get; set; }

}
