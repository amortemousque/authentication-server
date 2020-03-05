// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static IdentityServer4.IdentityServerConstants;
using IModels = IdentityServer4.Models;
using System.Linq;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.Service;

namespace AuthorizationServer.Domain.ClientAggregate
{
    public class Client
    {
        public Guid Id { get; set; }
        public bool Enabled { get; set; } = true;
        public string ClientId { get; set; }
        public string ProtocolType { get; set; } = ProtocolTypes.OpenIdConnect;
        public List<ClientSecret> ClientSecrets { get; set; }
        public bool RequireClientSecret { get; set; } = true;
        public string ClientName { get; set; }
        public string Description { get; set; }
        public string ClientUri { get; set; }
        public string LogoUri { get; set; }
        public bool RequireConsent { get; set; } = true;
        public bool AllowRememberConsent { get; set; } = true;
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        /// Specifies the allowed grant types (legal combinations of AuthorizationCode, Implicit, Hybrid, ResourceOwner, ClientCredentials).
        public List<string> AllowedGrantTypes { get; set; }

        public bool RequirePkce { get; set; }
        public bool AllowPlainTextPkce { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }

        public List<string> RedirectUris { get; set; }

        public List<string> PostLogoutRedirectUris { get; set; }

        public string FrontChannelLogoutUri { get; set; }
        public bool FrontChannelLogoutSessionRequired { get; set; } = true;
        public string BackChannelLogoutUri { get; set; }
        public bool BackChannelLogoutSessionRequired { get; set; } = true;
        public bool AllowOfflineAccess { get; set; }

        /// Specifies the api scopes that the client is allowed to request. If empty, the client can't access any scope
        public List<string> AllowedScopes { get; set; }

        public int IdentityTokenLifetime { get; set; } = 86400;//300;
		public int AccessTokenLifetime { get; set; } = 86400; //3600;
		public int AuthorizationCodeLifetime { get; set; } = 86400;//300;
        public int? ConsentLifetime { get; set; } = null;
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        public int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        public int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;
        public int AccessTokenType { get; set; } = (int)0; // AccessTokenType.Jwt;
        public bool EnableLocalLogin { get; set; } = true;

        /// Specifies which external IdPs can be used with this client (if list is empty all IdPs are allowed). Defaults to empty.
        public List<string> IdentityProviderRestrictions { get; set; }
        public bool IncludeJwtId { get; set; }

        /// Allows settings claims for the client (will be included in the access token).
        public List<ClientClaim> Claims { get; set; }

        public bool AlwaysSendClientClaims { get; set; }

        public string ClientClaimsPrefix { get; set; } = "client_";

        public string PairWiseSubjectSalt { get; set; }

        public List<string> AllowedCorsOrigins { get; set; }

        public List<ClientProperty> Properties { get; set; }


        public int ClientTypeId { get; set; }


        public async Task UpdateSettings(
            IClientRepository repository,
            string clientName,
            string description,
            string clientUri,
            string logoUri,
            bool requireClientSecret,
            bool requireConsent,
            bool alwaysIncludeUserClaimsInIdToken,
            bool allowAccessTokensViaBrowser,
            bool ResourceOwnerEnabled,
            int identityTokenLifetime,
            List<string> redirectUris,
            List<string> allowedCorsOrigins,
            List<string> allowedScopes
           ) {
            

            if (!await repository.HasUniqName(clientName, Id))
                throw new ArgumentException("An other tenant has the same name.", nameof(clientName));


            RequireClientSecret = requireClientSecret;
            ClientName = clientName;
            Description = description;
            ClientUri = clientUri;
            LogoUri = logoUri;
            RequireConsent = requireConsent;
            AlwaysIncludeUserClaimsInIdToken = alwaysIncludeUserClaimsInIdToken;
            AllowAccessTokensViaBrowser = allowAccessTokensViaBrowser;
            RedirectUris = redirectUris;
            AllowedScopes = allowedScopes;
            AllowedCorsOrigins = allowedCorsOrigins;
            IdentityTokenLifetime = identityTokenLifetime;
            if (ResourceOwnerEnabled)
            {
                AllowedGrantTypes = AllowedGrantTypes.Concat(IModels.GrantTypes.ResourceOwnerPassword).Distinct().ToList();
            } 
            else
            {
               AllowedGrantTypes.RemoveAll(a => IModels.GrantTypes.ResourceOwnerPassword.Any(r => r == a));
            }

        }


        public void Disable()
        {
            Enabled = false;
        }

        public void Enable()
        {
            Enabled = true;
        }



        public void GenerateNewClientId()
        {
            var secret = ClientIdGenerator.Generate(64);
            ClientSecrets = new List<ClientSecret> {
                new ClientSecret(IModels.HashExtensions.Sha256(secret), secret)
            };
        }


        public static class Factory
        {
            public static async Task<Client> CreateNewEntry(
                IClientRepository repository,
                string clientName,
                int clientTypeId
            )
            {

                if (string.IsNullOrWhiteSpace(clientName))
                    throw new ArgumentException("The ClientName must be specified", nameof(clientName));


                if (!await repository.HasUniqName(clientName))
                    throw new ArgumentException("An other tenant has the same name.", nameof(clientName));

                var client = new Client
                {
                    Id = Guid.NewGuid(),
                    ClientName = clientName,
                    ClientTypeId = clientTypeId,
                    ClientId = ClientIdGenerator.Generate(32),
                    AllowedScopes = new List<string> {
                        IdentityServerConstants.StandardScopes.OpenId
                    }
                };


                if (clientTypeId == ClientType.SinglePage.Id)
                {
                    client.RequireConsent = false;
                    client.AllowedGrantTypes = IModels.GrantTypes.Implicit.ToList();
                    client.AccessTokenType = (int)IModels.AccessTokenType.Jwt;
                    client.AllowAccessTokensViaBrowser = true;
                    client.AlwaysIncludeUserClaimsInIdToken = true;
                }
                else if (clientTypeId == ClientType.MachineToMachine.Id)
                {
                    client.RequireConsent = false;
                    client.AllowedGrantTypes = IModels.GrantTypes.ClientCredentials.ToList();
                    client.GenerateNewClientId();
                    client.RequireClientSecret = true;
                }

                return client;
            }
        }
    }
}