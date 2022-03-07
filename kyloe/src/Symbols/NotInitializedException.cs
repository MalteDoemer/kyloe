namespace Symobls
{
    public class NotInitializedException : System.Exception
    {
        public NotInitializedException(string field) : base($"The field '{field}' is not initialized") { }
        public NotInitializedException(string field, string message) : base($"The field '{field}' is not initialized: {message}") { }
    }
}