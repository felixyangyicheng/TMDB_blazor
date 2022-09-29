using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json;
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
        public List<UserMovie> favorites { get; set; }
        public List<UserMovie> FiltredFavorites { get; set; }
        /// <summary>
        /// uri de l'image.
        /// </summary>
        public string ImagePrefix { get; set; } = Endpoints.ImagePathPrefix;
        protected override async Task OnInitializedAsync()
        {
            string jsonLiked = File.ReadAllText("wwwroot/data/favorite.json");
            favorites = JsonSerializer.Deserialize<List<UserMovie>>(jsonLiked);
            FiltredFavorites = favorites;
            await base.OnInitializedAsync();
        }
        protected string GetCompletedPosterPath(string posterPath)
        {
            return ImagePrefix + posterPath;
        }
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
            File.WriteAllText("wwwroot/data/viewed.json", jsonliked);
            Snackbar.Add("Movie from list sucessfully");
            StateHasChanged();
        }
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
