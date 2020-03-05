using System;
using System.Linq;
using IModels = IdentityServer4.Models;

namespace AuthorizationServer.Domain.Service
{
    public class ClientIdGenerator
    {
        public ClientIdGenerator()
        {
        }

        private static Random random = new Random();
        public static string Generate(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var randstr = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return randstr;
        }
    }
}
