// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;

namespace AuthorizationServer.Domain.ApiAggregate
{
    public class ApiScope
    {
        // custom fields
        public Guid Id { get; set; }
        public Guid ApiResourceId { get; set; }

        // standard fields
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;

        /// List of user-claim types that should be included in the access token.
        public List<string> UserClaims { get; set; }
    }
}