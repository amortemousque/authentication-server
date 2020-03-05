using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediatR;
using Newtonsoft.Json;

namespace AuthorizationServer.Application.Commands
{
    public class UpdateClientCommand : IRequest<bool>
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public bool Enabled { get; set; } = true;

        public string ClientName { get; set; }

        public string Description { get; set; }

        public string ClientUri { get; set; }

        public string LogoUri { get; set; }

        public bool RequireClientSecret { get; set; }

        public bool RequireConsent { get; set; }

        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

        public bool AllowAccessTokensViaBrowser { get; set; }

        public int IdentityTokenLifetime { get; set; }

        public List<string> RedirectUris { get; set; }

        public List<string> AllowedCorsOrigins { get; set; }

        public List<string> AllowedScopes { get; set; }

        public bool ResourceOwnerEnabled { get; set; }

}
}
