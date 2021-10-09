using System;
using System.Collections;
using System.IO;
using System.Reflection;
using CosmosDbExperiments.Models;
using Newtonsoft.Json;

namespace CosmosDbExperiments.Tools
{
    public static class MovieParser
    {
        public static IEnumerable ParseMovies(int noOfRecordsToRead)
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            JsonSerializer serializer = new JsonSerializer();
            int count = 0;
            Movie movie; 
            using (FileStream fs = new FileStream($"{path}/Data/movies.json", FileMode.Open, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                while (reader.Read() && count < noOfRecordsToRead)
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        movie = serializer.Deserialize<Movie>(reader);
                        count++;
                        yield return movie;
                    }
                }
            }
            Console.WriteLine(count);
        }
        
    }
}