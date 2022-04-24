namespace Kyloe.Grammar
{
    public struct ParserGeneratorInfo
    {
        public string TokenKindEnumName;
        public string ExtensionClassName;
        public string TokenClassName;
        public string TerminalClassName;
        public string NodeClassName;
        public string LocationClassName;
        public string ErrorClassName;
        public string ErrorKindEnumName;
        public string LexerClassName;
        public string ParserClassName;

        public ParserGeneratorInfo(
            string tokenKindEnumName = "TokenKind",
            string extensionClassName = "TokenKindExtension",
            string tokenClassName = "Token",
            string terminalClassName = "Terminal",
            string nodeClassName = "Node",
            string locationClassName = "TextLocation",
            string errorClassName = "Error",
            string errorKindEnumName = "ErrorKind",
            string lexerClassName = "Lexer",
            string parserClassName = "Parser")
        {
            TokenKindEnumName = tokenKindEnumName;
            ExtensionClassName = extensionClassName;
            TokenClassName = tokenClassName;
            TerminalClassName = terminalClassName;
            NodeClassName = nodeClassName;
            LocationClassName = locationClassName;
            ErrorClassName = errorClassName;
            ErrorKindEnumName = errorKindEnumName;
            LexerClassName = lexerClassName;
            ParserClassName = parserClassName;
        }
    }
}