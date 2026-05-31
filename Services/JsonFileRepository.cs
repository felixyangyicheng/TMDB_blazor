using System.Text.Json;

namespace TMDB_blazor.Services
{
    public class JsonFileRepository : IJsonFileRepository
    {
        // Simple in-memory cache: path -> (lastWriteTime, data)
        private readonly Dictionary<string, (DateTime lastWrite, List<UserMovie> data)> _cache = new();

        public async Task<List<UserMovie>> ReadAllAsync(string jsonPath)
        {
            var fileInfo = new FileInfo(jsonPath);
            if (!fileInfo.Exists)
                return new List<UserMovie>();

            DateTime lastWrite = fileInfo.LastWriteTimeUtc;

            // Return cached data if file hasn't changed
            if (_cache.TryGetValue(jsonPath, out var cached) && cached.lastWrite == lastWrite)
                return cached.data;

            // Read from disk, cache, and return
            string json = await File.ReadAllTextAsync(jsonPath);
            var data = JsonSerializer.Deserialize<List<UserMovie>>(json) ?? new List<UserMovie>();
            _cache[jsonPath] = (lastWrite, data);
            return data;
        }

        public async Task SaveAsync(List<UserMovie> list, string jsonPath)
        {
            string content = JsonSerializer.Serialize(list);
            await File.WriteAllTextAsync(jsonPath, content);

            // Invalidate cache — next read will refresh
            _cache.Remove(jsonPath);
        }
    }
}
