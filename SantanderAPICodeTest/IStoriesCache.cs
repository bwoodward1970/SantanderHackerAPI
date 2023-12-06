using SantanderAPICodeTest.Model;

namespace SantanderAPICodeTest
{
    public interface IStoriesCache
    {
        public IList<PageStory> GetStories();

        public IList<PageStory> AssignStoriesList();
    }
}
