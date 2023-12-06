using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Text.Json;
using System.Threading;
using System.ComponentModel;

namespace SantanderAPICodeTest.HackerNews
{
    public class StoriesSource : IStoriesSource
    {
        private readonly ILogger<StoriesSource> _logger;
        private readonly IBackgroundTaskQueue _storyBackgroundTaskQueue;
        private readonly IStoriesHttpAPISource _storiesHttpAPISource;
        private ConcurrentDictionary<int, Story> _stories = new ConcurrentDictionary<int, Story>();

        public StoriesSource(ILogger<StoriesSource> logger, IStoriesHttpAPISource storiesHttpAPISource, IBackgroundTaskQueue storyBackgroundTaskQueue)
        {
            _logger  = logger;
            _storyBackgroundTaskQueue = storyBackgroundTaskQueue;
            _storiesHttpAPISource = storiesHttpAPISource;
        }
                
        public async Task<int[]?> GetHackerAPIBestStoryIdsAsync(CancellationToken cancellationToken = default) => 
            await _storiesHttpAPISource.ExecuteHackerStoryAPICall<int[]>(StoriesAPIUrl.HackerAPIStoriesUrl, cancellationToken);
  

        public async Task<int> AddHackerAPIBestStoriesAsync(CancellationToken cancellationToken = default) =>
            await UpdateHackerAPIStoriesAsync(GetHackerAPIBestStoryIdsAsync, cancellationToken);
        
        //10 second delay
        public async Task<int[]?> GetHackerChangedStoriesAsync(CancellationToken cancellationToken = default)
        {
            var updates = await _storiesHttpAPISource.ExecuteHackerStoryAPICall<Updates>(StoriesAPIUrl.HackerChangedStoriesUrl, cancellationToken);

            return updates?.Items;
        }

        public async ValueTask AddHackerStoryAsync(int storyId, CancellationToken cancellationToken = default)
        {
            try
            {
                var story = await _storiesHttpAPISource.ExecuteHackerStoryAPICall<Story>(string.Format(StoriesAPIUrl.HackerAPIStoryUrl, storyId), cancellationToken);

                if (story != null)
                {
                    _stories.AddOrUpdate(storyId, story, (k, v) => story);
                }

            }
            catch (Exception ex) 
            {
                _logger.LogError("Unable to add Story", ex);
                
                throw new StoriesSourceException("Unable to add Story", ex);
            }
           
        }
               

        public async Task<int> UpdateHackerAPIBestStoriesAsync(CancellationToken cancellationToken = default) =>
            await UpdateHackerAPIStoriesAsync(GetUpdatedStoryIdsAsync, cancellationToken);
        
        private async Task<int> UpdateHackerAPIStoriesAsync(Func<CancellationToken,Task<int[]?>> apiCallMethod, CancellationToken cancellationToken = default)
        {
            try
            {
                var storyIds = await apiCallMethod(cancellationToken);

                _logger.LogInformation("Adding Story ids for: {0}", apiCallMethod.Method.Name);

                foreach (var storyId in storyIds)
                {
                    await _storyBackgroundTaskQueue.QueueBackgroundWorkItemAsync((ct) => AddHackerStoryAsync(storyId, ct));
                }

                return storyIds.Count();

            }
            catch(Exception ex)
            {
                string msg = $"Unable to Update Story for {apiCallMethod.Method.Name} method";

                _logger.LogError(msg, ex);

                throw new StoriesSourceException(msg, ex);
            }
        }

        public async Task<int[]?> GetUpdatedStoryIdsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var storyIds = await GetHackerAPIBestStoryIdsAsync(cancellationToken);

                var changedStoryIds = await GetHackerChangedStoriesAsync(cancellationToken);

                var existingStoryIds = _stories.Select(s => s.Key).ToArray();

                return GetUpdatedStoryIds(existingStoryIds, storyIds, changedStoryIds);

            }
            catch (Exception ex)
            {
                string msg = "Getting Updated Stories failed.";

                _logger.LogError(msg, ex);

                throw new StoriesSourceException(msg, ex);
            }
        }

        public int[] GetUpdatedStoryIds(int[] existingStoryIds, int[] storyIds, int[] changedStoryIds)
        {
            var filteredChangedStoryIds = changedStoryIds.Intersect(storyIds);

            var newStoryIds = storyIds.Except(existingStoryIds);

            var toBeProcessed = new HashSet<int>(filteredChangedStoryIds);

            toBeProcessed.UnionWith(newStoryIds);

            return toBeProcessed.ToArray();

        }

        public IEnumerable<Story> GetStorySnapshot(CancellationToken cancellationToken = default) => 
            _stories.OrderByDescending(s => s.Value.Score).Select(s => s.Value);

    }
}
