using CosmosDbExperiments.Cosmos;
using System;
using CosmosDbExperiments.Config;
using System.Threading.Tasks;
using CosmosDbExperiments.Tools;
using CosmosDbExperiments.Models;
using System.Diagnostics;

public class CosmosExperiments
{
    private readonly int NumberOfRecordsForSimulation;
    private CosmosClientWrapper CosmosClientWrapper;
    public CosmosExperiments(Config config)
    {
        CosmosClientWrapper = new CosmosClientWrapper(config.CosmosAccountSettings);
        NumberOfRecordsForSimulation = config.NumberOfRecordsForSimulation;
        Console.WriteLine("Client objects created!!!");
    }

    public async Task InitializeDatabasesWithDataAsync()
    {
        await CosmosClientWrapper.InitializeDatabaseAsync();
        await CosmosClientWrapper.InitializeContainerAsync(shouldDeleteExisting: false);

       //  await DumpMoviesToContainerAsync();
        
        //await ReadMoviesWithReplicationToNearestRegionEnabledAndPkSpecified();
        await ReadMoviesWithReplicationToNearestRegionEnabledButPkNotSpecified();

       // await ReadMoviesWithReplicationToNearestRegionDisabledAndPkSpecified();
        //await ReadMoviesWithReplicationToNearestRegionDisabledAndPkNotSpecified();
    }

    public async Task DumpMoviesToContainerAsync()
    {
        Console.WriteLine($"Dumping data into Cosmos!!!!\n {CosmosClientWrapper.ToString()}");
        var totalRUs = 0.0;
        var timer = new Stopwatch();
        timer.Start();
        foreach(Movie movie in MovieParser.ParseMovies(NumberOfRecordsForSimulation))
        {
            var currentRU = await CosmosClientWrapper.InsertRecordIntoContainer<Movie>(movie, movie.YearPk);
            Console.WriteLine($"Written a doc with RU = {string.Format("{0:0.00}", currentRU)}");
            totalRUs = totalRUs + currentRU;
        }
        timer.Stop();
        var totalTimeTaken = timer.Elapsed;
        Console.WriteLine($"\n\nTotal time taken to write movies = {totalTimeTaken.ToString(@"m\:ss\.fff")}");
        Console.WriteLine($"\n\nTotal Request Units taken to write {NumberOfRecordsForSimulation} documents = {string.Format("{0:0.00}", totalRUs)}");
    }

    public async Task ReadMoviesWithReplicationToNearestRegionEnabledAndPkSpecified()
    {
        Console.WriteLine($"Reading data from Cosmos with replication to nearest region enabled!!");
        Console.WriteLine($"Partition key is also specified!!");
        var queryText = "SELECT * FROM c Where c.info.rating > 6.1";
        var totalRUs = 0.0;
        int currentYear = 1991;
        var timer = new Stopwatch();
        timer.Start();
        for(int i = 0; i < NumberOfRecordsForSimulation; i++)
        {
            var currentRU = await CosmosClientWrapper.QueryItemsWithGivenPartitionKey<Movie>(
                currentYear.ToString(),
                queryText
            );
            Console.WriteLine($"Queried container with RU = {string.Format("{0:0.00}", currentRU)}");
            if(currentYear >= 2013) {
                currentYear = 1991;
            }
            else {
                currentYear++;
            }
            totalRUs = totalRUs + currentRU;
        }

        timer.Stop();
        var totalTimeTaken = timer.Elapsed;
        Console.WriteLine($"\n\nTotal time taken to query movies = {totalTimeTaken.ToString(@"m\:ss\.fff")}");
        Console.WriteLine($"\n\nTotal Request Units taken to query {NumberOfRecordsForSimulation} times = {string.Format("{0:0.00}", totalRUs)}");
    }

    public async Task ReadMoviesWithReplicationToNearestRegionDisabledAndPkSpecified()
    {
        Console.WriteLine($"Reading data from Cosmos with replication to nearest region disabled!!");
        Console.WriteLine($"Partition key is specified!!");
        var queryText = "SELECT * FROM c Where c.info.rating > 6.1";
        var totalRUs = 0.0;
        int currentYear = 1991;
        var timer = new Stopwatch();
        timer.Start();
        for (int i = 0; i < NumberOfRecordsForSimulation; i++)
        {
            var currentRU = await CosmosClientWrapper.QueryItemsWithGivenPartitionKey<Movie>(
                currentYear.ToString(),
                queryText
            );
            Console.WriteLine($"Queried container with RU = {string.Format("{0:0.00}", currentRU)}");
            if (currentYear >= 2013)
            {
                currentYear = 1991;
            }
            else
            {
                currentYear++;
            }
            totalRUs = totalRUs + currentRU;
        }

        timer.Stop();
        var totalTimeTaken = timer.Elapsed;
        Console.WriteLine($"\n\nTotal time taken to query movies = {totalTimeTaken.ToString(@"m\:ss\.fff")}");
        Console.WriteLine($"\n\nTotal Request Units taken to query {NumberOfRecordsForSimulation} times = {string.Format("{0:0.00}", totalRUs)}");
    }

    public async Task ReadMoviesWithReplicationToNearestRegionDisabledAndPkNotSpecified()
    {
        Console.WriteLine($"Reading data from Cosmos with replication to nearest region disabled!!");
        Console.WriteLine($"Partition key is not specified!!");
        var queryText = "SELECT * FROM c Where c.info.rating > 6.1";
        var totalRUs = 0.0;
        var timer = new Stopwatch();
        timer.Start();
        for (int i = 0; i < NumberOfRecordsForSimulation; i++)
        {
            var currentRU = await CosmosClientWrapper.QueryItemsWithoutPartitionKey<Movie>(
                queryText
            );
            Console.WriteLine($"Queried container with RU = {string.Format("{0:0.00}", currentRU)}");
            totalRUs = totalRUs + currentRU;
        }

        timer.Stop();
        var totalTimeTaken = timer.Elapsed;
        Console.WriteLine($"\n\nTotal time taken to query movies = {totalTimeTaken.ToString(@"m\:ss\.fff")}");
        Console.WriteLine($"\n\nTotal Request Units taken to query {NumberOfRecordsForSimulation} times = {string.Format("{0:0.00}", totalRUs)}");
    }

    public async Task ReadMoviesWithReplicationToNearestRegionEnabledButPkNotSpecified()
    {
        Console.WriteLine($"Reading data from Cosmos with replication to nearest region Enabled!!");
        Console.WriteLine($"Partition key is not specified!!");
        var queryText = "SELECT * FROM c Where c.info.rating > 6.1";
        var totalRUs = 0.0;
        var timer = new Stopwatch();
        timer.Start();
        for (int i = 0; i < NumberOfRecordsForSimulation; i++)
        {
            var currentRU = await CosmosClientWrapper.QueryItemsWithoutPartitionKey<Movie>(
                queryText
            );
            Console.WriteLine($"Queried container with RU = {string.Format("{0:0.00}", currentRU)}");
            totalRUs = totalRUs + currentRU;
        }

        timer.Stop();
        var totalTimeTaken = timer.Elapsed;
        Console.WriteLine($"\n\nTotal time taken to query movies = {totalTimeTaken.ToString(@"m\:ss\.fff")}");
        Console.WriteLine($"\n\nTotal Request Units taken to query {NumberOfRecordsForSimulation} times = {string.Format("{0:0.00}", totalRUs)}");
    }

}