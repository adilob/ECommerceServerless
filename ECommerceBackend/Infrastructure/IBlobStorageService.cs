using System.Threading.Tasks;

namespace ECommerceBackend.Infrastructure
{
    public interface IBlobStorageService
    {
        Task<string> UploadBlobAsync(byte[] blob, string blobName, string contentType);
        Task<byte[]> DownloadBlobAsync(string blobName);
        Task DeleteBlobAsync(string blobName);
        Task<string> GetBlobUriAsync(string blobName);
    }
}
