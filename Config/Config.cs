namespace CosmosDbExperiments.Config
{
    public class Config
    {
        public CosmosContainerConfiguration NearestRegionSettings { get; set; }
        public CosmosContainerConfiguration AwayRegionSettings { get; set; }
    }
}