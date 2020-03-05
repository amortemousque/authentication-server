using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using AuthorizationServer.Infrastructure.Context;

namespace AuthorizationServer.Infrastructure.Storage
{
    public class AzureStorageManager : IStorageManager
    {

        private CloudBlobClient _publicClient;
        private CloudBlobClient _privateClient;

        private IApplicationContext _applicationContext;

        public AzureStorageManager(StorageConfiguration confirguration, IApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
            CloudStorageAccount accountPublic = CloudStorageAccount.Parse(confirguration.PublicConnectionString);
            CloudStorageAccount accountPrivate = CloudStorageAccount.Parse(confirguration.PrivateConnectionString);

            _publicClient = accountPublic.CreateCloudBlobClient();
            _privateClient = accountPrivate.CreateCloudBlobClient();
        }

        public async Task<int> DeleteBlob(string location, int accessType)
        {
            var containerName = _applicationContext.Tenant.Name;
            var container = accessType == (int)ContainerAccessType.Public ? _publicClient.GetContainerReference(containerName) : _privateClient.GetContainerReference(containerName);
            // Retrieve reference to a blob named "myblob.txt".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(location);

            // Delete the blob.
            await blockBlob.DeleteAsync();

            return 1;
        }

        public async Task<BlockBlob> DownloadBlob(string location, int accessType)
        {
            var containerName = _applicationContext.Tenant.Name;
            var container = accessType == (int)ContainerAccessType.Public ? _publicClient.GetContainerReference(containerName) : _privateClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(location);
            var bb = new BlockBlob();
            // Save the blob contents to a file named "myfile".
            using (var memoryStream = new MemoryStream())
            {
                await blockBlob.DownloadToStreamAsync(memoryStream);
                bb.Stream = memoryStream;
            }
            bb.ContentType = blockBlob.Properties.ContentType;

            return bb;
        }

        public async Task UploadBlob(string source, string destination, string contentType, int accessType)
        {
            var containerName = _applicationContext.Tenant.Name;
            destination = destination.TrimEnd(Path.DirectorySeparatorChar)
                                .TrimStart(Path.DirectorySeparatorChar);
            var container = accessType == (int)ContainerAccessType.Public ? _publicClient.GetContainerReference(containerName) : _privateClient.GetContainerReference(containerName);
            var newBlob = container.GetBlockBlobReference(destination);
            newBlob.Properties.ContentType = contentType;

            try
            {
                await newBlob.UploadFromFileAsync(source);

            }
            catch (Exception ex)
            {
                var test = ex;
            }
        }

        public async Task CreateStorageContainers(string containerName)
        {
             await _publicClient.GetContainerReference(containerName).CreateAsync();
             await _privateClient.GetContainerReference(containerName).CreateAsync();

            // Set the permissions so the blobs are public. 
            BlobContainerPermissions permissions = new BlobContainerPermissions
            {
                PublicAccess = BlobContainerPublicAccessType.Blob
            };
            await _publicClient.GetContainerReference(containerName).SetPermissionsAsync(permissions);
        }

    }
}
