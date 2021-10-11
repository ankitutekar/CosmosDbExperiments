using CosmosDbExperiments.Cosmos;
using System;
using CosmosDbExperiments.Config;
using System.Threading.Tasks;
using CosmosDbExperiments.Tools;
using CosmosDbExperiments.Models;
using System.Diagnostics;

public class CosmosExperiments
{
    private int NumberOfRecordsForSimulation;
    private int NumberOfIterations;
    private CosmosClientWrapper CosmosClientWrapper;
    public CosmosExperiments(Config config)
    {
        CosmosClientWrapper = new CosmosClientWrapper(config.CosmosAccountSettings);
        NumberOfRecordsForSimulation = config.NumberOfRecordsForSimulation;
        NumberOfIterations = config.NumberOfIterations;
        Console.WriteLine("Client is ready to communicate!!!");
    }

    public async Task RunExperimentsAsync()
    {
        bool quitFlag = false;
        while(!quitFlag)
        {
            var options = @"1. Initialize container with data
                        2. Run queries with PK
                        3. Run queries without PK
                        4. Update iterations count
                        5. Cleanup container
                        6. Quit";
            Console.WriteLine($"{options}\nWhat do you want to do? ");
            var choice = Int32.Parse(Console.ReadLine()?.Trim() ?? "0");
            switch (choice)
            {
                case 1:
                    await InitializeContainerWithDataAsync();
                    break;
                case 2:
                    await QueryMoviesWithPkSpecifiedAsync();
                    break;
                case 3:
                    await QueryMoviesWithoutPkSpecifiedAsync();
                    break;
                case 4:
                    UpdateIterationCount();
                    break;
                case 5:
                    await CleanupContainerAsync();
                    break;
                case 6:
                    quitFlag = true;
                    break;
                default:
                    continue;
            }
        }
    }

    private void UpdateIterationCount()
    {
        Console.WriteLine($"Enter new iteration count:");
        var newCount = Int32.Parse(Console.ReadLine()?.Trim() ?? NumberOfIterations.ToString());
        this.NumberOfIterations = newCount;
    }

    private async Task InitializeContainerWithDataAsync()
    {
        await CosmosClientWrapper.EnsureDatabaseAsync();
        Console.WriteLine($"Ensured that DB exists.");
        await CosmosClientWrapper.InitializeContainerAsync(shouldDeleteExisting: true);
        Console.WriteLine($"Created container in Cosmos.");

        await DumpMoviesToContainerAsync();
    }

    private async Task DumpMoviesToContainerAsync()
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
        Console.WriteLine($"\n\nTotal time taken to write {NumberOfRecordsForSimulation} movies = {totalTimeTaken.TotalMilliseconds} ms");
        Console.WriteLine($"\n\nTotal Request Units taken to write {NumberOfRecordsForSimulation} movies = {string.Format("{0:0.00}", totalRUs)}");
    }

    private async Task QueryMoviesWithPkSpecifiedAsync()
    {
        Console.WriteLine($"Querying data from Cosmos by passing partition key.");
        var expectedRating = 5.0;
        var queryText = $"SELECT * FROM c Where c.info.rating > {expectedRating}";
        var totalRUs = 0.0;
        int currentYear = 1991;
        var timer = new Stopwatch();
        timer.Start();
        for(int i = 0; i < NumberOfIterations; i++)
        {
            queryText = $"SELECT * FROM c Where c.info.rating > {expectedRating}";
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
            if (expectedRating >= 10)
            {
                expectedRating = 5.0;
            }
            else
            {
                expectedRating += 0.1;
            }
            totalRUs = totalRUs + currentRU;
        }

        timer.Stop();
        var totalTimeTaken = timer.Elapsed;
        Console.WriteLine($"\n\nTotal time taken to query from {NumberOfRecordsForSimulation} movies = {totalTimeTaken.TotalMilliseconds} ms");
        Console.WriteLine($"\n\nTotal Request Units taken to query movies {NumberOfIterations} times = {string.Format("{0:0.00}", totalRUs)}");
    }

    private async Task QueryMoviesWithoutPkSpecifiedAsync()
    {
        Console.WriteLine($"Querying data from Cosmos without specifying partition key.");
        var expectedRating = 5.0;
        var queryText = $"SELECT * FROM c Where c.info.rating > {expectedRating}";
        var totalRUs = 0.0;
        var timer = new Stopwatch();
        timer.Start();
        for (int i = 0; i < NumberOfRecordsForSimulation; i++)
        {
            queryText = $"SELECT * FROM c Where c.info.rating > {expectedRating}";
            var currentRU = await CosmosClientWrapper.QueryItemsWithoutPartitionKey<Movie>(
                queryText
            );
            Console.WriteLine($"Queried container with RU = {string.Format("{0:0.00}", currentRU)}");
            totalRUs = totalRUs + currentRU;
            if (expectedRating >= 10)
            {
                expectedRating = 5.0;
            }
            else
            {
                expectedRating += 0.1;
            }
        }

        timer.Stop();
        var totalTimeTaken = timer.Elapsed;

        Console.WriteLine($"\n\nTotal time taken to query from {NumberOfRecordsForSimulation} movies = {totalTimeTaken.TotalMilliseconds} ms");
        Console.WriteLine($"\n\nTotal Request Units taken to query movies {NumberOfIterations} times = {string.Format("{0:0.00}", totalRUs)}");
    }

    private async Task CleanupContainerAsync()
    {
        await this.CosmosClientWrapper.CleanupContainerAsync();
    }

    public async Task CleanUpAsync()
    {
        await CosmosClientWrapper.CleanupAsync();
    }

}