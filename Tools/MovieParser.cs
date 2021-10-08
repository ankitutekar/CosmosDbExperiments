using System;
using System.IO;
using System.Reflection;
using CosmosDbExperiments.Models;
using Newtonsoft.Json;

namespace CosmosDbExperiments.Tools
{
    public static class MovieParser
    {
        public static void ParseMovies()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Console.WriteLine(path);
            JsonSerializer serializer = new JsonSerializer();
            int count = 0;
            Movie movie; 
            using (FileStream fs = new FileStream($"{path}/Data/movies.json", FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        movie = serializer.Deserialize<Movie>(reader);
                        Console.WriteLine(movie.Info?.Plot);
                        count++;
                    }
                }
            }
            Console.WriteLine(count);
        }
        
    }
}