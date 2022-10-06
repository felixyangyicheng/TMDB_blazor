using System.Net;
using System.Text.Json;
using TMDB_blazor.Components;
using TMDB_blazor.Contracts;
using TMDB_blazor.Data;
using TMDbLib.Client;
using TMDbLib.Objects.Search;
using TMDbLib.Utilities.Serializer;

namespace TMDB_blazor.Services
{
    public class TmdbExtension : ITmdbExtension
    {
        private readonly IJsonFileRepository _jsonFileRepository;
        private readonly TMDbClient DataClient;
        public TmdbExtension(IJsonFileRepository jsonFileRepository, TMDbClient dataClient)
        {
            _jsonFileRepository = jsonFileRepository;
            DataClient = dataClient;
        }

        public async Task<List<UserMovie>> SearcheWithLocalFilter(string searchWord, bool? viewed, bool? liked, bool adult)
        {
            List<UserMovie> DisplayResult = new List<UserMovie>();
            List<UserMovie> HttpResult = new List<UserMovie>();
            List<UserMovie> ViewedResult = new List<UserMovie>();
            List<UserMovie> LikedResult = new List<UserMovie>();
            var SearchResult = await DataClient.SearchMovieAsync(searchWord, 1, adult);

            for (int i = 0; i < SearchResult.TotalPages; i++)
            {
                var tmp =
                DataClient.SearchMovieAsync(searchWord, i, adult).Result;
                foreach (var item in SearchResult.Results)
                {
                    var jsonsm = JsonSerializer.Serialize(item);
                    UserMovie um = JsonSerializer.Deserialize<UserMovie>(jsonsm);
                    HttpResult.Add(um);
                }

            }

 
                ViewedResult = _jsonFileRepository.ReadAll(Endpoints.jsonViewedPath)
                    .Where(a => a.Title.ToUpper().Contains(searchWord.ToUpper())).ToList();
         
            
                LikedResult = _jsonFileRepository.ReadAll(Endpoints.jsonLikedPath)
                    .Where(a => a.Title.ToUpper().Contains(searchWord.ToUpper())).ToList();
            

            List<UserMovie> commun = (from a in ViewedResult  //éléments commun dans viewed et liked
                                      join b in LikedResult on a.Id equals b.Id
                                      //join c in HttpResult on b.Id equals c.Id
                                      select new UserMovie
                                      {
                                          Id = a.Id,
                                          Adult = a.Adult,
                                          BackdropPath = a.BackdropPath,
                                          Favorite = b.Favorite,
                                          GenreIds = a.GenreIds,
                                          MediaType = a.MediaType,
                                          OriginalLanguage = a.OriginalLanguage,
                                          OriginalTitle = a.OriginalTitle,
                                          Overview = a.Overview,
                                          Popularity = a.Popularity,
                                          PosterPath = a.PosterPath,
                                          ReleaseDate = a.ReleaseDate,
                                          Title = a.Title,
                                          Video = a.Video,
                                          Viewed = a.Viewed,
                                          VoteAverage = a.VoteAverage,
                                          VoteCount = a.VoteCount
                                      }).ToList();

            List<UserMovie> q1 = ViewedResult.Where(a => !commun.Select(b => b.Id).Contains(a.Id)).ToList(); //éléments uniquement dans viewed
            List<UserMovie> q2 = LikedResult.Where(a => !commun.Select(b => b.Id).Contains(a.Id)).ToList();         //élement uniquement dans liked
            DisplayResult.AddRange(q1);
            DisplayResult.AddRange(q2);
            DisplayResult.AddRange(commun);


            var ViewedMovieIDList = ViewedResult.Select(c => c.Id).ToList();
            var LikedMovieIDList = ViewedResult.Select(c => c.Id).ToList();
            List<UserMovie> httpUnviewed = HttpResult.Where(f => !ViewedMovieIDList.Contains(f.Id)).ToList();
            List<UserMovie> httpUnliked = HttpResult.Where(f => !LikedMovieIDList.Contains(f.Id)).ToList();


            List<UserMovie> HttpCommun = (from a in httpUnviewed  //éléments commun dans viewed et liked
                                          join b in httpUnliked on a.Id equals b.Id
                                          //join c in HttpResult on b.Id equals c.Id
                                          select new UserMovie
                                          {
                                              Id = a.Id,
                                              Adult = a.Adult,
                                              BackdropPath = a.BackdropPath,
                                              Favorite = b.Favorite,
                                              GenreIds = a.GenreIds,
                                              MediaType = a.MediaType,
                                              OriginalLanguage = a.OriginalLanguage,
                                              OriginalTitle = a.OriginalTitle,
                                              Overview = a.Overview,
                                              Popularity = a.Popularity,
                                              PosterPath = a.PosterPath,
                                              ReleaseDate = a.ReleaseDate,
                                              Title = a.Title,
                                              Video = a.Video,
                                              Viewed = a.Viewed,
                                              VoteAverage = a.VoteAverage,
                                              VoteCount = a.VoteCount
                                          }).ToList();

            if (viewed==false)
            {
                DisplayResult.RemoveAll(a=> ViewedResult.Select(b=>a.Id).Contains(a.Id));
            DisplayResult.AddRange(HttpCommun);
            }
            else if (liked==false)
            {
                DisplayResult.RemoveAll(a => LikedResult.Select(b => a.Id).Contains(a.Id));

                DisplayResult.AddRange(HttpCommun);
            }
            else
            {
            DisplayResult.AddRange(HttpCommun);

            }
            return DisplayResult;
        }
    }
}
