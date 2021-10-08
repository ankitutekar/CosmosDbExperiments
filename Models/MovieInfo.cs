using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CosmosDbExperiments.Models
{
    public class MovieInfo
    {
        public IList<string> Actors { get; set; }
        public IList<string> Directors { get; set; }
        public IList<string> Genres { get; set; }
        public string Plot { get; set; }
        public DateTime ReleaseDate { get; set; }
        public float Rating { get; set; }
        public int Rank { get; set; }

        [JsonPropertyName("running_time_secs")]
        public int RunningTimeSecs { get; set; }
        
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }
    }
}