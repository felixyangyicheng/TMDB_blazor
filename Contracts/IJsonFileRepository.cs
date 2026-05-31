using TMDB_blazor.Data;

namespace TMDB_blazor.Contracts
{
    public interface IJsonFileRepository
    {
        Task<List<UserMovie>> ReadAllAsync(string jsonPath);
        Task SaveAsync(List<UserMovie> list, string jsonPath);
    }
}
