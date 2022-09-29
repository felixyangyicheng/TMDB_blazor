using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;

namespace TMDB_blazor.Data
{
    public class UserMovie:SearchMovie
    {
        public bool? Favorite { get; set; }
        public bool? Viewed { get; set; }
    }
}
