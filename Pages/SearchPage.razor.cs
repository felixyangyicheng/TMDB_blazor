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
        [Inject] ITmdbExtension _TmdbExtension { get; set; }

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
        public string searchWord { get; set; }
        public SearchContainer<SearchMovie> SearchResult { get; set; }

        public List<UserMovie> DisplayResult { get; set; } = new List<UserMovie>();
        public List<UserMovie> viewed { get; set; }
        public List<UserMovie> favorites { get; set; }
        public bool SearchViewedEnable { get; set; } = true;
        public bool SearchLikedEnable { get; set; } = true;
        public bool SearchAdultEnable { get; set; } = true;

        #endregion Properties
        #region Methods

        protected override async Task OnInitializedAsync()
        {
            DisplayResult = new List<UserMovie>();
          

            viewed = _json.ReadAll(Endpoints.jsonViewedPath);

            favorites = _json.ReadAll(Endpoints.jsonLikedPath);
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



            List<UserMovie> q1 = viewed.Where(a => !commun.Select(b => b.Id).Contains(a.Id)).ToList();

            List<UserMovie> q2 = favorites.Where(a => !commun.Select(b => b.Id).Contains(a.Id)).ToList();


            DisplayResult.AddRange(q1);
            DisplayResult.AddRange(q2);
            DisplayResult.AddRange(commun);

            foreach (var item in DisplayResult)
            {
                Console.WriteLine(item.Title + " viewed: " + item.Viewed + " liked: " + item.Favorite);
            }
            //DisplayResult = DisplayResult.DistinctBy(a => a.Id).ToList();
            await base.OnInitializedAsync();
        }

        protected async Task InputChanged(string searchWord)
        {

            DisplayResult.Clear();

            if (!string.IsNullOrEmpty(searchWord))
            {

                DisplayResult = await _TmdbExtension.SearcheWithLocalFilter(searchWord, SearchViewedEnable, SearchLikedEnable, SearchAdultEnable);


                StateHasChanged();

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
