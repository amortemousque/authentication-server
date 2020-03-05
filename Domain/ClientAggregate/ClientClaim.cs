using System;
namespace AuthorizationServer.Domain.ClientAggregate
{
    public class ClientClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public ClientClaim()
        {
           
        }
    }
}
