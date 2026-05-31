using Microsoft.AspNetCore.Components;
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
        /// <summary>
        /// injection du repository JSON
        /// </summary>
        [Inject, NotNull] IJsonFileRepository _json { get; set; } = default!;
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
        public string MovieTrailerUrl { get; set; } = "";

        #endregion Properties
        #region Methods


        protected override async Task OnParametersSetAsync()
        {
            if (Identity > 0)
            {
                Movie = await DataClient.GetMovieAsync(Identity) ?? new Movie();
                if (Movie.Id > 0)
                    MovieTrailerUrl = await GetMovieTrailerAsync(Movie.Id);
            }
            viewed = await _json.ReadAllAsync(Endpoints.jsonViewedPath);
            favorites = await _json.ReadAllAsync(Endpoints.jsonLikedPath);
        }

        protected async Task AddViewed(Movie movie)
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
                await _json.SaveAsync(viewed, Endpoints.jsonViewedPath);
                Snackbar.Add("Movie added to list sucessfully");
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async Task AddLiked(Movie movie)
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
                    Popularity = movie.Popularity ?? 0,
                    PosterPath = movie.PosterPath,
                    ReleaseDate = movie.ReleaseDate,
                    Title = movie.Title,
                    Video = movie.Video,
                    VoteAverage = movie.VoteAverage,
                    VoteCount = movie.VoteCount,
                };
                favorites.Add(LikedMovie);
                await _json.SaveAsync(favorites, Endpoints.jsonLikedPath);
                Snackbar.Add("Movie added to list sucessfully");
                await InvokeAsync(StateHasChanged);
            }
        }
        protected string GetCompletedPosterPath(string? posterPath)
        {
            return string.IsNullOrEmpty(posterPath) ? Endpoints.AlternativeImage : ImagePrefix + posterPath;
        }

        private async Task<string> GetMovieTrailerAsync(int id)
        {
            var result = await DataClient.GetMovieVideosAsync(id) ?? new();
            var youtubeKey = result.Results?.FirstOrDefault()?.Key ?? "";
            return $"https://www.youtube.com/embed/{youtubeKey}";
        }
        #endregion Methods
    }
}
