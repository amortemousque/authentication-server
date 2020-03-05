using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

namespace AuthorizationServer.Infrastructure.Storage
{
    public interface IStorageManager
    {

        Task UploadBlob(string source, string destination, string contentType, int accessType);

        Task<BlockBlob> DownloadBlob(string location, int accessType);

        Task<int> DeleteBlob(string location, int accessType);

        Task CreateStorageContainers(string containerName);

    }
}
