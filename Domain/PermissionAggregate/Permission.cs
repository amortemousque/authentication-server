using System;
using System.Threading.Tasks;
using global::MongoDB.Bson;
using global::MongoDB.Bson.Serialization.Attributes;
using AuthorizationServer.Domain.Contracts;

namespace AuthorizationServer.Domain.PermissionAggregate
{
    public class Permission
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public void UpdateInfos(string description)
        {
            Description = description;
        }

        public static class Factory
        {
            public static async Task<Permission> CreateNewEntry(
                IPermissionRepository repository,
                string name,
                string description
            )
            {

                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("The permission name must be specified", nameof(name));


                if (!await repository.HasUniqName(name))
                    throw new ArgumentException("An other permission has the same name.", nameof(name));


                var permission = new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Description = description
                };

                return permission;
            }
        }
    }
}        