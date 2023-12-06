using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SantanderAPICodeTest;
using SantanderAPICodeTest.HackerNews;
using SantanderAPICodeTest.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IBackgroundTaskQueue>(new StoryBackgroundTaskQueue(500));
builder.Services.AddSingleton<IStoriesHttpAPISource, StoriesHttpAPISource>();
builder.Services.AddSingleton<IStoriesSource, StoriesSource>();
builder.Services.AddSingleton<IStoriesCache, StoriesCache>();
builder.Services.AddHostedService<StoriesHostedService>();
builder.Services.AddHostedService<StoryQueuedHostedService>();

builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.WriteIndented = true;
    options.SerializerOptions.IncludeFields = true;
});

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseStatusCodePages(async statusCodeContext
    => await Results.Problem(statusCode: statusCodeContext.HttpContext.Response.StatusCode)
                 .ExecuteAsync(statusCodeContext.HttpContext));

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();


app.MapGet("/besthackernews", async Task<Ok<IEnumerable<PageStory>>> 
    (IStoriesCache storiesCache,
    [FromQuery] int? numStories,
    CancellationToken cancellationToken) =>
{
    var stories = storiesCache.GetStories();

    int storyCount = numStories ?? stories.Count;

    return TypedResults.Ok(stories.Take(storyCount));

});

app.Run();

