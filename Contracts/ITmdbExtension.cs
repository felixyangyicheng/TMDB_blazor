namespace TMDB_blazor.Contracts
{
    public interface ITmdbExtension
    {
        Task<List<UserMovie>> SearcheWithLocalFilter(string searchWord,bool? viewed, bool? liked, bool adult);
    }
}
