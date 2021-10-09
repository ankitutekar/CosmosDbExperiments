using CosmosDbExperiments.Cosmos;
using System;
using CosmosDbExperiments.Config;

public class CosmosExperiments
{
    private CosmosClientWrapper ClientForNearbyRegion;
    private CosmosClientWrapper ClientForAwayRegion;
    public CosmosExperiments(Config config)
    {
        ClientForNearbyRegion = new CosmosClientWrapper(config.NearestRegionSettings);
        ClientForAwayRegion = new CosmosClientWrapper(config.AwayRegionSettings);
        Console.WriteLine("Client objects created!!!");
    }
}