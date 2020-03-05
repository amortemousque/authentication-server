// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace AuthorizationServer.Domain.RessourceAggregate
{
    public class IdentityResource
    {
        public Guid Id { get; set; }
        public bool Enabled { get; set; } = true;
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;

        /// List of accociated user claims that should be included when this resource is requested.
        public List<string> UserClaims { get; set; }
    }
}
