using System.Text.Json;

namespace TMDB_blazor.Services
{
    public class TmdbExtension : ITmdbExtension
    {
        private readonly IJsonFileRepository _jsonFileRepository;
        private readonly TMDbClient DataClient;
        public TmdbExtension(IJsonFileRepository jsonFileRepository, TMDbClient dataClient)
        {
            _jsonFileRepository = jsonFileRepository;
            DataClient = dataClient;
        }

        public async Task<List<UserMovie>> SearcheWithLocalFilter(string searchWord, bool? viewedFilter, bool? likedFilter, bool adult)
        {
            // 1. Search TMDB API — first page only (pagination adds latency for minimal UI gain)
            var searchResult = await DataClient.SearchMovieAsync(searchWord, 1, adult);
            var upperWord = searchWord.ToUpper();

            // 2. Load local lists (async + cached via repository)
            var allViewed = await _jsonFileRepository.ReadAllAsync(Endpoints.jsonViewedPath);
            var allLiked = await _jsonFileRepository.ReadAllAsync(Endpoints.jsonLikedPath);

            var viewedIds = new HashSet<int>(allViewed.Select(x => x.Id));
            var likedIds = new HashSet<int>(allLiked.Select(x => x.Id));

            // 3. Build display list: merge HTTP results with local viewed/liked status
            var result = new List<UserMovie>();
            foreach (var item in searchResult.Results)
            {
                string json = JsonSerializer.Serialize(item);
                var um = JsonSerializer.Deserialize<UserMovie>(json)!;

                // Mark local status
                if (viewedIds.Contains(um.Id))
                {
                    var local = allViewed.First(v => v.Id == um.Id);
                    um.Viewed = local.Viewed;
                }
                if (likedIds.Contains(um.Id))
                {
                    var local = allLiked.First(v => v.Id == um.Id);
                    um.Favorite = local.Favorite;
                }

                result.Add(um);
            }

            // 4. Append local-only items (viewed/liked that match the search but aren't in HTTP results)
            var httpIds = new HashSet<int>(result.Select(x => x.Id));
            foreach (var item in allViewed.Where(a => a.Title.ToUpper().Contains(upperWord) && !httpIds.Contains(a.Id)))
                result.Add(item);
            foreach (var item in allLiked.Where(a => a.Title.ToUpper().Contains(upperWord) && !httpIds.Contains(a.Id)))
            {
                if (!result.Any(r => r.Id == item.Id))
                    result.Add(item);
            }

            // 5. Apply filters
            if (viewedFilter == false)
                result.RemoveAll(r => r.Viewed == true);
            if (likedFilter == false)
                result.RemoveAll(r => r.Favorite == true);

            // Sort by popularity (most relevant first)
            result = result.OrderByDescending(r => r.Popularity).ToList();

            return result;
        }
    }
}
