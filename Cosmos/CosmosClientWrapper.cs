using System;
using System.Threading.Tasks;
using CosmosDbExperiments.Config;
using Microsoft.Azure.Cosmos;

namespace CosmosDbExperiments.Cosmos
{
    public class CosmosClientWrapper
    {
        public CosmosClient Client { get; private set; }
        public Database Database { get; private set; }
        public Container Container { get; private set; }
        private readonly string DatabaseId;
        private readonly string ContainerId;
        private readonly string PartitionKey;

        public CosmosClientWrapper(CosmosContainerConfiguration configuration)
        {
            this.Client = new CosmosClient(configuration.Endpoint, configuration.PrimaryKey,
            new CosmosClientOptions()
            {
                SerializerOptions = new CosmosSerializationOptions()
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            });
            this.DatabaseId = configuration.DatabaseId;
            this.ContainerId = configuration.ContainerId;
            this.PartitionKey = configuration.PartitionKey;
        }

        public override string ToString()
        {
            return $"Cosmos client for Database: {DatabaseId}, Container: {ContainerId}";
        }

        public async Task InitializeDatabaseAsync()
        {
            this.Database = await this.Client.CreateDatabaseIfNotExistsAsync(this.DatabaseId);
        }

        public async Task InitializeContainerAsync()
        {
            try
            {
                var currentContainer = this.Database.GetContainer(this.ContainerId);
                await currentContainer.DeleteContainerAsync();
            }
            catch(CosmosException) { }

            this.Container = await this.Database.CreateContainerIfNotExistsAsync(this.ContainerId, this.PartitionKey);
        }

        public async Task<double> InsertRecordIntoContainer<T>(T record, string pk)
        {
            try
            {
                var response = await this.Container.CreateItemAsync<T>(record, new PartitionKey(pk));
                return response.RequestCharge;
            }
            catch(CosmosException e)
            {
                Console.WriteLine(e.Message);
            }
            return 0;
        }

        public async Task DeleteDbAndCleanupAsync()
        {
            await this.Database.DeleteAsync();

            this.Client.Dispose();
        }
    }
}