using FreeSql.DatabaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using BootstrapBlazor.Components;
using DocumentFormat.OpenXml.Office2021.Excel.NamedSheetViews;
namespace b16blazorIDS2.Models.ids;

[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class AspNetRoleClaims
{

    [JsonProperty, Column(IsPrimary = true, IsIdentity = true)]
    public int Id { get; set; }

    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string RoleId { get; set; }

    [JsonProperty, Column(StringLength = -2)]
    public string ClaimType { get; set; }

    [JsonProperty, Column(StringLength = -2)]
    public string ClaimValue { get; set; }

    [Navigate(nameof(RoleId))]
    public virtual AspNetRoles AspNetRoles { get; set; }

}
