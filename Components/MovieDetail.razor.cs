using Microsoft.AspNetCore.Components;
using TMDB_blazor.Data;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace TMDB_blazor.Components
{
    public partial class MovieDetail
    {
        /// <summary>
        ///  injection d'dépendence NavigationManager
        /// </summary>
        [Inject] NavigationManager Nav { get; set; }
        #region parameters


        /// <summary>
        /// Parametre:Item sélectionné
        /// </summary>
        [Parameter] public SearchMovie SelectedItem { get; set; }

        /// <summary>
        /// Parametre: notification:quand un film est sélectionné en tant que déjà vu
        /// </summary>
        [Parameter] public EventCallback<SearchMovie> OnMovieViewed { get; set; }
        /// <summary>
        /// un film vu
        /// </summary>
        [Parameter] public SearchMovie ViewedMovie { get; set; }
        /// <summary>
        /// Parametre: notification:quand un film est sélectionné en tant que préféré
        /// </summary>
        [Parameter] public EventCallback<SearchMovie> OnMovieLiked { get; set; }
        /// <summary>
        /// un film préféré
        /// </summary>

        [Parameter] public SearchMovie LikedMovie { get; set; }
        #endregion parameters
        #region properties
        public int MyProperty { get; set; }
        #endregion
        #region properties

        /// <summary>
        /// uri prefix of image
        /// </summary>
        public string ImagePrefix { get; set; } = Endpoints.ImagePathPrefix;
        /// <summary>
        /// uri de l'image alternative quand l'image originale n'est pas chargée.
        /// </summary>
        public string AlternativeImage { get; set; } = Endpoints.AlternativeImage;
        #endregion
        #region methods
        /// <summary>
        /// Obtenir l"uri complet de l'affiche du film
        /// </summary>
        /// <param name="posterPath"></param>
        /// <returns></returns>
        protected string? GetCompletedPosterPath(string posterPath)
        {
            return ImagePrefix + posterPath;
        }
        /// <summary>
        /// Ajouter le film dans la liste des visionnés
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        protected async Task AddToViewed(SearchMovie movie)
        {
            if (movie != null)
            {
                await OnMovieViewed.InvokeAsync(movie);
                StateHasChanged();
            }
        }
        /// <summary>
        /// Ajouter le film dans la liste des préférés
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        protected async Task AddToFavorite(SearchMovie movie)
        {
            if (movie != null)
            {
                await OnMovieLiked.InvokeAsync(movie);
                StateHasChanged();
            }
        }
        /// <summary>
        /// Rediriger vers la page pour les détail du film sélectionné
        /// </summary>
        /// <param name="movie"></param>
        protected void Redirect(SearchMovie movie)
        {
            Nav.NavigateTo("/moviepage/" + movie.Id);
        }
        #endregion methods
    }
}
