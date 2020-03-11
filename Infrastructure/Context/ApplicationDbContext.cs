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
using System.Threading.Tasks;
using AuthorizationServer.Domain.AccountInviteAggregate;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace AuthorizationServer.Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext, IDisposable
    {
        private readonly IMongoDatabase _database;


        public ApplicationDbContext(IMongoDatabase database)
        {
	        _database = database;
        }

        public ApplicationDbContext(string connectionString)
        {
            if (connectionString == null) 
                throw new ArgumentNullException(nameof(connectionString), "MongoDBConfiguration cannot be null."); 
 
            var mongoDbMementoDatabaseName = MongoUrl.Create(connectionString).DatabaseName;
            var databaseName = MongoUrl.Create(connectionString).DatabaseName; 
 
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public async Task AddClient(Client entity)
        {
            await Clients.InsertOneAsync(entity);
        }

        public async Task AddIdentityResource(IdentityResource entity)
        {
            await IdentityResources.InsertOneAsync(entity);
        }

        public async Task AddApiResource(ApiResource entity)
        {
            await ApiResources.InsertOneAsync(entity);
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

        public IMongoCollection<AccountInvite> Invites => _database.GetCollection<AccountInvite>("AccountInvites");

        public void Dispose()
        {
            //Todo
        }
    }
}    