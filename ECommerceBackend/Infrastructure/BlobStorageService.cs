using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Threading.Tasks;

namespace ECommerceBackend.Infrastructure
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly BlobContainerClient _blobContainerClient;

        public BlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
            _blobContainerClient = _blobServiceClient.GetBlobContainerClient("product-images");
        }

        public async Task DeleteBlobAsync(string blobName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);

            if (blobClient.Exists())
            {
                await blobClient.DeleteAsync();
            }
        }

        public async Task<byte[]> DownloadBlobAsync(string blobName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);

            if (!blobClient.Exists())
            {
                return null;
            }

            var downloadInfo = await blobClient.DownloadAsync();

            using (var stream = new System.IO.MemoryStream())
            {
                await downloadInfo.Value.Content.CopyToAsync(stream);
                return stream.ToArray();
            }
        }

        public async Task<string> GetBlobUriAsync(string blobName)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);

            if (!blobClient.Exists())
            {
                return await Task.FromResult<string>(null);
            }

            return await Task.FromResult(blobClient.Uri.ToString());
        }

        public async Task<string> UploadBlobAsync(byte[] blob, string blobName, string contentType)
        {
            if (blob == null || blob.Length == 0)
            {
                return null;
            }

            var blobClient = _blobContainerClient.GetBlobClient(blobName);

            using (var stream = new System.IO.MemoryStream(blob))
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });
            }

            return blobClient.Uri.ToString();
        }
    }
}
