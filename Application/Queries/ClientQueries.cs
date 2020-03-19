using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
 
using System.Linq.Expressions;
using AuthorizationServer.Domain.Contracts;
using AuthorizationServer.Domain.ClientAggregate;
using AuthorizationServer.Exceptions;

namespace AuthorizationServer.Application.Queries
{
    public class ClientQueries
    {
        private readonly IClientRepository _clientRepository;

        public ClientQueries(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }


        public async Task<Client> GetClientAsync(Guid id)
        {
            var response = await _clientRepository.GetById(id);

            if (response == null)
                throw new NotFoundException();

            return response;
        }

        public async Task<List<Client>> GetClientsAsync(string clientName, bool? enabled)
        {
            
                var response = await _clientRepository.GetAll();
                                                          
            if(!string.IsNullOrWhiteSpace(clientName)) {
                response = response.Where(a => a.ClientName.StartsWith(clientName));
            }

            if (enabled != null)
            {
                response = response.Where(a => a.Enabled == enabled);
            }


            return response.ToList();
        }
            
    }

}
        