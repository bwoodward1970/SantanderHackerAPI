using System.Collections.Immutable;
using Microsoft.Extensions.Caching.Memory;
using SantanderAPICodeTest.HackerNews;
using SantanderAPICodeTest.Model;

namespace SantanderAPICodeTest
{
    public class StoriesCache : IStoriesCache
    {
        private readonly ILogger<StoriesCache> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IStoriesSource _storiesSource;
        public const string StoriesCacheKey = "Stories";
        
        public StoriesCache(ILogger<StoriesCache> logger, IMemoryCache memoryCache, IStoriesSource storiesSource) 
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _storiesSource = storiesSource;
        }

        public IList<PageStory> GetStories() 
        {
            if (!_memoryCache.TryGetValue(StoriesCacheKey, out IList<PageStory> stories))
            {
                stories = AssignStoriesList();
            }

            return stories;
        }
                 
        public IList<PageStory> AssignStoriesList()
        {
            var stories = _storiesSource.GetStorySnapshot();

            var pageStories = stories.Select(s =>
                new PageStory(s.Title,
                s.Url,
                s.By,
                s.Time,
                s.Score,
                s.Descendants)).ToImmutableList();

            _logger.LogInformation("Assigning _stories list, count {0}", pageStories.Count);

            //increase the expiration to max of 10 in 2 second increments
            int pageStoriesDecimalCount = (int)(pageStories.Count * 0.1);

            int secondsExpiration = pageStoriesDecimalCount < 10 ? pageStoriesDecimalCount < 1 ? 1 : pageStoriesDecimalCount : 10;

            _memoryCache.Set(StoriesCacheKey, pageStories, TimeSpan.FromSeconds(secondsExpiration));

            return pageStories;
        }
}
}
