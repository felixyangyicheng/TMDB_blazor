using System.Text.Json;
using TMDB_blazor.Contracts;
using TMDB_blazor.Data;

namespace TMDB_blazor.Services
{
    public class JsonFileRepository:IJsonFileRepository
    {
        public JsonFileRepository()
        {

        }

        public void Save(List<UserMovie> list, string jsonPath)
        {
            string content = JsonSerializer.Serialize(list);

            if (list.Count == 1)
            {
                content = "[" + content + "]";
            }
            File.WriteAllText(jsonPath, content);
        }



        public List<UserMovie> ReadAll(string jsonPath)
        {
          
           return JsonSerializer.Deserialize<List<UserMovie>>(File.ReadAllText(jsonPath));
        }
    }
}
