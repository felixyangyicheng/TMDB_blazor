using TMDB_blazor.Data;
using TMDbLib.Objects.Search;

namespace TMDB_blazor.Contracts
{
    public interface ITmdbExtension
    {
        Task<List<UserMovie>> SearcheWithLocalFilter(string searchWord,bool? viewed, bool? liked, bool adult);
    }
}
