using System;
using System.Linq;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Kyloe.Syntax
{
    internal sealed class Lexer
    {
        private readonly ImmutableArray<(SyntaxTokenKind, string, Regex?)> patterns;
        
        private readonly HashSet<SyntaxTokenKind> discardTerminals;
        
        private readonly string text;
        
        private int pos;
        
        public Lexer(string text)
        {
            this.pos = 0;
            this.text = text;
            
            var builder = ImmutableArray.CreateBuilder<(SyntaxTokenKind, string, Regex?)>(56);
            builder.Add((SyntaxTokenKind.Whitespace, string.Empty , new Regex(@"\G\s+", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.LineComment, string.Empty , new Regex(@"\G\/\/.*\n", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.BlockComment, string.Empty , new Regex(@"\G\/\*.*\*\/", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.Comma, @"," , null));
            builder.Add((SyntaxTokenKind.Dot, @"." , null));
            builder.Add((SyntaxTokenKind.Colon, @":" , null));
            builder.Add((SyntaxTokenKind.SemiColon, @";" , null));
            builder.Add((SyntaxTokenKind.SmallArrow, @"->" , null));
            builder.Add((SyntaxTokenKind.LessEqual, @"<=" , null));
            builder.Add((SyntaxTokenKind.Less, @"<" , null));
            builder.Add((SyntaxTokenKind.GreaterEqual, @">=" , null));
            builder.Add((SyntaxTokenKind.Greater, @">" , null));
            builder.Add((SyntaxTokenKind.DoubleEqual, @"==" , null));
            builder.Add((SyntaxTokenKind.NotEqual, @"!=" , null));
            builder.Add((SyntaxTokenKind.PlusEqual, @"+=" , null));
            builder.Add((SyntaxTokenKind.MinusEqual, @"-=" , null));
            builder.Add((SyntaxTokenKind.StarEqual, @"*=" , null));
            builder.Add((SyntaxTokenKind.SlashEqual, @"/=" , null));
            builder.Add((SyntaxTokenKind.PercentEqual, @"%=" , null));
            builder.Add((SyntaxTokenKind.AmpersandEqual, @"&=" , null));
            builder.Add((SyntaxTokenKind.PipeEqual, @"|=" , null));
            builder.Add((SyntaxTokenKind.HatEqual, @"^=" , null));
            builder.Add((SyntaxTokenKind.Equal, @"=" , null));
            builder.Add((SyntaxTokenKind.Plus, @"+" , null));
            builder.Add((SyntaxTokenKind.Minus, @"-" , null));
            builder.Add((SyntaxTokenKind.Star, @"*" , null));
            builder.Add((SyntaxTokenKind.Slash, @"/" , null));
            builder.Add((SyntaxTokenKind.Percent, @"%" , null));
            builder.Add((SyntaxTokenKind.DoubleAmpersand, @"&&" , null));
            builder.Add((SyntaxTokenKind.Ampersand, @"&" , null));
            builder.Add((SyntaxTokenKind.DoublePipe, @"||" , null));
            builder.Add((SyntaxTokenKind.Pipe, @"|" , null));
            builder.Add((SyntaxTokenKind.Hat, @"^" , null));
            builder.Add((SyntaxTokenKind.Tilde, @"~" , null));
            builder.Add((SyntaxTokenKind.Bang, @"!" , null));
            builder.Add((SyntaxTokenKind.LeftParen, @"(" , null));
            builder.Add((SyntaxTokenKind.RightParen, @")" , null));
            builder.Add((SyntaxTokenKind.LeftSquare, @"[" , null));
            builder.Add((SyntaxTokenKind.RightSquare, @"]" , null));
            builder.Add((SyntaxTokenKind.LeftCurly, @"{" , null));
            builder.Add((SyntaxTokenKind.RightCurly, @"}" , null));
            builder.Add((SyntaxTokenKind.Float, string.Empty , new Regex(@"\G\b\d+\.\d+", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.Int, string.Empty , new Regex(@"\G\b\d+\b", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.Bool, string.Empty , new Regex(@"\G\b(true|false)\b", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.String, string.Empty , new Regex(@"\G(\"".*\""|\u0027.*\u0027)", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.VarKeyword, string.Empty , new Regex(@"\G\bvar\b", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.ConstKeyword, string.Empty , new Regex(@"\G\bconst\b", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.FuncKeyword, string.Empty , new Regex(@"\G\bfunc\b", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.IfKeyword, string.Empty , new Regex(@"\G\bif\b", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.ElseKeyword, string.Empty , new Regex(@"\G\belse\b", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.ElifKeyword, string.Empty , new Regex(@"\G\belif\b", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.WhileKeyword, string.Empty , new Regex(@"\G\bwhile\b", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.BreakKeyword, string.Empty , new Regex(@"\G\bbreak\b", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.ContinueKeyword, string.Empty , new Regex(@"\G\bcontinue\b", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.ReturnKeyword, string.Empty , new Regex(@"\G\breturn\b", RegexOptions.Compiled | RegexOptions.Multiline)));
            builder.Add((SyntaxTokenKind.Identifier, string.Empty , new Regex(@"\G\b[a-zA-Z_]([a-zA-Z_]|\d)*\b", RegexOptions.Compiled | RegexOptions.Multiline)));
            this.patterns = builder.MoveToImmutable();
            
            this.discardTerminals = new HashSet<SyntaxTokenKind>();
            this.discardTerminals.Add(SyntaxTokenKind.Whitespace);
            this.discardTerminals.Add(SyntaxTokenKind.LineComment);
            this.discardTerminals.Add(SyntaxTokenKind.BlockComment);
        }
        
        public IEnumerable<SyntaxTerminal> AllTerminals()
        {
            while (pos < text.Length)
            {
                bool didMatch = false;
                foreach (var (kind, str, regex) in patterns)
                {
                    if (regex is null)
                    {
                        var leftover = text.Length - pos;
                        
                        if (str.Length > leftover) continue;
                        
                        var match = string.CompareOrdinal(text, pos, str, 0, str.Length);
                        
                        if (match != 0) continue;
                        
                        var location = Kyloe.Utility.SourceLocation.FromLength(pos, str.Length);
                        var terminal = new SyntaxTerminal(kind, str, location);
                        pos += location.Length;
                        didMatch = true;
                        yield return terminal;
                        break;
                    }
                    else 
                    {
                        var match = regex.Match(text, pos);
                        if (!match.Success) continue;
                        
                        var location = Kyloe.Utility.SourceLocation.FromLength(match.Index, match.Length);
                        var terminal = new SyntaxTerminal(kind, match.Value, location);
                        pos += location.Length;
                        didMatch = true;
                        yield return terminal;
                        break;
                    }
                }
                if (!didMatch)
                {
                    var errTerminal = new SyntaxTerminal(SyntaxTokenKind.Error, text[pos].ToString(), Kyloe.Utility.SourceLocation.FromLength(pos, 1));
                    pos += 1;
                    yield return errTerminal;
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
