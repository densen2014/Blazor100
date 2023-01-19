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
	public partial class AspNetUsers {

		[JsonProperty, Column(StringLength = -2, IsPrimary = true, IsNullable = false)]
		public string Id { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string LockoutEnd { get; set; }

		[JsonProperty]
		public int TwoFactorEnabled { get; set; }

		[JsonProperty]
		public int PhoneNumberConfirmed { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string PhoneNumber { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string ConcurrencyStamp { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string SecurityStamp { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string PasswordHash { get; set; }

		[JsonProperty]
		public int EmailConfirmed { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string NormalizedEmail { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Email { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string NormalizedUserName { get; set; }

		[JsonProperty]
		public int LockoutEnabled { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string UserName { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Country { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Province { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string City { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string County { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Zip { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Street { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string TaxNumber { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string provider { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string UUID { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string DOB { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string Name { get; set; }

		[JsonProperty, Column(StringLength = -2)]
		public string UserRole { get; set; }

		[JsonProperty]
		public int AccessFailedCount { get; set; }

	}

}
