// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;

namespace AuthorizationServer.Domain.ClientAggregate
{
    public class ClientSecret : Secret
    {

        public ClientSecret(string value, string decrypted = null, DateTime? expiration = default(DateTime?))
        {
            this.Value = value;
            this.Expiration = expiration;
            this.Decrypted = decrypted;
        }
    }
}