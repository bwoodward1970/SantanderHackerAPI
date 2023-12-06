using System.ComponentModel;
using Microsoft.Extensions.Logging;
using Moq;
using SantanderAPICodeTest.HackerNews;

namespace SantanderAPICodeTest.Test
{
    public class StoriesSourceTests
    {
        [Fact]
        [Description("Best Stories Request Returns 200 Story Ids")]
        public async void BestStoryIdsTest()
        {
            var storiesSource = CreateStoriesSource();

            var result = await storiesSource.GetHackerAPIBestStoryIdsAsync();

            Assert.Equal(200, result.Length); 
        }

        [Fact]
        [Description("Updated Stories Request returns Updated Stories")]
        public async void ChangedStoriesTest()
        {
            var storiesSource = CreateStoriesSource();

            var result = await storiesSource.GetHackerChangedStoriesAsync();

            Assert.True(result.Length > 0);

        }

        [Fact]
        [Description("There are no Updated Stories and no new Stories are returned to Process")]
        public void GetUpdatedStoryIdsNoChangesTest()
        {
            var storiesSource = CreateStoriesSource();

            int[] existingStoryIds = GetExistingStoryIds;
            
            int[] apiStoryIds = GetAPIStoryIds;
            
            int[] changedStoryIds = new int[] { 38408921 };

            var result = storiesSource.GetUpdatedStoryIds(existingStoryIds, apiStoryIds, changedStoryIds);

            Assert.Equal(Array.Empty<int>(), result);

        }

        [Fact]
        [Description("There are Updated Stories to Process")]
        public void GetUpdatedStoryIdsOneChangeTest()
        {
            var storiesSource = CreateStoriesSource();

            int[] existingStoryIds = GetExistingStoryIds;

            int[] apiStoryIds = GetAPIStoryIds;

            int[] changedStoryIds = new int[] { 38408920 };

            var result = storiesSource.GetUpdatedStoryIds(existingStoryIds, apiStoryIds, changedStoryIds);

            Assert.Equal(new int[] { 38408920 }, result);

        }

        private StoriesSource CreateStoriesSource()
        {
            var backgroundTaskQueue = new StoryBackgroundTaskQueue(500);

            var storiesHttpAPISource = new StoriesHttpAPISource(Mock.Of<ILogger<StoriesHttpAPISource>>());

            var storiesSource = new StoriesSource(Mock.Of<ILogger<StoriesSource>>(), storiesHttpAPISource, backgroundTaskQueue);

            return storiesSource;       
        
        }

        private int[] GetExistingStoryIds => new int[] { 38435908, 38429370, 38429291, 38427864, 38434613, 38405823, 38415252, 38408920 };

        private int[] GetAPIStoryIds => new int[] { 38435908, 38429370, 38429291, 38427864, 38434613, 38405823, 38415252, 38408920 };

    }
}