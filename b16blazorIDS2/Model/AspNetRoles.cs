using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;
#nullable disable

namespace b16blazorIDS2.Models.ids;

[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class AspNetRoles
{

    [JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
    public string Id { get; set; }

    [JsonProperty, Column(StringLength = -2)]
    public string Name { get; set; }

    [JsonProperty, Column(StringLength = -2)]
    public string NormalizedName { get; set; }

    [JsonProperty, Column(StringLength = -2)]
    public string ConcurrencyStamp { get; set; }

}
