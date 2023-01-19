using FreeSql.DatabaseModel;using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using FreeSql.DataAnnotations;

namespace b16blazorIDS2.Models.ids {

	[JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
	public partial class AspNetUserLogins {

		[JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
		public string LoginProvider { get; set; }

		[JsonProperty, Column(StringLength = -2, IsNullable = false)]
		public string UserId { get; set; }

		[JsonProperty, Column(StringLength = -2, IsNullable = false)]
		public string ProviderKey { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string ProviderDisplayName { get; set; }

	}

}
