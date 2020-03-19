namespace AuthorizationServer.Domain.UserAggregate
{
    using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Claims;
    using System.Threading.Tasks;
    using MongoDB.Bson.Serialization.Attributes;
    using Microsoft.AspNetCore.Identity;
    using Newtonsoft.Json;

    public class IdentityUser : IdentityUser<string>
	{
		public IdentityUser()
		{
            Roles = new List<string>();
			Logins = new List<IdentityUserLogin>();
			Claims = new List<IdentityUserClaim>();
			Tokens = new List<IdentityUserToken>();
		}

		// custom fields
        [PersonalData]
        public Guid TenantId { get; set; }

        [PersonalData]
        public Guid? PersonId { get; set; }

        [PersonalData]
        public string GivenName { get; set; }

        [PersonalData]
        public string FamilyName { get; set; }

        [PersonalData]
        public string Culture { get; set; }

        [PersonalData]
        public string Picture { get; set; }

        [PersonalData]
        public string FullName { get; set; }

        [PersonalData]
        public string NormalizedFullName { get; set; }

        [PersonalData]
        public Guid CreatedBy { get; set; }

        [PersonalData]
        public Guid UpdatedBy { get; set; }

        [PersonalData]
        public DateTime CreatedAt { get; set; }

        [PersonalData]
        public DateTime UpdatedAt { get; set; } 

        [PersonalData]
        public virtual DateTime? LoggedInAt { get; set; }

		public virtual DateTime? LockoutEndDateUtc { get; set; }

		[BsonIgnoreIfNull]
		[JsonIgnore]
		public virtual List<string> Roles { get; set; }

		public virtual void AddRole(string role)
		{
			Roles.Add(role);
		}

		public virtual void RemoveRole(string role)
		{
			Roles.Remove(role);
		}


		[BsonIgnoreIfNull]
        [JsonIgnore]
        public virtual List<IdentityUserLogin> Logins { get; set; }

		public virtual void AddLogin(UserLoginInfo login)
		{
			Logins.Add(new IdentityUserLogin(login));
		}

		public virtual void RemoveLogin(string loginProvider, string providerKey)
		{
			Logins.RemoveAll(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
		}

		public virtual bool HasPassword()
		{
            return !string.IsNullOrWhiteSpace(this.PasswordHash);
		}

		[BsonIgnoreIfNull]
        [JsonIgnore]
        public virtual List<IdentityUserClaim> Claims { get; set; }

		public virtual void AddClaim(Claim claim)
		{
			Claims.Add(new IdentityUserClaim(claim));
		}

		public virtual void RemoveClaim(Claim claim)
		{
			Claims.RemoveAll(c => c.Type == claim.Type && c.Value == claim.Value);
		}

		public virtual void ReplaceClaim(Claim existingClaim, Claim newClaim)
		{
			var claimExists = Claims
				.Any(c => c.Type == existingClaim.Type && c.Value == existingClaim.Value);
			if (!claimExists)
			{
				// note: nothing to update, ignore, no need to throw
				return;
			}
			RemoveClaim(existingClaim);
			AddClaim(newClaim);
		}

		[BsonIgnoreIfNull]
        [JsonIgnore]
        public virtual List<IdentityUserToken> Tokens { get; set; }

		private IdentityUserToken GetToken(string loginProider, string name)
			=> Tokens
				.FirstOrDefault(t => t.LoginProvider == loginProider && t.Name == name);

		public virtual void SetToken(string loginProider, string name, string value)
		{
			var existingToken = GetToken(loginProider, name);
			if (existingToken != null)
			{
				existingToken.Value = value;
				return;
			}

			Tokens.Add(new IdentityUserToken
			{
				LoginProvider = loginProider,
				Name = name,
				Value = value
			});
		}

		public virtual string GetTokenValue(string loginProider, string name)
		{
			return GetToken(loginProider, name)?.Value;
		}

		public virtual void RemoveToken(string loginProvider, string name)
		{
			Tokens.RemoveAll(t => t.LoginProvider == loginProvider && t.Name == name);
		}

		public override string ToString() => UserName;


        public void ChangeName(string givenName, string familyName) 
        {
            if (string.IsNullOrWhiteSpace(givenName))
                throw new ArgumentNullException(nameof(givenName));

            if (string.IsNullOrWhiteSpace(familyName))
                throw new ArgumentNullException(nameof(familyName));

            GivenName = givenName;
            FamilyName = familyName;
            FullName = $"{givenName} {familyName}";
        }

        public void ConfirmEmail() 
        {
            EmailConfirmed = true;
        }

        public static class Factory
        {
            public static async Task<IdentityUser> CreateNewEntry(
                Guid personId,
                string email,
                string givenName,
                string familyName,
                string culture,
                string picture,
                bool emailConfirmed
            )
            {
                if (personId == Guid.Empty)
                    throw new ArgumentNullException(nameof(personId));

                var user = new IdentityUser
                {
					PersonId = personId,
					Id = personId.ToString(),
                    UserName = email,
                    Email = email,
                    EmailConfirmed = emailConfirmed,
                    GivenName = givenName,
                    FamilyName = familyName,
                    FullName = $"{givenName} {familyName}",
                    Culture = culture,
                    Picture = picture,
                    Roles = new List<string>(),
                    Logins = new List<IdentityUserLogin>(),
                    Tokens = new List<IdentityUserToken>()
                };
                return user;
            }
        }
    }
}