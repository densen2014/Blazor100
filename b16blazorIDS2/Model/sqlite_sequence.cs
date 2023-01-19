using FreeSql.DatabaseModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace b16blazorIDS2.Models.ids;

[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
public partial class sqlite_sequence
{

}
