using System;
using CosmosDbExperiments.Tools;

namespace CosmosDbExperiments
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            MovieParser.ParseMovies();
        }
    }
}
