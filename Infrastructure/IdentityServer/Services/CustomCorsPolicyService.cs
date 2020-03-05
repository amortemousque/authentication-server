// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using AuthorizationServer.Infrastructure.Context;

namespace AuthorizationServer.Infrastructure.IdentityServer.Services
{
    public class CustomCorsPolicyService : ICorsPolicyService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CustomCorsPolicyService> _logger;


        public CustomCorsPolicyService(ApplicationDbContext context, ILogger<CustomCorsPolicyService> logger)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            
            _context = context;
            _logger = logger;
        }

        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            // If we use SelectMany directly, we got a NotSupportedException inside MongoDB driver.
            // Details: 
            // System.NotSupportedException: Unable to determine the serialization information for the collection 
            // selector in the tree: aggregate([]).SelectMany(x => x.AllowedCorsOrigins.Select(y => y.Origin))
            var origins = _context.Clients.AsQueryable().Select(x => x.AllowedCorsOrigins).ToList();

            // As a workaround, we use SelectMany in memory.
            var distinctOrigins = origins.SelectMany(o => o).Where(x => x != null).Distinct();
            foreach (var uri in distinctOrigins)
            {
                if (WildcardUrlHelper.Compare(uri, origin))
                {
                    return Task.FromResult(true);
                }
            }

            return Task.FromResult(false);
        }
    }
}