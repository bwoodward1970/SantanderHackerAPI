namespace SantanderAPICodeTest.HackerNews
{
    public interface IStoriesHttpAPISource
    {
        public Task<T?> ExecuteHackerStoryAPICall<T>(string request, CancellationToken cancellationToken = default);
    }
}
