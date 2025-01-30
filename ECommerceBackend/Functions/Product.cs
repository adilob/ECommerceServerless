using ECommerceBackend.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using ECommerceBackend.Infrastructure;

namespace ECommerceBackend.Functions
{
    public class Product
    {
        private readonly IRepository<Models.Product> _productRepository;
        private readonly IBlobStorageService _blobStorageService;

        public Product(IRepository<Models.Product> repository, IBlobStorageService blobStorageService)
        {
            _productRepository = repository;
            _blobStorageService = blobStorageService;
        }

        [FunctionName("AddProduct")]
        public async Task<IActionResult> AddProduct(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing request to add a new product.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var product = JsonConvert.DeserializeObject<Models.Product>(requestBody);

            await _productRepository.AddAsync(product);
            log.LogInformation($"Product added: {product?.Name}");

            return new OkObjectResult(product);
        }

        [FunctionName("GetProducts")]
        public async Task<IActionResult> GetProducts(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Processing request to add a new product.");
            var products = await _productRepository.GetAllAsync();
            log.LogInformation($"Products retrieved: {products?.Count()}");

            return new OkObjectResult(products);
        }

        [FunctionName("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "products/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation($"Processing request to update product with ID: {id}");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var product = JsonConvert.DeserializeObject<Models.Product>(requestBody);

            var dbProduct = await _productRepository.GetByIdAsync(id);
            if (dbProduct == null)
            {
                return new BadRequestObjectResult($"Product with ID: {id} does not exist!");
            }

            await _productRepository.UpdateAsync(id, product);
            log.LogInformation($"Product updated: {product?.Name}");

            return new NoContentResult();
        }

        [FunctionName("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "products/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            log.LogInformation($"Processing request to delete product with ID: {id}");

            var dbProduct = await _productRepository.GetByIdAsync(id);
            if (dbProduct == null)
            {
                return new BadRequestObjectResult($"Product with ID: {id} does not exist!");
            }

            await _productRepository.DeleteAsync(id);
            return new NoContentResult();
        }

        [FunctionName("UploadProductImage")]
        public async Task<IActionResult> UploadProductImage(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "products/{productId}/upload-image")] HttpRequest req,
            ILogger log, string productId)
        {
            log.LogInformation($"Processing request to upload image for product with ID: {productId}");
            var dbProduct = await _productRepository.GetByIdAsync(productId);

            if (dbProduct == null)
            {
                return new BadRequestObjectResult($"Product with ID: {productId} does not exist!");
            }

            var formFile = req.Form.Files["file"];
            if (formFile == null)
            {
                return new BadRequestObjectResult("No file was uploaded!");
            }

            var blobName = $"{productId}-{formFile.FileName}";
            var fileBytes = new byte[formFile.Length];

            using (var stream = formFile.OpenReadStream())
            {
                stream.Read(fileBytes, 0, (int)formFile.Length);
            }

            var blobUri = await _blobStorageService.UploadBlobAsync(fileBytes, blobName, formFile.ContentType);
            log.LogInformation($"Image uploaded for product with ID: {productId}");

            return new OkObjectResult(blobUri);
        }

        [FunctionName("GetProductImage")]
        public async Task<IActionResult> GetProductImage(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "products/{productId}/image/{fileName}")] HttpRequest req,
            ILogger log, string productId, string fileName)
        {
            log.LogInformation($"Processing request to get image for product with ID: {productId}");
            var dbProduct = await _productRepository.GetByIdAsync(productId);

            if (dbProduct == null)
            {
                return new BadRequestObjectResult($"Product with ID: {productId} does not exist!");
            }

            var blobName = $"{productId}-{fileName}";
            var blobUri = await _blobStorageService.GetBlobUriAsync(blobName);

            if (blobUri == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(new { ImageUrl = blobUri });
        }

        [FunctionName("DeleteProductImage")]
        public async Task<IActionResult> DeleteProductImage(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "products/{productId}/image/{fileName}")] HttpRequest req,
            ILogger log, string productId, string fileName)
        {
            log.LogInformation($"Processing request to delete image for product with ID: {productId}");
            var dbProduct = await _productRepository.GetByIdAsync(productId);

            if (dbProduct == null)
            {
                return new BadRequestObjectResult($"Product with ID: {productId} does not exist!");
            }

            var blobName = $"{productId}-{fileName}";
            await _blobStorageService.DeleteBlobAsync(blobName);

            log.LogInformation($"Image deleted for product with ID: {productId}");
            return new NoContentResult();
        }
    }
}
