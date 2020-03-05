// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AutoMapper;
using AuthorizationServer.Domain.IdenityServer;

namespace AuthorizationServer.Infrastructure.IdentityServer.Mappers
{
    public static class PersistedGrantMappers
    {
        static PersistedGrantMappers()
        {
            Mapper = new MapperConfiguration(cfg =>cfg.AddProfile<PersistedGrantMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static IdentityServer4.Models.PersistedGrant ToModel(this PersistedGrant token)
        {
            return token == null ? null : Mapper.Map<IdentityServer4.Models.PersistedGrant>(token);
        }

        public static PersistedGrant ToEntity(this IdentityServer4.Models.PersistedGrant token)
        {
            return token == null ? null : Mapper.Map<PersistedGrant>(token);
        }

        public static void UpdateEntity(this IdentityServer4.Models.PersistedGrant token, PersistedGrant target)
        {
            Mapper.Map(token, target);
        }
    }
}