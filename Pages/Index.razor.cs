using TMDB_blazor.Components;
using TMDbLib.Objects.Discover;

namespace TMDB_blazor.Pages
{
    public partial class Index
    {
        #region denpency injection

        [Inject, NotNull] IJsonFileRepository _json { get; set; } = default!;
        /// <summary>
        /// injection service Snackbar
        /// </summary>
        [Inject, NotNull] ISnackbar Snackbar { get; set; } = default!;
        /// <summary>
        ///  injection d'dépendence TMDBClient
        /// </summary>
        [Inject, NotNull] TMDbClient DataClient { get; set; } = default!;
        /// <summary>
        ///  injection d'dépendence NavigationManager
        /// </summary>
        [Inject, NotNull] NavigationManager Nav { get; set; } = default!;
        /// <summary>
        ///		Obtient ou définit l'identifiant du film à afficher.
        /// </summary>
        [Parameter] public int Identifier { get; set; }
        #endregion
        #region properties

        public SearchContainer<SearchMovie> list { get; set; } =new();

        public DiscoverMovie dm { get; set; } = default!;
        public List<UserMovie> favorites { get; set; } = new();
        public List<UserMovie> FiltredFavorites { get; set; } = new();
        public List<UserMovie> viewed { get; set; } = new();
        public List<UserMovie> FiltredViewed { get; set; } = new();
        public SearchMovie SelectItem { get; set; } = new();
        public UserMovie LikedMovie { get; set; } = new();
        public UserMovie ViewedMovie { get; set; } = new();
        public bool popularVisible { get; set; } = true;
        /// <summary>
        /// html class pour composant caroussel
        /// </summary>
        public string popularStyle { get; set; } = "row index-popular-show";
        /// <summary>
        /// text affichage sur boutton de changement de visibilité de caroussel et détail de film
        /// </summary>
        public string PopularButtonString { get; set; } = "Close popular";
        /// <summary>
        /// élément par page dans la liste(tableau) générique
        /// </summary>
        public int itemsPerPage { get; set; } = 5;
        #endregion
        #region methods

        protected override async Task OnInitializedAsync()
        {
            //list = await DataClient.GetMoviePopularListAsync();

            dm = DataClient.DiscoverMoviesAsync()!;

            list = (await dm.OrderBy(DiscoverMovieSortBy.Revenue).Query(1))!;

            viewed = await _json.ReadAllAsync(Endpoints.jsonViewedPath);
            favorites = await _json.ReadAllAsync(Endpoints.jsonLikedPath);

            FiltredFavorites = favorites;
            FiltredViewed = viewed;
            await base.OnInitializedAsync();
        }

        /// <summary>
        /// passer l'instance de film sélectionné de composant caroussel au composant détail de film
        /// </summary>
        /// <param name="item"></param>
        void Select(SearchMovie item)
        {
            SelectItem = item;
        }

        /// <summary>
        /// Ajouter le film dans la liste des visionnés, puis ré-écrire dans le fichier json
        /// </summary>
        /// <param name="movie"></param>
        protected async Task AddViewed(SearchMovie movie)
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
                await _json.SaveAsync(viewed, Endpoints.jsonViewedPath);
                Snackbar.Add("Movie added to list sucessfully");
                await InvokeAsync(StateHasChanged);
            }
        }
        /// <summary>
        /// Ajouter le film dans la liste des préférés, puis ré-écrire dans le fichier json
        /// </summary>
        /// <param name="movie"></param>
        protected async Task AddLiked(SearchMovie movie)
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
                await _json.SaveAsync(favorites, Endpoints.jsonLikedPath);
                Snackbar.Add("Movie added to list sucessfully");
                await InvokeAsync(StateHasChanged);
            }
        }
        protected void ChangeVisibility()
        {
            popularVisible = !popularVisible;
            if (popularVisible == true)
            {
                popularStyle = "row index-popular-show";
                PopularButtonString = "Close popular";
                itemsPerPage = 5;
            }
            else
            {
                popularStyle = "index-popular-hide";
                PopularButtonString = "Open popular";
                itemsPerPage = 10;
            }
            StateHasChanged();
        }
        /// <summary>
        /// Rechercher
        /// </summary>
        /// <param name="el"></param>
        protected void SearchChanged(string el)
        {
            if (!string.IsNullOrEmpty(el))
            {
                FiltredViewed = viewed.Where(a => a.Title?.ToUpper().Contains(el.ToUpper()) == true
                                               || a.OriginalTitle?.ToUpper().Contains(el.ToUpper()) == true
                                               || a.ReleaseDate?.ToString("d").Contains(el) == true
                                          ).ToList();
                FiltredFavorites = favorites.Where(a => a.Title?.ToUpper().Contains(el.ToUpper()) == true
                                                     || a.OriginalTitle?.ToUpper().Contains(el.ToUpper()) == true
                                                     || a.ReleaseDate?.ToString("d").Contains(el) == true
                                                ).ToList();
                StateHasChanged();
            }
        }
        /// <summary>
        /// Rediriger vers la page détail de film
        /// </summary>
        /// <param name="movie"></param>
        protected void Redirect(SearchMovie movie)
        {
            Nav.NavigateTo("/moviepage/" + movie.Id);
        }
        #endregion

    }
}
