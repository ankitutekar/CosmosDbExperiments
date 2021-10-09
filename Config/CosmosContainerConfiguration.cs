namespace CosmosDbExperiments.Config
{
    public class CosmosContainerConfiguration
    {
        public string Endpoint { get; set; }
        public string PrimaryKey { get; set; }
        public string DatabaseId { get; set; }
        public string ContainerId { get; set; }
        public string PartitionKey { get; set; }
    }
}