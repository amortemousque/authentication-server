// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AuthorizationServer.Domain.ApiAggregate;
using AuthorizationServer.Domain.ClientAggregate;
using AuthorizationServer.Domain.IdenityServer;
using AuthorizationServer.Domain.PermissionAggregate;
using AuthorizationServer.Domain.RessourceAggregate;
using AuthorizationServer.Domain.RoleAggregate;
using AuthorizationServer.Domain.TenantAggregate;
using AuthorizationServer.Domain.UserAggregate;
using AuthorizationServer.Infrastructure.IdentityServer;
using MongoDB.Driver;
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AuthorizationServer.Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext, IDisposable
    {
        private readonly IMongoDatabase _database;

        public ApplicationDbContext(IApplicationDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            _database = client.GetDatabase(settings.DatabaseName);
        }


        public IMongoCollection<Tenant> Tenants
        {
            get
            {
                return _database.GetCollection<Tenant>("Tenants");
            }
        }


        public IMongoCollection<Client> Clients
        {
            get
            {
                return _database.GetCollection<Client>(Constants.TableNames.Client);
            }
        }

        public IMongoCollection<IdentityResource> IdentityResources
        {
            get
            {
                return _database.GetCollection<IdentityResource>(Constants.TableNames.IdentityResource);
            }
        }


        public IMongoCollection<ApiResource> ApiResources
        {
            get
            {
                return _database.GetCollection<ApiResource>(Constants.TableNames.ApiResource);
            }
        }

        public IMongoCollection<PersistedGrant> PersistedGrants
        {
            get
            {
                return _database.GetCollection<PersistedGrant>(Constants.TableNames.PersistedGrant);
            }
        }


        public IMongoCollection<IdentityUser> Users
        {
            get
            {
                return _database.GetCollection<IdentityUser>("Users");
            }
        }


        public IMongoCollection<IdentityRole> Roles
        {
            get
            {
                return _database.GetCollection<IdentityRole>("Roles");
            }
        }

        public IMongoCollection<Permission> Permissions
        {
            get
            {
                return _database.GetCollection<Permission>("Permissions");
            }
        }
    }
}    