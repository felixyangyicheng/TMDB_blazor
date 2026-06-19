using MudBlazor;

namespace TMDB_blazor.Data
{
    /// <summary>
    /// Thème moderne sombre — style terminal Reasonix
    /// </summary>
    public static class TMDBTheme
    {
        public static MudTheme Create() => new()
        {
            PaletteDark = new PaletteDark
            {
                Primary = "#14B8A6",
                Secondary = "#6366F1",
                Tertiary = "#A78BFA",
                AppbarBackground = "#0B1120",
                AppbarText = "#E2E8F0",
                Background = "#0D1321",
                Surface = "#111827",
                TextPrimary = "#F1F5F9",
                TextSecondary = "#94A3B8",
                ActionDefault = "#94A3B8",
                ActionDisabled = "#334155",
                Divider = "#1E293B",
                TableLines = "#1E293B",
                TableStriped = "#0F172A",
                Error = "#EF4444",
                Info = "#3B82F6",
                Success = "#22C55E",
                Warning = "#F59E0B",
                Dark = "#020617",
            },
            LayoutProperties = new LayoutProperties
            {
                DefaultBorderRadius = "8px",
            },
        };
    }
}
