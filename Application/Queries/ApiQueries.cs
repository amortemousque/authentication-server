using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
 
using System.Linq.Expressions;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.ApiAggregate;
using AuthorizationServer.Exceptions;

namespace AuthorizationServer.Application.Queries
{
    public class ApiQueries
    {
        private readonly IApiResourceRepository _apiResourceRepository;

        public ApiQueries(IApiResourceRepository apiResourceRepository)
        {
            _apiResourceRepository = apiResourceRepository;
        }


        public async Task<ApiResource> GetApiAsync(Guid apiId)
        {
            var response = await _apiResourceRepository.GetById(apiId);

            if (response == null)
                throw new NotFoundException();

            return response;
        }

        public async Task<List<ApiResource>> GetApisAsync(string name, string displayName, bool? enabled)
        {
            
            var response = await _apiResourceRepository.GetAll();
                                                          
            if(!string.IsNullOrWhiteSpace(name)) {
                response = response.Where(a => a.Name.StartsWith(name));
            }

            if(!string.IsNullOrWhiteSpace(displayName)) {
                response = response.Where(a => a.DisplayName.StartsWith(displayName));
            }

            if (enabled != null)
            {
                response = response.Where(a => a.Enabled == enabled);
            }


            return response.ToList();
        }


        public async Task<List<ApiScope>> GetApiScopesAsync(Guid apiResourceId)
        {
            var response = await _apiResourceRepository.GetById(apiResourceId);
            return response.Scopes.ToList();
        }
            
        public async Task<List<ApiScope>> GetAllApiScopesAsync()
        {
            var response = await _apiResourceRepository.GetAll();
            var scopes = response.SelectMany(a => a.Scopes).ToList();
            return scopes;
        }
    }

}
    