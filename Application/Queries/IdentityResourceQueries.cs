using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
 
using System.Linq.Expressions;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.RessourceAggregate;

namespace AuthorizationServer.Application.Queries
{
    public class IdentityResourceQueries
    {
        private readonly IIdentityResourceRepository _identityResourceRepository;

        public IdentityResourceQueries(IIdentityResourceRepository identityResourceRepository)
        {
            _identityResourceRepository = identityResourceRepository;
        }


        public async Task<List<IdentityResource>> GetAllIdentityScopesAsync()
        {
            var response = await _identityResourceRepository.List();
            return response;
        }
    }

}
