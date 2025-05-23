using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using System.Text.Json;
using TMDB_blazor.Data;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace TMDB_blazor.Pages
{
    public partial class MoviePage
    {
        #region Injection
        /// <summary>
        /// injection service Snackbar
        /// </summary>
        [Inject, NotNull] ISnackbar Snackbar { get; set; } = default!;
        /// <summary>
        ///  injection d'dépendence TMDBClient
        /// </summary>
        [Inject, NotNull] TMDbClient DataClient { get; set; } = default!;
        #endregion Injection

        #region Parameters

        [Parameter]public int Identity { get; set; }
        #endregion Parameters

        #region Properties

        public SearchContainer<SearchMovie> ApiList { get; set; } = new();
        public UserMovie LikedMovie { get; set; } = new();
        public UserMovie ViewedMovie { get; set; } = new();
        public List<UserMovie> favorites { get; set; } = new();
        public List<UserMovie> viewed { get; set; } = new();
        public string ImagePrefix { get; set; } = Endpoints.ImagePathPrefix;

        public Movie Movie { get; set; } = new();

        #endregion Properties
        #region Methods


        protected override async Task OnParametersSetAsync()
        {
            if (Identity > 0)
            {
                //Lorsque l'identifiant est défini, on appel en async l'API pour récupérer le film correspondant.
                Movie = await DataClient.GetMovieAsync(Identity);
                
            }
            string jsonViewed = File.ReadAllText("wwwroot/data/viewed.json");
            string jsonLiked = File.ReadAllText("wwwroot/data/favorite.json");

            viewed = JsonSerializer.Deserialize<List<UserMovie>>(jsonViewed) ?? throw new NullReferenceException("viewd json is empty");
            favorites = JsonSerializer.Deserialize<List<UserMovie>>(jsonLiked) ?? throw new NullReferenceException("favorites json is empty");

        }

        void AddViewed(Movie movie)
        {
            if (viewed.Any(a => a.Id == movie.Id))
            {
                Snackbar.Add("Movie already added to list !");
            }
            else
            {
                ViewedMovie = new UserMovie
                {
                    Viewed = true,
                    Adult = movie.Adult,
                    BackdropPath = movie.BackdropPath,
                    
                    Id = movie.Id,
           
                    OriginalLanguage = movie.OriginalLanguage,
                    OriginalTitle = movie.OriginalTitle,
                    Overview = movie.Overview,
                    Popularity = movie.Popularity ?? 0,
                    PosterPath = movie.PosterPath,
                    ReleaseDate = movie.ReleaseDate,
                    Title = movie.Title,
                    Video = movie.Video,
                    VoteAverage = movie.VoteAverage,
                    VoteCount = movie.VoteCount,
                };
                viewed.Add(ViewedMovie);
                string content = JsonSerializer.Serialize(viewed);

                if (viewed.Count == 1)
                {
                    content = "[" + content + "]";
                }
                File.WriteAllText("wwwroot/data/viewed.json", content);
                Snackbar.Add("Movie added to list sucessfully");
                StateHasChanged();
            }
        }

        void AddLiked(Movie movie)
        {
            if (favorites.Any(a => a.Id == movie.Id))
            {
                Snackbar.Add("Movie already added to list !");
            }
            else
            {
                LikedMovie = new UserMovie
                {
                    Favorite = true,
                    Adult = movie.Adult,
                    BackdropPath = movie.BackdropPath,
     
                    Id = movie.Id,
                    OriginalLanguage = movie.OriginalLanguage,
                    OriginalTitle = movie.OriginalTitle,
                    Overview = movie.Overview,
                    Popularity = movie.Popularity??0,
                    PosterPath = movie.PosterPath,
                    ReleaseDate = movie.ReleaseDate,
                    Title = movie.Title,
                    Video = movie.Video,
                    VoteAverage = movie.VoteAverage,
                    VoteCount = movie.VoteCount,
                };
                favorites.Add(LikedMovie);
                string content = JsonSerializer.Serialize(favorites);
                if (favorites.Count == 1)
                {
                    content = "[" + content + "]";
                }
                File.WriteAllText("wwwroot/data/favorite.json", content);
                Snackbar.Add("Movie added to list sucessfully");
                StateHasChanged();
            }

        }
        protected string GetCompletedPosterPath(string posterPath)
        {
            return ImagePrefix + posterPath;
        }

        protected  string GetMovieTrailer(int id)
        {
            var result = DataClient.GetMovieVideosAsync(id).Result;
            var youtubeKey=result.Results.First().Key;

            return $"https://www.youtube.com/embed/{youtubeKey}";
        }
        #endregion Methods
    }
}
