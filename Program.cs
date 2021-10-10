using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CosmosDbExperiments
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            CosmosExperiments CosmosExperiments = new CosmosExperiments(GetConfiguration());

            await CosmosExperiments.InitializeDatabasesWithDataAsync();

            Console.ReadLine();
        }

        static Config.Config GetConfiguration()
        {
            var config = new ConfigurationBuilder()
                                            .AddJsonFile("appsettings.json")
                                            .Build();
            var appSettings = config.GetSection("App");
            var configObj = new Config.Config();
            appSettings.Bind(configObj);
            
            return configObj;
        }
    }
}
