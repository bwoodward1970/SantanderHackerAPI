using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using SantanderAPICodeTest.HackerNews;

namespace SantanderAPICodeTest.Test
{
    public class StoriesCacheTests
    {
        [Theory]
        [InlineData(1, false)]
        [InlineData(150, true)]
        [Description("Cache Expiries correctly expiring")]
        public void AssignStoriesCacheTimingTests(int storyCount, bool expectedExists)
        {
            var testCache = new MemoryCache(new MemoryCacheOptions());

            var storiesSourceMock = new Mock<IStoriesSource>();

            storiesSourceMock.Setup(x => x.GetStorySnapshot(default)).Returns(GetStories(storyCount));

            var storiesCache = new StoriesCache(Mock.Of<ILogger<StoriesCache>>(), 
                testCache, storiesSourceMock.Object);

            var storiesList = storiesCache.AssignStoriesList();

            Assert.Equal(storyCount, storiesList.Count);

            Thread.Sleep(1000);

            bool exists = testCache.TryGetValue(StoriesCache.StoriesCacheKey, out var result);

            Assert.Equal(expectedExists, exists);
        }

        private IEnumerable<Story> GetStories(int number)
        {
            var stories = new List<Story>(number);

            for(int i = 0; i < number; i++) 
            {
                stories.Add(GetStory());
            }

            return stories;
        }

        private Story GetStory() => new Story("Tiny volumetric display",
                "https://mitxela.com/projects/candle",
                "ttesmer", new DateTime(1970, 01, 20, 16, 38, 39, 785), 1561, 174);
    }
}
