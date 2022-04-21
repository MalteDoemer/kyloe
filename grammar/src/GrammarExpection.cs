namespace Kyloe.Grammar
{
    public class GrammarException : System.Exception
    {
        public GrammarException(string message) : base(message) { }
        public GrammarException(string message, GrammarLocation location) : base($"{location}: {message}") { }
    }
}