namespace SantanderAPICodeTest.HackerNews
{
    public class StoriesSourceException : Exception
    {
        public StoriesSourceException() { }

        public StoriesSourceException(string message) : base (message) { }

        public StoriesSourceException (string message, Exception innerException) : base (message, innerException) { }

    }
}
