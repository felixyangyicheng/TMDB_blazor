using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using TMDB_blazor.Data;

namespace TMDB_blazor.Components
{
    public partial class ViewedList
    {
        #region dependency injection
        /// <summary>
        /// injection service Snackbar
        /// </summary>
        [Inject, NotNull] ISnackbar Snackbar { get; set; } = default!;
        #endregion
        #region properties
        /// <summary>
        /// liste des films visionnés de json
        /// </summary>
        public List<UserMovie> viewed { get; set; }= new List<UserMovie>();
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
            string jsonViewed = File.ReadAllText("wwwroot/data/viewed.json");
            viewed = JsonSerializer.Deserialize<List<UserMovie>>(jsonViewed)??throw new NullReferenceException("viewd json is empty");
            FiltredViewed = viewed;
            await base.OnInitializedAsync();
        }
        /// <summary>
        /// Obtenir l'uri complet de l'affiche de film
        /// </summary>
        /// <param name="posterPath"></param>
        /// <returns></returns>
        protected string GetCompletedPosterPath(string posterPath)
        {
            return ImagePrefix + posterPath;
        }
        /// <summary>
        /// Supprimer le film de la liste, puis ré-écrire la liste dans le fichier json
        /// </summary>
        /// <param name="userMovie"></param>
        /// <returns></returns>
        protected async Task RemoveFromList(UserMovie userMovie)
        {
            string jsonViewed ="[]";
            FiltredViewed.Remove(userMovie);
            viewed=FiltredViewed;
            if (viewed.Count!=0)
            {        
                jsonViewed=JsonSerializer.Serialize(viewed);
            }
            if (viewed.Count == 1)
            {
                jsonViewed = "[" + jsonViewed + "]";
            }
            File.WriteAllText("wwwroot/data/viewed.json", jsonViewed);
            Snackbar.Add("Movie from list sucessfully");
            await InvokeAsync(StateHasChanged);
        }
        /// <summary>
        /// Rechercher
        /// </summary>
        /// <param name="el"></param>
        protected void SearchChanged(string el)
        {
            FiltredViewed = viewed.Where(a => (a.Title.ToUpper().Contains(el.ToUpper())
                                            || a.OriginalTitle.ToUpper().Contains(el.ToUpper())
                                            || ((DateTime)a.ReleaseDate).ToString("d").Contains(el)
                                            )).ToList();

            StateHasChanged();
        }
        #endregion

    }
}
