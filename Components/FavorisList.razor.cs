using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json;
using TMDB_blazor.Contracts;
using TMDB_blazor.Data;
using TMDB_blazor.Pages;
using TMDbLib.Objects.Discover;

namespace TMDB_blazor.Components
{
	public partial class FavorisList
	{
        /// <summary>
        /// injection service Snackbar
        /// </summary>
        [Inject] ISnackbar Snackbar { get; set; }
        /// <summary>
        /// injection d'dépendenc pour la lecture et écriture fichier json
        /// </summary>
        [Inject] IJsonFileRepository _json { get; set; }
        /// <summary>
        /// liste des préférés de json
        /// </summary>
        public List<UserMovie> favorites { get; set; }
        /// <summary>
        /// liste filtrée des préférés
        /// </summary>
        public List<UserMovie> FiltredFavorites { get; set; }
        /// <summary>
        /// uri de l'image.
        /// </summary>
        public string ImagePrefix { get; set; } = Endpoints.ImagePathPrefix;
        /// <summary>
        /// Initiation de conposant
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            favorites = _json.ReadAll(Endpoints.jsonLikedPath);
            FiltredFavorites = favorites;
            await base.OnInitializedAsync();
        }
        /// <summary>
        /// Permettre d'avoir l'uri complet de l'affiche du film
        /// </summary>
        /// <param name="posterPath"></param>
        /// <returns></returns>
        protected string GetCompletedPosterPath(string posterPath)
        {
            return ImagePrefix + posterPath;
        }
        /// <summary>
        /// Supprimer le film de la liste, puis ré-écrire la liste dans le fichier
        /// </summary>
        /// <param name="userMovie"></param>
        /// <returns></returns>
        protected async Task RemoveFromList(UserMovie userMovie)
        {
            string jsonliked = "[]";
            FiltredFavorites.Remove(userMovie);
            favorites = FiltredFavorites;
            if (favorites.Count != 0)
            {
                jsonliked = JsonSerializer.Serialize(favorites);
            }
            if (favorites.Count==1)
            {
                jsonliked= "[" + jsonliked + "]";
            }
            File.WriteAllText("wwwroot/data/favorite.json", jsonliked);
            Snackbar.Add("Movie from list sucessfully");
            StateHasChanged();
        }
        /// <summary>
        /// Rechercher 
        /// </summary>
        /// <param name="el"></param>
        protected void SearchChanged(string el)
        {
            FiltredFavorites = favorites.Where(a => (a.Title.ToUpper().Contains(el.ToUpper())
                                            || a.OriginalTitle.ToUpper().Contains(el.ToUpper())
                                            || ((DateTime)a.ReleaseDate).ToString("d").Contains(el)
                                            )).ToList();
            StateHasChanged();
        }
    }
}
