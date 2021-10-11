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
                },
                ApplicationRegion = configuration.Region
            });
            this.DatabaseId = configuration.DatabaseId;
            this.ContainerId = configuration.ContainerId;
            this.PartitionKey = configuration.PartitionKey;
        }

        public override string ToString()
        {
            return $"Cosmos client for Database: {DatabaseId}, Container: {ContainerId}";
        }

        public async Task EnsureDatabaseAsync()
        {
            this.Database = await this.Client.CreateDatabaseIfNotExistsAsync(this.DatabaseId);
        }

        public async Task InitializeContainerAsync(bool shouldDeleteExisting)
        {
            if(shouldDeleteExisting)
            {
                try
                {
                    var currentContainer = this.Database.GetContainer(this.ContainerId);
                    await currentContainer.DeleteContainerAsync();
                }
                catch (CosmosException) { }
            }

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

        public async Task<double> ReadItemWithGivenPartitionKeyAndId<T>(string pk, string id)
        {
            var response = await this.Container.ReadItemAsync<T>(id, new PartitionKey(pk));

            return response.RequestCharge;
        }

        public async Task<double> QueryItemsWithGivenPartitionKey<T>(string pk, string queryText)
        {
            //reading only first 10 records
            var iterator = this.Container.GetItemQueryIterator<T>(
                      queryText: queryText,
                      requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(pk),
                                                                MaxItemCount = 10 }
                    );
            var response = await iterator.ReadNextAsync();
            Console.WriteLine($"Queried container {this.ContainerId} with partition key {pk}, {response.Count} returned.");
            return response.RequestCharge;
        }

        public async Task<double> QueryItemsWithoutPartitionKey<T>(string queryText)
        {
            var iterator = this.Container.GetItemQueryIterator<T>(
                      queryText: queryText,
                      requestOptions: new QueryRequestOptions
                      {
                          MaxItemCount = 10
                      }
                    );
            var response = await iterator.ReadNextAsync();

            return response.RequestCharge;
        }

        public async Task CleanupContainerAsync()
        {
            await this.Container.DeleteContainerAsync();
        }

        public async Task CleanupAsync()
        {
            await CleanupContainerAsync();
            this.Client.Dispose();
        }
    }
}