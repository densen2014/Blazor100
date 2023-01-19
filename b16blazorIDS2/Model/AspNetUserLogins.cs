using FreeSql.DatabaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;
using System.ComponentModel;
using BootstrapBlazor.Components;
using DocumentFormat.OpenXml.Office2021.Excel.NamedSheetViews;

namespace b16blazorIDS2.Models.ids;

[AutoGenerateClass(Searchable = true, Filterable = true, Sortable = true, ShowTips = true)]
[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class AspNetUserLogins
{

    [DisplayName("外联登录")]
    [JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
    public string LoginProvider { get; set; }

    [DisplayName("用户ID")]
    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string UserId { get; set; }

    [DisplayName("外联Key")]
    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string ProviderKey { get; set; }

    [DisplayName("外联名称")]
    [JsonProperty, Column(StringLength = -2)]
    public string ProviderDisplayName { get; set; }

    /// <summary>
    /// 用户
    /// </summary>
    [Navigate(nameof(UserId))]

    public virtual AspNetUsers AspNetUsers { get; set; }

}
