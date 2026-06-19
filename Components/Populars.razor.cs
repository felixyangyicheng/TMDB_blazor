using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Diagnostics.CodeAnalysis;
using TMDB_blazor.Data;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace TMDB_blazor.Components
{
    public partial class Populars
    {
        [Inject, NotNull] TMDbClient DataClient { get; set; } = default!;
        [Parameter] public EventCallback<SearchMovie> OnSelectItem { get; set; }

        public string ImagePrefix { get; set; } = Endpoints.ImagePathPrefix;
        public string AlternativeImage { get; set; } = Endpoints.AlternativeImage;
        public SearchContainer<SearchMovie> SearchContainer { get; set; } = new();

        private MudCarousel<string> _carousel = new();
        private bool _arrows = true;
        private bool _enableSwipeGesture = true;
        public bool _autocycle = false;
        private IList<string> _source = new List<string>();
        private int selectedIndex = 0;

        protected override async Task OnInitializedAsync()
        {
            SearchContainer = await DataClient.GetMoviePopularListAsync() ?? new();
            if (SearchContainer?.Results is not null)
            {
                foreach (var i in SearchContainer.Results)
                {
                    if (i.Title is not null)
                        _source.Add(i.Title);
                }
            }
            await base.OnInitializedAsync();
        }

        protected string GetCompletedPosterPath(string? posterPath)
        {
            return string.IsNullOrEmpty(posterPath) ? AlternativeImage : ImagePrefix + posterPath;
        }

        protected async Task ClickMovie(int id)
        {
            if (SearchContainer?.Results is not null && id < SearchContainer.Results.Count)
            {
                await OnSelectItem.InvokeAsync(SearchContainer.Results[id]);
                StateHasChanged();
            }
        }
    }
}
