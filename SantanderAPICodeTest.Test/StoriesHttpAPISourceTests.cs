using System.ComponentModel;
using System.Text;
using Microsoft.Extensions.Logging;
using Moq;
using SantanderAPICodeTest.HackerNews;

namespace SantanderAPICodeTest.Test
{
    public class StoriesHttpAPISourceTests
    {
        [Fact]
        [Description("The returned Story stream creates the correct Story Attributes")]
        public async void ExecuteHackerStoryAPICallForStoryTest()
        {
            using var storyStream = GetStoryStream();

            var storiesHttpAPISource = new TestStoriesHttpAPISource(storyStream, 
                                        Mock.Of<ILogger<StoriesHttpAPISource>>());

            var story = await storiesHttpAPISource
                .ExecuteHackerStoryAPICall<Story>(string.Format(StoriesAPIUrl.HackerAPIStoryUrl, 38429370), new CancellationToken());

            Assert.Equal(GetExpectedStory(), story);

        }

        private Story GetExpectedStory() => new Story("Tiny volumetric display",
                "https://mitxela.com/projects/candle",
                "ttesmer", new DateTime(1970, 01, 20, 16, 38, 39, 785), 1561, 174);


        private Stream GetStoryStream()
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(StoryStreamString);
            MemoryStream stream = new MemoryStream(byteArray);      

            return stream;

        }

        private string StoryStreamString = """{"by":"ttesmer","descendants":174,"id":38498109,"kids":[38500457,38498629,38501145,38498923,38500651,38502029,38499818,38498748,38498762,38500497,38502363,38501545,38505973,38499208,38502328,38506720,38500677,38499412,38503990,38503773,38499396,38501188,38502505,38499092,38499958,38499310,38500612,38498981,38503403,38498945,38503449,38499994,38499829,38506114,38500058,38506329,38507480,38504310,38499488,38498888,38499269,38498962,38500547,38499100,38498564,38501972,38500337,38509010,38500224,38502046],"score":1561,"time":1701519785,"title":"Tiny volumetric display","type":"story","url":"https://mitxela.com/projects/candle"}""";
    }
}
