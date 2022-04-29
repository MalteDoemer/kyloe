using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Kyloe.Syntax
{
    public sealed class Lexer
    {
        private readonly Regex regex;
        
        private readonly Dictionary<string, SyntaxTokenKind> groupNames;
        
        private readonly HashSet<SyntaxTokenKind> discardTerminals;
        
        private readonly string text;
        
        private int pos;
        
        public Lexer(string text)
        {
            this.pos = 0;
            this.text = text;
            this.groupNames = new Dictionary<string, SyntaxTokenKind>();
            this.regex = new Regex(@"(?<Whitespace>\G\s+)|(?<LineComment>\G\/\/.*\n)|(?<BlockComment>\G\/\*.*\*\/)|(?<Comma>\G\,)|(?<Dot>\G\.)|(?<Colon>\G\:)|(?<SemiColon>\G\;)|(?<SmallArrow>\G\-\>)|(?<LessEqual>\G\<\=)|(?<Less>\G\<)|(?<GreaterEqual>\G\>\=)|(?<Greater>\G\>)|(?<DoubleEqual>\G\=\=)|(?<NotEqual>\G\!\=)|(?<PlusEqual>\G\+\=)|(?<MinusEqual>\G\-\=)|(?<StarEqual>\G\*\=)|(?<SlashEqual>\G\/\=)|(?<PercentEqual>\G\%\=)|(?<AmpersandEqual>\G\&\=)|(?<PipeEqual>\G\|\=)|(?<HatEqual>\G\^\=)|(?<Equal>\G\=)|(?<Plus>\G\+)|(?<Minus>\G\-)|(?<Star>\G\*)|(?<Slash>\G\/)|(?<Percent>\G\%)|(?<DoubleAmpersand>\G\&\&)|(?<Ampersand>\G\&)|(?<DoublePipe>\G\|\|)|(?<Pipe>\G\|)|(?<Hat>\G\^)|(?<Tilde>\G\~)|(?<Bang>\G\!)|(?<LeftParen>\G\()|(?<RightParen>\G\))|(?<LeftSquare>\G\[)|(?<RightSquare>\G\])|(?<LeftCurly>\G\{)|(?<RightCurly>\G\})|(?<Float>\G\b\d+\.\d+)|(?<Int>\G\b\d+\b)|(?<Bool>\G\b(true|false)\b)|(?<String>\G(\"".*\""|\u0027.*\u0027))|(?<VarKeyword>\G\bvar\b)|(?<ConstKeyword>\G\bconst\b)|(?<IfKeyword>\G\bif\b)|(?<ElseKeyword>\G\belse\b)|(?<FuncKeyword>\G\bfunc\b)|(?<Identifier>\G\b[a-zA-Z_]([a-zA-Z_]|\d)*\b)", RegexOptions.Compiled | RegexOptions.Multiline);
            
            var names = Enum.GetNames<SyntaxTokenKind>();
            var values = Enum.GetValues<SyntaxTokenKind>();
            for (int i = 0; i < names.Length; i++)
            {
                groupNames.Add(names[i], values[i]);
            }
            
            this.discardTerminals = new HashSet<SyntaxTokenKind>();
            this.discardTerminals.Add(SyntaxTokenKind.Whitespace);
            this.discardTerminals.Add(SyntaxTokenKind.LineComment);
            this.discardTerminals.Add(SyntaxTokenKind.BlockComment);
        }
        
        public IEnumerable<SyntaxTerminal> AllTerminals()
        {
            while (pos < text.Length)
            {
                var match = regex.Match(text, pos);
                if (match.Success)
                {
                    var group = match.Groups.OfType<System.Text.RegularExpressions.Group>().Where(g => g.Success).Last();
                    var location = Kyloe.Utility.SourceLocation.FromLength(match.Index, match.Length);
                    var terminal = new SyntaxTerminal(groupNames[group.Name], match.Value, location);
                    pos += location.Length;
                    yield return terminal;
                }
                else 
                {
                    var location = Kyloe.Utility.SourceLocation.FromLength(pos, 1);
                    var terminal = new SyntaxTerminal(SyntaxTokenKind.Error, text[pos].ToString(), location);
                    pos += 1;
                    yield return terminal;
                }
            }
            yield return new SyntaxTerminal(SyntaxTokenKind.End, "<end>", Kyloe.Utility.SourceLocation.FromLength(pos, 0));
        }
        
        public IEnumerable<SyntaxTerminal> Terminals()
        {
            return AllTerminals().Where(t => !discardTerminals.Contains(t.Kind));
        }
    }
}
