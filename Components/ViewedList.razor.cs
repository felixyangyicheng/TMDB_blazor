using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json;
using TMDB_blazor.Data;

namespace TMDB_blazor.Components
{
    public partial class ViewedList
    {
        /// <summary>
        /// injection service Snackbar
        /// </summary>
        [Inject] ISnackbar Snackbar { get; set; }
        public List<UserMovie> viewed { get; set; }
        public List<UserMovie> FiltredViewed { get; set; }
   
        /// <summary>
        /// uri de l'image.
        /// </summary>
        public string ImagePrefix { get; set; } = Endpoints.ImagePathPrefix;
        protected override async Task OnInitializedAsync()
        {
            string jsonViewed = File.ReadAllText("wwwroot/data/viewed.json");
            viewed = JsonSerializer.Deserialize<List<UserMovie>>(jsonViewed);
            FiltredViewed = viewed;
            await base.OnInitializedAsync();
        }
        protected string GetCompletedPosterPath(string posterPath)
        {
            return ImagePrefix + posterPath;
        }
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
            StateHasChanged();
        }
        protected void SearchChanged(string el)
        {
            FiltredViewed = viewed.Where(a => (a.Title.ToUpper().Contains(el.ToUpper())
                                            || a.OriginalTitle.ToUpper().Contains(el.ToUpper())
                                            || ((DateTime)a.ReleaseDate).ToString("d").Contains(el)

                                            )).ToList();

            StateHasChanged();
        }
    }
}
