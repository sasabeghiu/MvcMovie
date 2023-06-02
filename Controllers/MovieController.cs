using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Newtonsoft.Json;
using System.Web;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace MvcMovie.Controllers
{
    public class MovieController : Controller
    {
        private readonly HttpClient httpClient;

        public MovieController(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Search(string query)
        {
            //encode query to handle specials chars
            var apiKey = "1cd72031";
            var encodedQuery = HttpUtility.UrlEncode(query);
            var apiUrl = $"http://www.omdbapi.com/?apikey={apiKey}&s={encodedQuery}";

            //send a get request to api and throw exception if response is not successful
            var response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            //read response as string and parse it to a list of movie models
            var result = await response.Content.ReadAsStringAsync();
            var model =  await ParseMovieData(result);

            return View("Results", model);
        }

        //this method parses the movie data without a trailer.
        //use this method in case youtube throws a exceeded quota error
        /*private List<MovieModel> ParseMovieData(string result)
        {
            //deserialize JSON response string to a list of movie objects
            var parsedResult = JsonConvert.DeserializeObject<SearchResult>(result);

            if (parsedResult != null && parsedResult.Search != null)
            {
                Console.WriteLine("Parsed result count: " + parsedResult.Search.Count);
                return parsedResult.Search;
            }
            else
            {
                Console.WriteLine("Error parsing movie data: " + result);
                return new List<MovieModel>();
            }
        }*/

        //this method parses the movie data with a trailer
        private async Task<List<MovieModel>> ParseMovieData(string result)
        {
            //deserialize JSON response string to a list of movie objects
            var parsedResult = JsonConvert.DeserializeObject<SearchResult>(result);
            var movies = new List<MovieModel>();

            if (parsedResult != null && parsedResult.Search != null)
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = "AIzaSyB7QCvyN3mEumyDscwPYQu5VsHlMAkmg0A"
                });


                foreach (var movie in parsedResult.Search)
                {
                    var searchListRequest = youtubeService.Search.List("snippet");
                    searchListRequest.Q = movie.Title + " trailer";
                    searchListRequest.MaxResults = 1;

                    var searchListResponse = await searchListRequest.ExecuteAsync();
                    var searchResult = searchListResponse.Items.FirstOrDefault();

                    if (searchResult != null && searchResult.Id.Kind == "youtube#video")
                    {
                        var trailerVideoId = searchResult.Id.VideoId;
                        movie.TrailerUrl = $"https://www.youtube.com/embed/{trailerVideoId}";
                    }

                    movies.Add(movie);
                }

                return movies;
            }
            else
            {
                Console.WriteLine("Error parsing movie data: " + result);
                return new List<MovieModel>();
            }
        }

        public IActionResult Results(List<MovieModel> data)
        {
            return View(data);
        }
    }
}
//AIzaSyB7QCvyN3mEumyDscwPYQu5VsHlMAkmg0A
