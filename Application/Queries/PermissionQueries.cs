using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
 
using System.Linq.Expressions;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.PermissionAggregate;

namespace AuthorizationServer.Application.Queries
{
    public class PermissionQueries
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionQueries(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }


        public async Task<Permission> GetPermissionAsync(Guid id)
        {
            var response = await _permissionRepository.GetById(id);

            if (response == null)
                throw new KeyNotFoundException();

            return response;
        }

        public async Task<List<Permission>> GetPermissionsAsync(string name)
        {

            var response = await _permissionRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(name))
            {
                response = response.Where(a => a.Name.StartsWith(name));
            }


            return response.ToList();
        }

    }

}
