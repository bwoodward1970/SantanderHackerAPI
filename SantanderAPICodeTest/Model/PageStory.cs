namespace SantanderAPICodeTest.Model
{
    public record PageStory(string Title,
        string Uri,
        string PostedBy,
        DateTime Time,
        int Score,
        int CommentCount
        );
}
