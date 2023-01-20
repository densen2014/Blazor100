using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;
#nullable disable

namespace b16blazorIDS2.Models.ids;

[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class AspNetUserRoles
{

    [JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
    public string UserId { get; set; }

    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string RoleId { get; set; }

    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(RoleId))]
    public virtual AspNetRoles AspNetRoles { get; set; }

    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(UserId))]
    public virtual AspNetUsers AspNetUsers { get; set; }

}
