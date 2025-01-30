using Azure;
using Azure.Messaging.EventGrid;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using System;

[assembly: FunctionsStartup(typeof(ECommerceBackend.Startup))]
namespace ECommerceBackend
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var cosmosDbConnectionString = Environment.GetEnvironmentVariable("CosmosDBConnectionString");
            var blobStorageConnectionString = Environment.GetEnvironmentVariable("BlobStorageConnectionString");

            var eventGridTopicEndpoint = Environment.GetEnvironmentVariable("EventGridTopicEndpoint");
            var eventGridKey = Environment.GetEnvironmentVariable("EventGridTopicKey");

            //var b2cAuthority = Environment.GetEnvironmentVariable("B2C_Authority");
            //var clientId = Environment.GetEnvironmentVariable("B2C_ClientId");

            //if (string.IsNullOrEmpty(b2cAuthority) || string.IsNullOrEmpty(clientId))
            //{
            //    throw new InvalidOperationException("B2C_Authority and B2C_ClientId must be set in configuration.");
            //}

            //builder.Services
            //    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //    .AddJwtBearer(options =>
            //    {
            //        options.Authority = b2cAuthority;
            //        options.Audience = clientId;
            //        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            //        {
            //            ValidateIssuer = true,
            //            ValidIssuer = b2cAuthority,
            //            ValidateAudience = true,
            //            ValidAudience = clientId,
            //            ValidateLifetime = true
            //        };
            //    });

            //builder.Services.AddAuthorization();

            var cosmosClient = new CosmosClient(cosmosDbConnectionString);
            var blobServiceClient = new BlobServiceClient(blobStorageConnectionString);
            var eventGridPublisherClient = new EventGridPublisherClient(new Uri(eventGridTopicEndpoint), new AzureKeyCredential(eventGridKey));

            builder.Services.AddSingleton(cosmosClient);
            builder.Services.AddSingleton(blobServiceClient);
            builder.Services.AddSingleton(eventGridPublisherClient);

            builder.Services.AddTransient<Repositories.IRepository<Models.Product>, Repositories.ProductRepository>();
            builder.Services.AddTransient<Repositories.IRepository<Models.Order>, Repositories.OrderRepository>();
            builder.Services.AddTransient<Repositories.IRepository<Models.User>, Repositories.UserRepository>();
            builder.Services.AddTransient<Infrastructure.IBlobStorageService, Infrastructure.BlobStorageService>();
            builder.Services.AddTransient<Infrastructure.IEventGridService, Infrastructure.EventGridService>();
        }
    }
}
