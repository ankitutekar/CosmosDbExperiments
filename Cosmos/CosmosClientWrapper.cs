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
            this.Client = new CosmosClient(configuration.Endpoint, configuration.PrimaryKey);
            this.DatabaseId = configuration.DatabaseId;
            this.ContainerId = configuration.ContainerId;
            this.PartitionKey = configuration.PartitionKey;
        }

        public async Task InitializeDatabaseAsync()
        {
            this.Database = await this.Client.CreateDatabaseIfNotExistsAsync(this.DatabaseId);
        }

        public async Task InitializeContainerAsync()
        {
            this.Container = await this.Database.CreateContainerIfNotExistsAsync(this.ContainerId, this.PartitionKey);
        }
    }
}