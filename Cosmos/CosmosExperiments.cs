using CosmosDbExperiments.Cosmos;
using System;
using CosmosDbExperiments.Config;
using System.Threading.Tasks;
using CosmosDbExperiments.Tools;
using CosmosDbExperiments.Models;
using System.Diagnostics;

public class CosmosExperiments
{
    private const int NumberOfRecordsForSimulation = 300;
    private CosmosClientWrapper ClientForNearbyRegion;
    private CosmosClientWrapper ClientForAwayRegion;
    public CosmosExperiments(Config config)
    {
        ClientForNearbyRegion = new CosmosClientWrapper(config.NearestRegionSettings);
        ClientForAwayRegion = new CosmosClientWrapper(config.AwayRegionSettings);
        Console.WriteLine("Client objects created!!!");
    }

    public async Task InitializeDatabasesWithDataAsync()
    {
        //nearby region
        await ClientForNearbyRegion.InitializeDatabaseAsync();
        await ClientForNearbyRegion.InitializeContainerAsync();

        await DumpMoviesToContainerAsync(ClientForNearbyRegion);
    }

    public async Task DumpMoviesToContainerAsync(CosmosClientWrapper wrapper)
    {
        Console.WriteLine($"Dumping data into Cosmos!!!!\n {wrapper.ToString()}");
        var totalRUs = 0.0;
        var timer = new Stopwatch();
        timer.Start();
        foreach(Movie movie in MovieParser.ParseMovies(NumberOfRecordsForSimulation))
        {
            var currentRU = await wrapper.InsertRecordIntoContainer<Movie>(movie, movie.YearPk);
            Console.WriteLine($"Written a doc with RU = {string.Format("{0:0.00}", currentRU)}");
            totalRUs = totalRUs + currentRU;
        }
        timer.Stop();
        var totalTimeTaken = timer.Elapsed;
        Console.WriteLine($"\n\nTotal time taken to write movies = {totalTimeTaken.ToString(@"m\:ss\.fff")}");
        Console.WriteLine($"\n\nTotal Request Units taken to write {NumberOfRecordsForSimulation} documents = {string.Format("{0:0.00}", totalRUs)}");
    }

}