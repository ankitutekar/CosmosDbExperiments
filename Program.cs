using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace CosmosDbExperiments
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("\n\n**********Welcome to Cosmos*********");
            
            CosmosExperiments CosmosExperiments = new CosmosExperiments(GetConfiguration());

            try
            {
                await CosmosExperiments.RunExperimentsAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                await CosmosExperiments.CleanUpAsync();
            }
            Console.WriteLine($"\n\nPress any key to continue...");
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
