using System;
using AuthorizationServer.Domain.SeedWork;

namespace AuthorizationServer.Domain
{
    public class ClientType : Enumeration
    {
        public static ClientType SinglePage = new ClientType(1, "Single page application");
        public static ClientType MachineToMachine = new ClientType(2, "Machine to machine application");


        protected ClientType() { }

        public ClientType(int id, string name)
            : base(id, name)
        {

        }
    }
}
    