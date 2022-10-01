using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using TMDB_blazor.Data;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using static MudBlazor.CategoryTypes;

namespace TMDB_blazor.Components
{
    public partial class Populars
    {

        /// <summary>
        ///  injection d'dépendence TMDBClient
        /// </summary>
        [Inject] TMDbClient DataClient { get; set; }
        /// <summary>
        ///		Obtient ou définit l'identifiant du film à afficher.
        /// </summary>
        [Parameter] public int Identifier { get; set; }
        /// <summary>
        /// Parametre: notification quand un film est sélectionné
        /// </summary>
        [Parameter] public EventCallback<SearchMovie> OnSelectItem { get; set; }
        /// <summary>
        /// Parametre: un film sélectionné
        /// </summary>
        [Parameter] public SearchMovie SelectedItem { get; set; }
        /// <summary>
        /// uri prefix of image
        /// </summary>
   

        public string ImagePrefix { get; set; } = Endpoints.ImagePathPrefix;
        /// <summary>
        /// uri de l'image alternative quand l'image originale n'est pas chargée.
        /// </summary>
        public string AlternativeImage { get; set; } = Endpoints.AlternativeImage;
        /// <summary>
        /// Object retourné de la fonction GetMoviePopularListAsync().Result
        /// </summary>
        public SearchContainer<SearchMovie> SearchContainer { get; set; }
        /// <summary>
        /// nom de MudCarousel
        /// </summary>
        private MudCarousel<string> _carousel;
        /// <summary>
        /// les flèches ou pas
        /// </summary>
        private bool _arrows = true;
        /// <summary>
        /// les bulles ou pas
        /// </summary>
        private bool _bullets = false;
        /// <summary>
        /// les swipes ou pas
        /// </summary>
        private bool _enableSwipeGesture = true;
        /// <summary>
        /// lecture automatique ou pas
        /// </summary>
        private bool _autocycle = false;
        /// <summary>
        /// liste de nom des éléments à afficher dans caroussel
        /// </summary>
        private IList<string> _source = new List<string>();
        /// <summary>
        /// index de l'élément sélectionné
        /// </summary>
        private int selectedIndex = 0;

        //public List<Movie> Movies { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override async Task OnParametersSetAsync()
        {
            SearchContainer = await DataClient.GetMoviePopularListAsync();
            foreach (var i in SearchContainer.Results)
            {
                _source.Add(i.Title);
            }
            await base.OnParametersSetAsync();
        }
        /// <summary>
        /// Obtenir l'uri complet de l'affiche du film
        /// </summary>
        /// <param name="posterPath"></param>
        /// <returns></returns>
        protected string GetCompletedPosterPath(string posterPath)
        {
            return ImagePrefix + posterPath;
        }
        /// <summary>
        /// sélectionné le film -> (attendre un callback dans un autre composant)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected async Task ClickMovie(int id)
        {
            await OnSelectItem.InvokeAsync(SearchContainer.Results[id]);
            StateHasChanged();
        }

    }

}
