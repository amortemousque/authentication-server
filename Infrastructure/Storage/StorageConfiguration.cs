using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace AuthorizationServer.Infrastructure.Storage
{
    public class StorageConfiguration
    {
        public string PublicConnectionString { get; set; }
        public string PrivateConnectionString { get; set; }

        public StorageConfiguration(string publicConnectionString, string privateConnectionString) {
            PublicConnectionString = publicConnectionString;
            PrivateConnectionString = privateConnectionString;
        }
    }
}
