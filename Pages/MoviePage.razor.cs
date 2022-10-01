using Microsoft.AspNetCore.Components;
using MudBlazor;
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
        [Inject] ISnackbar Snackbar { get; set; }
        /// <summary>
        ///  injection d'dépendence TMDBClient
        /// </summary>
        [Inject] TMDbClient DataClient { get; set; }
        #endregion Injection

        #region Parameters

        [Parameter]public int Identity { get; set; }
        #endregion Parameters

        #region Properties

        public SearchContainer<SearchMovie> ApiList { get; set; }
        public UserMovie LikedMovie { get; set; }
        public UserMovie ViewedMovie { get; set; }
        public List<UserMovie> favorites { get; set; }
        public List<UserMovie> viewed { get; set; }
        public string ImagePrefix { get; set; } = Endpoints.ImagePathPrefix;

        public Movie Movie { get; set; }

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

            viewed = JsonSerializer.Deserialize<List<UserMovie>>(jsonViewed);
            favorites = JsonSerializer.Deserialize<List<UserMovie>>(jsonLiked);

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
                    Popularity = movie.Popularity,
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
                    Popularity = movie.Popularity,
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
        #endregion Methods
    }
}
