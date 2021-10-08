using System.Text.Json.Serialization;

namespace CosmosDbExperiments.Models
{
    public class Movie
    {
        public int Year { get; set; }
        public string Title { get; set; }
        public MovieInfo Info { get; set; }
    }
}