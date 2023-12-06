using System;

namespace SantanderAPICodeTest.HackerNews
{
    public record Story(string Title, 
        string Url, 
        string By, 
        DateTime Time,
        int Score,
        int Descendants
        );
    
}
