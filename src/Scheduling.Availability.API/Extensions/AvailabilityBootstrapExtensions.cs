using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using Scheduling.Availability.API.Models;
using Scheduling.Core.Utils;
using Scheduling.Domain.Entities;

namespace Scheduling.Availability.API.Extensions;

public static class AvailabilityBootstrapExtensions
{
    private const string PRODUCT_GROUP_CLAIM_NAME = @"ds.product-group";
    // private const string PATH_TO_CA_FILE = "certs/rds-combined-ca-bundle.pem";

    public static void AddRepositoryServices(this IServiceCollection services)
    {
        Console.WriteLine($"enter -> {nameof(AddRepositoryServices)}");

        // Setup database client
        services.AddSingleton<IMongoClient>(provider =>
        {
            var logger = provider.GetService<ILogger<Program>>();
            logger.LogDebug($"enter -> services.AddSingleton<IMongoClient>");

            try
            {
                logger.LogDebug("acquiring databaseConnectionSettings");
                var databaseConnectionSettings = provider.GetService<IOptions<AvailabilityDatabaseSettings>>()?.Value;

                // ADD CA certificate to local trust store
                // TODO: This AWS documentation provided code snippet _does not work_
                //       https://docs.aws.amazon.com/documentdb/latest/developerguide/connect_programmatically.html#connect_programmatically-tls_enabled
                //       This might be a consequence of the driver version the project uses, I have not investigated.
                //       The cert is found, imported properly and added to the trust store successfully.
                //       In the meantime we are using AllowInsecureTls flag which eliminates the trust TLS offers.
                // X509Store localTrustStore = new X509Store(StoreName.Root);
                // X509Certificate2Collection certificateCollection = new X509Certificate2Collection();
                // certificateCollection.Import(PATH_TO_CA_FILE);
                // try
                // {
                //     localTrustStore.Open(OpenFlags.ReadWrite);
                //     localTrustStore.AddRange(certificateCollection);
                // }
                // catch (Exception ex)
                // {
                //     logger.LogError("Root certificate import failed: " + ex.Message, ex);
                //     throw;
                // }
                // finally
                // {
                //     localTrustStore.Close();
                // }

                logger.LogDebug("acquiring connectionString");
                var connectionString = databaseConnectionSettings?.ConnectionString ?? "";
                var obscuredConnectionString = Regex.Replace(connectionString, "(mongodb://[\\w_]+\\:)(.*?)@(.*?)", $"$1***HIDDEN***@");
                logger.LogDebug($"connectionString: {obscuredConnectionString}");
                logger.LogDebug($"initializing MongoClientSettings from: {obscuredConnectionString}");
                var settings = MongoClientSettings.FromConnectionString(connectionString);

                logger.LogDebug("MongoClientSettings initialized");

                logger.LogDebug("Applying additional settings -- AllowInsecureTls, ClusterConfigurator (query logging)");

                // settings.SslSettings.ClientCertificates = certificateCollection;
                settings.AllowInsecureTls = true; // TEMPORARY
                settings.ClusterConfigurator = cb => {
                    cb.Subscribe<CommandStartedEvent>(e => {
                        logger.LogDebug($"{e.CommandName} - {e.Command.ToJson()}");
                    });
                };

                logger.LogDebug("Additional settings applied -- AllowInsecureTls, ClusterConfigurator (query logging)");

                logger.LogDebug("initializing MongoClient");

                var client = new MongoClient(settings);
                logger.LogDebug("Client initialized");

                return client;
            }
            catch (Exception exception)
            {
                var appException = new ApplicationException("Unable to initialize MongoClient", exception);
                logger.LogError(appException.Message, appException);
                throw appException;
            }
        });

        // Setup collection resolver
        services.AddScoped<IMongoCollection<AssignmentEntity>>(provider =>
        {
            var logger = provider.GetService<ILogger<Program>>();
            logger.LogDebug($"enter -> services.AddScoped<IMongoCollection<Models.Reservation>>");

            var context = provider.GetService<IHttpContextAccessor>()?.HttpContext;
            logger.LogDebug($"retrieved context");
            
            var productGroup = context?.User.FindFirst(PRODUCT_GROUP_CLAIM_NAME)?.Value ?? "test";
            logger.LogDebug($"productGroup: {productGroup}");
            
            var databaseConnectionSettings = provider.GetService<IOptions<AvailabilityDatabaseSettings>>()?.Value;
            logger.LogDebug($"retrieved databaseConnectionSettings");
            
            var collectionName = $"{databaseConnectionSettings?.CollectionName}_{productGroup}";
            logger.LogDebug($"collectionName: {collectionName}");
            var mongoDatabase = provider.GetService<IMongoClient>()?.GetDatabase(databaseConnectionSettings?.DatabaseName);
            
            logger.LogDebug($"acquired database");
            var exists = mongoDatabase?.ListCollectionNames().ToList().Any(name => name == collectionName) ?? false;
            logger.LogDebug($"{collectionName} exists? {exists}");
            
            var collection = (exists 
                                 ? mongoDatabase?.GetCollection<AssignmentEntity>(collectionName) 
                                 : CollectionManager.CreateAvailabilityCollection(mongoDatabase, collectionName)) ??
                             throw new InvalidOperationException("Where my database collection?! :sadface:");

            logger.LogDebug($"returning collection {collectionName}");
            
            return collection;
        });
    }
}
