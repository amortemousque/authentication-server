using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using Microsoft.AspNetCore.Identity;

namespace AuthorizationServer.Domain.RoleAggregate
{

    public class IdentityRole : IdentityRole<string>
	{
		public static class NormalizedNames
		{
			public const string Candidate = "CANDIDATE";
			public const string Recruit = "RECRUIT";
			public const string Employee = "EMPLOYEE";
			public const string Manager = "MANAGER";
			public const string HRManager = "HRMANAGER";
			public const string Director = "DIRECTOR";
			public const string SupportCollaborator = "SUPPORT";
			private static readonly string[] All = {Candidate, Recruit, Employee, Manager, HRManager, Director};
			public static bool Contains(string role) => All.Contains(role);
		}

		public IdentityRole()
		{
            Id = Guid.NewGuid().ToString();
		}

		public IdentityRole(string roleName) : this()
		{
			Name = roleName;
			NormalizedName = Name.ToUpper();
		}


        [BsonIgnoreIfNull]
        [PersonalData]
        public virtual List<string> Permissions { get; set; }

        [PersonalData]
        public string Description { get; set; }

		[BsonId]
		public string Id { get; set; }

		public string Name { get; set; }

		public string NormalizedName { get; set; }

		public override string ToString() => Name;
	}
}