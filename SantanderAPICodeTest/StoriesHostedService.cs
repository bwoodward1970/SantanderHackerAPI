using System.Collections.Immutable;
using System.Threading;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using SantanderAPICodeTest.HackerNews;
using SantanderAPICodeTest.Model;

namespace SantanderAPICodeTest
{
    public class StoriesHostedService : BackgroundService
    {
        private readonly ILogger<StoriesHostedService> _logger;
        private readonly IStoriesSource _storiesSource;
        private int _executionCount;
        

        public StoriesHostedService(IStoriesSource storiesSource, ILogger<StoriesHostedService> logger)
        {
            _logger = logger;
            _storiesSource = storiesSource;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Stories Hosted Service running.");

            await InitializeStories(stoppingToken);
            
            using PeriodicTimer timer = new(TimeSpan.FromSeconds(10));
            
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await UpdateStories(stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Stories Hosted Service is stopping.");
            }
        }

        private async Task<int> UpdateStories(CancellationToken cancellationToken)
        {
            int count = Interlocked.Increment(ref _executionCount);

            _logger.LogInformation("Timed Hosted Service is working. Count: {0}", count);

            int updatedCount = await _storiesSource.UpdateHackerAPIBestStoriesAsync(cancellationToken);

            _logger.LogInformation("Updated Stories Count: {0}", updatedCount);

            return updatedCount;

        }

        private async Task<int> InitializeStories(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Intializing Stories");

            int storyCount = await _storiesSource.AddHackerAPIBestStoriesAsync(cancellationToken);

            _logger.LogInformation("Added Stories Count: {0}", storyCount);

            return storyCount;
        }
         
    }

}
