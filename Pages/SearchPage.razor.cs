using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json;
using System.Text.RegularExpressions;
using TMDB_blazor.Contracts;
using TMDB_blazor.Data;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using static MudBlazor.CategoryTypes;

namespace TMDB_blazor.Pages
{
    public partial class SearchPage
    {

        #region Inject
        /// <summary>
        /// injection d'dépendenc pour l'extension de recherche TMDB wrapper
        /// </summary>
        [Inject] ITmdbExtension _TmdbExtension { get; set; }
        /// <summary>
        /// injection d'dépendenc pour la lecture et écriture fichier json
        /// </summary>
        [Inject] IJsonFileRepository _json { get; set; }

        /// <summary>
        ///  injection d'dépendence NavigationManager
        /// </summary>
        [Inject] NavigationManager Nav { get; set; }
        /// <summary>
        /// injection service Snackbar
        /// </summary>
        [Inject] ISnackbar Snackbar { get; set; }
        /// <summary>
        ///  injection d'dépendence TMDBClient
        /// </summary>
        [Inject] TMDbClient DataClient { get; set; }
        #endregion Inject
        #region Parameters
        #endregion Parameters
        #region Properties
        /// <summary>
        /// chaine de caractères de recherche
        /// </summary>
        public string searchWord { get; set; } 
        /// <summary>
        /// le résultat brut de API
        /// </summary>
        public SearchContainer<SearchMovie> SearchResult { get; set; }
        /// <summary>
        /// Résultat à afficher
        /// </summary>
        public List<UserMovie> DisplayResult { get; set; } = new List<UserMovie>();
        /// <summary>
        /// Liste de visionnés
        /// </summary>
        public List<UserMovie> viewed { get; set; }
        /// <summary>
        /// liste de préférés
        /// </summary>
        public List<UserMovie> favorites { get; set; }
        /// <summary>
        /// condition de recherche: films visionnés
        /// </summary>
        public bool SearchViewedEnable { get; set; } = true;
        /// <summary>
        /// condition de recherche: films préférés
        /// </summary>
        public bool SearchLikedEnable { get; set; } = true;
        /// <summary>
        /// condition de recherche: films avec contenu pour adults
        /// </summary>
        public bool SearchAdultEnable { get; set; } = true;
        /// <summary>
        /// état de rechargement
        /// </summary>
        public bool loading { get; set; } = false;

        #endregion Properties
        #region Methods

        protected override async Task OnInitializedAsync()
        {
            DisplayResult = new List<UserMovie>();         
            //liste des films visonnés
            viewed = _json.ReadAll(Endpoints.jsonViewedPath);  
            //liste des films préférés
            favorites = _json.ReadAll(Endpoints.jsonLikedPath); 
            //les éléments communs de liste des visionnés et celle des préférés
            List<UserMovie> commun  = (from a in viewed      
                                  join b in favorites on a.Id equals b.Id
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
            //les éléments uniquement dans liste visionnés
            List<UserMovie> q1 = viewed.Where(a => !commun.Select(b => b.Id).Contains(a.Id)).ToList();
            //les éléments uniquement dans liste préférés
            List<UserMovie> q2 = favorites.Where(a => !commun.Select(b => b.Id).Contains(a.Id)).ToList();
            DisplayResult.AddRange(q1);
            DisplayResult.AddRange(q2);
            DisplayResult.AddRange(commun);
            await base.OnInitializedAsync();
        }
        /// <summary>
        /// quand la recherche est validée 
        /// </summary>
        /// <param name="searchWord"></param>
        /// <returns></returns>
        protected async Task InputChanged(string searchWord)
        {
            DisplayResult.Clear();
            if (!string.IsNullOrEmpty(searchWord))
            {
                loading = true;
                DisplayResult = await _TmdbExtension.SearcheWithLocalFilter(searchWord, SearchViewedEnable, SearchLikedEnable, SearchAdultEnable);          
                StateHasChanged();
                loading = false;
            }
        }
        protected async Task ChangeSearchViewedCondition()
        {
            SearchViewedEnable = !SearchViewedEnable;


            DisplayResult.Clear();
            StateHasChanged();

        }
        protected async Task ChangeSearchLikedCondition()
        {
            SearchLikedEnable = !SearchLikedEnable;

            DisplayResult.Clear();
            StateHasChanged();

        }
        protected void Redirect(SearchMovie movie)
        {
            Nav.NavigateTo("/moviepage/" + movie.Id);
        }
        #endregion Methods
    }
}
