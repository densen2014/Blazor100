using BootstrapBlazor.Components;
using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel;
#nullable disable

namespace b16blazorIDS2.Models.ids;

/// <summary>
/// 用户声明
/// </summary>
[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class AspNetUserClaims
{

    [DisplayName("声明ID")]
    [JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
    public int Id { get; set; }

    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string UserId { get; set; }

    [DisplayName("索赔类型")]
    [JsonProperty, Column(StringLength = -2)]
    public string ClaimType { get; set; }

    [JsonProperty, Column(StringLength = -2)]
    public string ClaimValue { get; set; }

    /// <summary>
    /// 用户
    /// </summary>
    [AutoGenerateColumn(Ignore = true)]
    [Navigate(nameof(UserId))]
    public virtual AspNetUsers AspNetUsers { get; set; }

}
