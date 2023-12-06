using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using SantanderAPICodeTest.HackerNews;

namespace SantanderAPICodeTest.Test
{
    internal class TestStoriesHttpAPISource : StoriesHttpAPISource
    {
        private Stream _responseStream;

        public TestStoriesHttpAPISource(Stream responseStream, ILogger<StoriesHttpAPISource> logger) : base(logger)
        {  
            _responseStream = responseStream;
        }

        protected override Task<Stream> GetStreamAsync(string request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_responseStream);
        }


    }
}
