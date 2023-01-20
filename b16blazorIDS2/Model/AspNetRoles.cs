using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel;
#nullable disable

namespace b16blazorIDS2.Models.ids;

/// <summary>
/// 角色
/// </summary>
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class AspNetRoles
{

    [DisplayName("角色ID")]
    [JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
    public string Id { get; set; }

    [DisplayName("角色")]
    [JsonProperty, Column(StringLength = -2)]
    public string Name { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string NormalizedName { get; set; }

    [AutoGenerateColumn(Visible = false)]
    [JsonProperty, Column(StringLength = -2)]
    public string ConcurrencyStamp { get; set; }

}
