// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using AuthorizationServer.Infrastructure.IdentityServer.Mappers;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using AuthorizationServer.Infrastructure.Context;
using AuthorizationServer.Infrastructure.IdentityServer.Mappers;

namespace AuthorizationServer.Infrastructure.IdentityServer.Stores
{
    public class ResourceStore : IResourceStore
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ResourceStore> _logger;

        public ResourceStore(ApplicationDbContext context, ILogger<ResourceStore> logger)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            _context = context;
            _logger = logger;
        }

        public Task<IdentityServer4.Models.ApiResource> FindApiResourceAsync(string name)
        {
            var apis =
                from apiResource in _context.ApiResources.AsQueryable()
                where apiResource.Name == name
                select apiResource;

            var api = apis.FirstOrDefault();

            if (api != null)
            {
                _logger.LogDebug("Found {api} API resource in database", name);
            }
            else
            {
                _logger.LogDebug("Did not find {api} API resource in database", name);
            }

            return Task.FromResult(api.ToModel());
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var names = scopeNames.ToArray();

            var apis =
                from api in _context.ApiResources.AsQueryable()
                where api.Scopes.Where(x => names.Contains(x.Name)).Any()
                select api;

            var results = apis.ToArray();
            var models = results.Select(x => x.ToModel()).ToArray();

            _logger.LogDebug("Found {scopes} API scopes in database", models.SelectMany(x => x.Scopes).Select(x => x.Name));

            return Task.FromResult(models.AsEnumerable());
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();

            var resources =
                from identityResource in _context.IdentityResources.AsQueryable()
                where scopes.Contains(identityResource.Name)
                select identityResource;

            var results = resources.ToArray();

            _logger.LogDebug("Found {scopes} identity scopes in database", results.Select(x => x.Name));

            return Task.FromResult(results.Select(x => x.ToModel()).ToArray().AsEnumerable());
        }

        public Task<IdentityServer4.Models.Resources> GetAllResourcesAsync()
        {
            var identity = _context.IdentityResources.AsQueryable();

            var apis = _context.ApiResources.AsQueryable();

            var result = new IdentityServer4.Models.Resources(
                identity.ToArray().Select(x => x.ToModel()).AsEnumerable(),
                apis.ToArray().Select(x => x.ToModel()).AsEnumerable());

            _logger.LogDebug("Found {scopes} as all scopes in database", result.IdentityResources.Select(x => x.Name).Union(result.ApiResources.SelectMany(x => x.Scopes).Select(x => x.Name)));

            return Task.FromResult(result);
        }
    }
}