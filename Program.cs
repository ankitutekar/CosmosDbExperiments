using System;
using Microsoft.Extensions.Configuration;

namespace CosmosDbExperiments
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            CosmosExperiments CosmosExperiments = new CosmosExperiments(GetConfiguration());  
            //MovieParser.ParseMovies();
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
