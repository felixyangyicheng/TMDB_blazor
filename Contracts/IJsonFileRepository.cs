using TMDB_blazor.Data;

namespace TMDB_blazor.Contracts
{
    public interface IJsonFileRepository
    {
        List<UserMovie> ReadAll(string jsonPath);
        void Save (List<UserMovie> list, string jsonPath);
    
    }
}
