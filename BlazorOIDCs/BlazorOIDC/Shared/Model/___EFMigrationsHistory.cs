// ********************************** 
// Densen Informatica 中讯科技 
// 作者：Alex Chow
// e-mail:zhouchuanglin@gmail.com 
// **********************************

using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace Densen.Models.ids;

[JsonObject(MemberSerialization.OptIn), Table(Name = "__EFMigrationsHistory", DisableSyncStructure = true)]
public partial class ___EFMigrationsHistory
{

    [JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
    public string? MigrationId { get; set; }

    [JsonProperty, Column(StringLength = -2, IsNullable = false)]
    public string? ProductVersion { get; set; }

}
