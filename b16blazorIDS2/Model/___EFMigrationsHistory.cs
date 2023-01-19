using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace b16blazorIDS2.Models.ids {

	[JsonObject(MemberSerialization.OptIn), Table(Name = "__EFMigrationsHistory", DisableSyncStructure = true)]
	public partial class ___EFMigrationsHistory {

		[JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
		public string MigrationId { get; set; }

		[JsonProperty, Column(StringLength = -2, IsNullable = false)]
		public string ProductVersion { get; set; }

	}

}
