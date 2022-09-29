using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using TMDB_blazor.Components;
using TMDB_blazor.Data;
using TMDbLib.Client;
using TMDbLib.Objects.Discover;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace TMDB_blazor.Pages
{
    public partial class Index
    {
        /// <summary>
        /// injection service Snackbar
        /// </summary>
        [Inject] ISnackbar Snackbar { get; set; }
        /// <summary>
        ///  injection d'dépendence TMDBClient
        /// </summary>
        [Inject] TMDbClient DataClient { get; set; }
        /// <summary>
        ///		Obtient ou définit l'identifiant du film à afficher.
        /// </summary>
        [Parameter] public int Identifier { get; set; }

        public string ImagePrefix { get; set; } = Endpoints.ImagePathPrefix;
        public string AlternativeImage { get; set; } = Endpoints.AlternativeImage;

        public SearchContainer<SearchMovie> list { get; set; }

        public DiscoverMovie dm { get; set; }
        public List<UserMovie> favorites { get; set; }
        public List<UserMovie> FiltredFavorites { get; set; }
        public List<UserMovie> viewed { get; set; }
        public List<UserMovie> FiltredViewed { get; set; }
        public List<SearchMovie> movies { get; set; }
        public List<SearchContainer<SearchMovie>> Movies { get; set; }
        public SearchMovie SelectItem { get; set; }
        public UserMovie LikedMovie { get; set; }
        public UserMovie ViewedMovie { get; set; }
        public bool popularVisible { get; set; } = true;
        public string popularStyle { get; set; } = "display:block";
        public string PopularButtonString { get; set; } = "Close popular";
        public int itemsPerPage { get; set; } = 5;
        protected override async Task OnInitializedAsync()
        {
            //list = await DataClient.GetMoviePopularListAsync();

            dm = DataClient.DiscoverMoviesAsync();

            list = await dm.OrderBy(DiscoverMovieSortBy.Revenue).Query(1);
            string jsonViewed = File.ReadAllText("wwwroot/data/viewed.json");
            string jsonLiked = File.ReadAllText("wwwroot/data/favorite.json");

            viewed = JsonSerializer.Deserialize<List<UserMovie>>(jsonViewed);
            favorites = JsonSerializer.Deserialize<List<UserMovie>>(jsonLiked);

            FiltredFavorites = favorites;
            FiltredViewed = viewed;
            await base.OnInitializedAsync();
        }


        void Select(SearchMovie item)
        {
            SelectItem = item;
        }


        void AddViewed(SearchMovie movie)
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
                    GenreIds = movie.GenreIds,
                    Id = movie.Id,
                    MediaType = movie.MediaType,
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
                File.WriteAllText("wwwroot/data/viewed.json", content);
                Snackbar.Add("Movie added to list sucessfully");
                StateHasChanged();
            }
        }

        void AddLiked(SearchMovie movie)
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
                    GenreIds = movie.GenreIds,
                    Id = movie.Id,
                    MediaType = movie.MediaType,
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
                string content = JsonSerializer.Serialize(LikedMovie);
                File.WriteAllText("wwwroot/data/favorite.json", content);
                Snackbar.Add("Movie added to list sucessfully");
                StateHasChanged();
            }

        }
        protected string GetCompletedPosterPath(string posterPath)
        {
            return ImagePrefix + posterPath;
        }

        protected void ChangeVisibility()
        {
            popularVisible = !popularVisible;
            if (popularVisible == true)
            {
                popularStyle = "display:block";
                PopularButtonString = "Close popular";
                itemsPerPage = 5;
            }
            else
            {
                popularStyle = "display:none";
                PopularButtonString = "Open popular";
                itemsPerPage = 10;

            }
            StateHasChanged();
        }

        protected void SearchChanged(string el)
        {
            FiltredViewed = viewed.Where(a => (a.Title.ToUpper().Contains(el.ToUpper()) 
                                            || a.OriginalTitle.ToUpper().Contains(el.ToUpper())
                                            ||((DateTime)a.ReleaseDate).ToString("d").Contains(el)

                                            )).ToList();
            FiltredFavorites = favorites.Where(a => (a.Title.ToUpper().Contains(el.ToUpper()) 
                                                    || a.OriginalTitle.ToUpper().Contains(el.ToUpper())
                                                    ||((DateTime)a.ReleaseDate).ToString("d").Contains(el)
                                                    )).ToList();
            StateHasChanged();
        }
    }
}
