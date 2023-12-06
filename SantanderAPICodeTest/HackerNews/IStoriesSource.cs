namespace SantanderAPICodeTest.HackerNews
{
    public interface IStoriesSource
    {
        Task<int> UpdateHackerAPIBestStoriesAsync(CancellationToken cancellationToken = default);

        Task<int> AddHackerAPIBestStoriesAsync(CancellationToken cancellationToken = default);

        IEnumerable<Story> GetStorySnapshot(CancellationToken cancellationToken = default);
    }
}
