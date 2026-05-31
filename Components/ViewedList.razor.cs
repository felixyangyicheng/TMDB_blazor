using Microsoft.AspNetCore.Components;

namespace TMDB_blazor.Components
{
    public partial class ViewedList
    {
        #region dependency injection
        /// <summary>
        /// injection service Snackbar
        /// </summary>
        [Inject, NotNull] ISnackbar Snackbar { get; set; } = default!;
        /// <summary>
        /// injection du repository JSON
        /// </summary>
        [Inject, NotNull] IJsonFileRepository _json { get; set; } = default!;
        #endregion
        #region properties
        /// <summary>
        /// liste des films visionnés de json
        /// </summary>
        public List<UserMovie> viewed { get; set; } = new List<UserMovie>();
        /// <summary>
        /// liste filtrée des films visionnés
        /// </summary>
        public List<UserMovie> FiltredViewed { get; set; } = new List<UserMovie>();
   
        /// <summary>
        /// uri de l'image.
        /// </summary>
        public string ImagePrefix { get; set; } = Endpoints.ImagePathPrefix;
        #endregion
        #region methodes

        protected override async Task OnInitializedAsync()
        {
            viewed = await _json.ReadAllAsync(Endpoints.jsonViewedPath);
            FiltredViewed = viewed;
            await base.OnInitializedAsync();
        }
        /// <summary>
        /// Obtenir l'uri complet de l'affiche de film
        /// </summary>
        /// <param name="posterPath"></param>
        /// <returns></returns>
        protected string GetCompletedPosterPath(string? posterPath)
        {
            return string.IsNullOrEmpty(posterPath) ? ImagePrefix + "default" : ImagePrefix + posterPath;
        }
        /// <summary>
        /// Supprimer le film de la liste, puis ré-écrire la liste dans le fichier json
        /// </summary>
        /// <param name="userMovie"></param>
        /// <returns></returns>
        protected async Task RemoveFromList(UserMovie userMovie)
        {
            FiltredViewed.Remove(userMovie);
            viewed = FiltredViewed;
            await _json.SaveAsync(viewed, Endpoints.jsonViewedPath);
            Snackbar.Add("Movie from list sucessfully");
            await InvokeAsync(StateHasChanged);
        }
        /// <summary>
        /// Rechercher
        /// </summary>
        /// <param name="el"></param>
        protected async Task SearchChanged(string el)
        {
            FiltredViewed = viewed.Where(a => (a.Title?.ToUpper().Contains(el.ToUpper()) == true
                                            || a.OriginalTitle?.ToUpper().Contains(el.ToUpper()) == true
                                            || a.ReleaseDate?.ToString("d").Contains(el) == true
                                            )).ToList();
            await InvokeAsync(StateHasChanged);
      
        }
        #endregion

    }
}
