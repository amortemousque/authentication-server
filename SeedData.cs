// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using AuthorizationServer.Infrastructure.Context;
using AuthorizationServer.Infrastructure.IdentityServer.Mappers;

namespace AuthorizationServer
{
    public class SeedData
    {
        public async static void EnsureSeedDataServer4(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                try {
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    if (!context.Clients.AsQueryable().Any())
                    {
                        foreach (var client in Configuration.Clients.Get().ToList())
                        {
                           await context.AddClient(client.ToEntity());
                        }
                    }

                    var ressource = await context.IdentityResources.Find(_ => true).ToListAsync();
                    foreach (var resource in Configuration.Resources.GetIdentityResources().ToList())
                    {
                        if(!ressource.Any(r => r.Name == resource.Name)) {
                            await context.AddIdentityResource(resource.ToEntity());
                        }
                    }


                    if (!context.ApiResources.AsQueryable().Any())
                    {
                        foreach (var resource in Configuration.Resources.GetApiResources().ToList())
                        {
                            await context.AddApiResource(resource.ToEntity());
                        }
                    }
                } catch (Exception ex) {
                    var toto = ex;
                } 

            }
        }
    }
}
