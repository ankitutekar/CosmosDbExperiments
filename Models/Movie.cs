namespace CosmosDbExperiments.Models
{
    public class Movie
    {
        public int Year { get; set; }

        public string YearPk { get => Year.ToString(); }
        public string Id { get => Title; }
        public string Title { get; set; }
        public MovieInfo Info { get; set; }
    }
}