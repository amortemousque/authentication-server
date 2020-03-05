using System;
using System.IO;

namespace AuthorizationServer.Infrastructure.Storage
{
    public class BlockBlob
    {
        public MemoryStream Stream { get; set; }

        public string ContentType { get; set; }
    }
}
