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
        Console.WriteLine("\n\nClient is ready to communicate!!!\n\n");
    }

    public async Task RunExperimentsAsync()
    {
        bool quitFlag = false;
        while(!quitFlag)
        {
            var options = "1. Initialize container with data\n2. Run queries with PK" +
                          "\n3. Run queries without PK\n4. Replace an item" + 
                          "\n5. Update iterations count" +
                          "\n6. Cleanup container\n7. Quit";
            Console.WriteLine($"{options}\n\nWhat do you want to do? ");
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
                    await ReplaceAnItemInContainerAsync();
                    break;
                case 5:
                    UpdateIterationCount();
                    break;
                case 6:
                    await CleanupContainerAsync();
                    break;
                case 7:
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
        await CosmosClientWrapper.InitializeContainerAsync(shouldDeleteExisting: true);
        Console.WriteLine($"\n\nCreated container in Cosmos.");

        await DumpMoviesToContainerAsync();
    }

    private async Task DumpMoviesToContainerAsync()
    {
        Console.WriteLine($"\n\nDumping data into Cosmos!!!!\n {CosmosClientWrapper.ToString()}");
        var totalRUs = 0.0;
        var timer = new Stopwatch();
        timer.Start();
        foreach(Movie movie in MovieParser.ParseMovies(NumberOfRecordsForSimulation))
        {
            var currentRU = await CosmosClientWrapper.InsertRecordIntoContainerAsync<Movie>(movie, movie.YearPk);
            Console.WriteLine($"Written a doc with RU = {string.Format("{0:0.00}", currentRU)}");
            totalRUs = totalRUs + currentRU;
        }
        timer.Stop();
        var totalTimeTaken = timer.Elapsed;
        Console.WriteLine($"\n\nTotal time taken to write {NumberOfRecordsForSimulation} movies = {totalTimeTaken.TotalMilliseconds} ms");
        Console.WriteLine($"\n\nTotal Request Units taken to write {NumberOfRecordsForSimulation} movies = {string.Format("{0:0.00}", totalRUs)}\n\n");
    }

    private async Task ReplaceAnItemInContainerAsync()
    {
        Console.WriteLine($"\n\nEnter details of item to replace(case-sensitive):");
        Console.WriteLine($"Movie name:");
        var movieName = Console.ReadLine().Trim();
        Console.WriteLine($"Year of release:");
        var yearOfRelease = Console.ReadLine().Trim();
        Console.WriteLine($"\n\nUpdating {movieName} in Cosmos!!!!\n\n");
        var totalRUs = 0.0;
        var timer = new Stopwatch();
        var existingItem = await CosmosClientWrapper.ReadItemWithGivenPartitionKeyAndIdAsync<Movie>(yearOfRelease, movieName);
        Console.WriteLine(existingItem);
        if(existingItem == null)
        {
            return;
        }
        timer.Start();
        var updatedMovie = existingItem;
        for(int i =0; i < NumberOfIterations; i++)
        {
            updatedMovie.Info.RunningTimeSecs += i;
            var currentRU = await CosmosClientWrapper.ReplaceRecordInContainerAsync<Movie>(updatedMovie, existingItem.Id);
            Console.WriteLine($"Updated a doc with RU = {string.Format("{0:0.00}", currentRU)}");
            totalRUs = totalRUs + currentRU;
        }
        timer.Stop();
        var totalTimeTaken = timer.Elapsed;
        Console.WriteLine($"\n\nTotal time taken to update movie {NumberOfIterations} times = {totalTimeTaken.TotalMilliseconds} ms");
        Console.WriteLine($"\n\nTotal Request Units taken to update movie {NumberOfIterations} times = {string.Format("{0:0.00}", totalRUs)}\n\n");
    }

    private async Task QueryMoviesWithPkSpecifiedAsync()
    {
        Console.WriteLine($"\n\nQuerying data from Cosmos by passing partition key.");
        var minRating = 5.0;
        var expectedRating = minRating;
        var queryText = $"SELECT * FROM c Where c.info.rating > {expectedRating}";
        var totalRUs = 0.0;
        
        int minYear = 1991;
        int maxYear = 2013;
        int currentYear = minYear;

        var timer = new Stopwatch();
        timer.Start();
        for(int i = 0; i < NumberOfIterations; i++)
        {
            queryText = $"SELECT * FROM c Where c.info.rating > {expectedRating}";
            var currentRU = await CosmosClientWrapper.QueryItemsWithGivenPartitionKeyAsync<Movie>(
                currentYear.ToString(),
                queryText
            );
            Console.WriteLine($"Queried container with RU = {string.Format("{0:0.00}", currentRU)}");
            if(currentYear >= maxYear) {
                currentYear = minYear;
            }
            else {
                currentYear++;
            }
            if (expectedRating >= 10)
            {
                expectedRating = minRating;
            }
            else
            {
                expectedRating += 0.1;
            }
            totalRUs = totalRUs + currentRU;
        }

        timer.Stop();
        var totalTimeTaken = timer.Elapsed;
        Console.WriteLine($"\n\nTotal time taken to query from {NumberOfRecordsForSimulation} movies, {NumberOfIterations} times = {totalTimeTaken.TotalMilliseconds} ms");
        Console.WriteLine($"\n\nTotal Request Units taken to query movies {NumberOfIterations} times = {string.Format("{0:0.00}", totalRUs)}\n\n");
    }

    private async Task QueryMoviesWithoutPkSpecifiedAsync()
    {
        Console.WriteLine($"\n\nQuerying data from Cosmos without specifying partition key.");
        var minRating = 5.0;
        var expectedRating = minRating;
        var queryText = $"SELECT * FROM c Where c.info.rating > {expectedRating}";
        var totalRUs = 0.0;
        var timer = new Stopwatch();
        timer.Start();
        for (int i = 0; i < NumberOfIterations; i++)
        {
            queryText = $"SELECT * FROM c Where c.info.rating > {expectedRating}";
            var currentRU = await CosmosClientWrapper.QueryItemsWithoutPartitionKeyAsync<Movie>(
                queryText
            );
            Console.WriteLine($"Queried container with RU = {string.Format("{0:0.00}", currentRU)}");
            totalRUs = totalRUs + currentRU;
            if (expectedRating >= 10)
            {
                expectedRating = minRating;
            }
            else
            {
                expectedRating += 0.1;
            }
        }

        timer.Stop();
        var totalTimeTaken = timer.Elapsed;

        Console.WriteLine($"\n\nTotal time taken to query from {NumberOfRecordsForSimulation} movies, {NumberOfIterations} times = {totalTimeTaken.TotalMilliseconds} ms");
        Console.WriteLine($"\n\nTotal Request Units taken to query movies {NumberOfIterations} times = {string.Format("{0:0.00}", totalRUs)}\n\n");
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