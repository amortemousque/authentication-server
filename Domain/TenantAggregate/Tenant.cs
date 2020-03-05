using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AuthorizationServer.Domain.Contracts;
using Newtonsoft.Json;

namespace AuthorizationServer.Domain.TenantAggregate
{

    public class Tenant
    {
	    public Guid Id { get; set; }

	    public Guid CreatedBy { get; set; }

	    public Guid UpdatedBy { get; set; }

	    public DateTime CreatedAt { get; set; }

	    public DateTime UpdatedAt { get; set; } 

        public string Name { get; set; }

        public long UsersNumber { get; set; }

        public int RegionId { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string LogoUri { get; set; }

        public string EmailSignature { get; set; }

        public bool Enabled { get; set; }

        public DateTime? ArchivedAt { get; set; }

        public override string ToString() => Name;

        public void UpdateInfos(string description, string logoUri, string emailSignature)
        {
            Description = description;
            LogoUri = logoUri;
			EmailSignature = emailSignature;
        }

        public void Disable()
        {
            Enabled = false;
        }

        public void Enable()
        {
            Enabled = true;
        }

        public void Archive()
        {
            ArchivedAt = DateTime.Now;
        }

        public static class Factory
        {
            public static async Task<Tenant> CreateNewEntry(
                ITenantRepository repository,
                string name,
                string displayName,
                string description
            )
            {

                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("The tenant must be specified", nameof(name));

				if (!Regex.IsMatch(name, "^(?=.{3,63}$)[a-z0-9]+(-[a-z0-9]+)*$"))
					throw new ArgumentException("The tenant name must have 3 to 63 characters, start with an alphanumeric character and contain alphanumeric characters or hyphens only");

                if (!await repository.HasUniqName(name))
                    throw new ArgumentException("An other tenant has the same name.", nameof(name));

                if (!await repository.HasUniqArchivedName(name))
                    throw new ArgumentException("An archived tenant has the same name.", nameof(name));

                var tenant = new Tenant
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    DisplayName = displayName,
                    Description = description,
                    Enabled = true,
                    RegionId = TenantRegion.EUROPE.Id,
                    UsersNumber = 0
                };

                return tenant;
            }
        }
    }
}    