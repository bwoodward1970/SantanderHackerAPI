using System.Text.Json;

namespace SantanderAPICodeTest.HackerNews
{
    public class StoriesHttpAPISource : IStoriesHttpAPISource
    {               
        private readonly ILogger<StoriesHttpAPISource> _logger;
        private HttpClient _httpClient = new HttpClient();
        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
        { PropertyNameCaseInsensitive = true };

        public StoriesHttpAPISource(ILogger<StoriesHttpAPISource> logger)
        {
            jsonSerializerOptions.Converters.Add(new DateTimeConverter());
            _logger = logger;
        }

        public async Task<T?> ExecuteHackerStoryAPICall<T>(string request, CancellationToken cancellationToken = default)
        {
            var stream = await GetStreamAsync(request);
            
            return await JsonSerializer.DeserializeAsync<T>(stream, jsonSerializerOptions, cancellationToken);
        }

        protected virtual async Task<Stream> GetStreamAsync(string request, CancellationToken cancellationToken = default) 
            => await _httpClient.GetStreamAsync(request);
    }
}
