using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CosmosDbExperiments.Models
{
    public class MovieInfo
    {
        public IList<string> Actors { get; set; }
        public IList<string> Directors { get; set; }
        public IList<string> Genres { get; set; }
        public string Plot { get; set; }

        [JsonProperty(PropertyName = "release_date")]
        public DateTime ReleaseDate { get; set; }
        public float Rating { get; set; }
        public int Rank { get; set; }

        [JsonProperty(PropertyName = "running_time_secs")]
        public int RunningTimeSecs { get; set; }

        [JsonProperty(PropertyName = "image_url")]
        public string ImageUrl { get; set; }
    }
}