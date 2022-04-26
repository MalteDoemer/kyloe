using System.Linq;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace Kyloe.Syntax
{
    public sealed class Parser
    {
        private readonly ImmutableArray<SyntaxTerminal> terminals;
        
        private readonly HashSet<SyntaxTokenKind> stopTerminals;
        
        private readonly ICollection<Kyloe.Diagnostics.Diagnostic> errors;
        
        private int pos;
        
        private bool isValid;
        
        private SyntaxTerminal current => pos < terminals.Length ? terminals[pos] : terminals[terminals.Length - 1];
        
        public Parser(string text, ICollection<Kyloe.Diagnostics.Diagnostic> errors)
        {
            this.pos = 0;
            this.isValid = true;
            this.errors = errors;
            var lexer = new Lexer(text);
            var builder = ImmutableArray.CreateBuilder<SyntaxTerminal>();
            foreach (var terminal in lexer.Terminals())
            {
                if (terminal.Kind == SyntaxTokenKind.Error)
                {
                    errors.Add(new Kyloe.Diagnostics.Diagnostic(Kyloe.Diagnostics.DiagnosticKind.InvalidCharacterError, string.Format("invalid character: \\u{0:x4}", (int)(terminal.Text[0])), terminal.Location));
                }
                else 
                {
                    builder.Add(terminal);
                }
            }
            this.terminals = builder.ToImmutable();
            this.stopTerminals = new HashSet<SyntaxTokenKind>();
            this.stopTerminals.Add(SyntaxTokenKind.SemiColon);
            this.stopTerminals.Add(SyntaxTokenKind.LeftCurly);
            this.stopTerminals.Add(SyntaxTokenKind.RightCurly);
            this.stopTerminals.Add(SyntaxTokenKind.End);
        }
        
        private SyntaxTerminal Advance()
        {
            var temp = current;
            pos += 1;
            isValid = true;
            return temp;
        }
        
        private SyntaxToken? Expect(SyntaxTokenKind expected, params SyntaxTokenKind[] next)
        {
            if (current.Kind == expected) return Advance();
            Unexpected(expected);
            var forged = new SyntaxTerminal(expected, current.Text, current.Location, invalid: true);
            if (next.Length != 0) SkipInput(next);
            return forged;
        }
        
        private void SkipInput(params SyntaxTokenKind[] next)
        {
            var nextSet = next.ToHashSet();
            while (!next.Contains(current.Kind) && ! stopTerminals.Contains(current.Kind))
            {
                pos += 1;
            }
        }
        
        private void Unexpected(params SyntaxTokenKind[] expected)
        {
            if (!isValid)
            {
                return;
            }
            isValid = false;
            string msg;
            if (expected.Length == 1)
            {
                msg = $"expected {expected[0]}, got {current.Kind}";
            }
            else 
            {
                msg = $"expected one of ({string.Join(", ", expected)}), got {current.Kind}";
            }
            errors.Add(new Kyloe.Diagnostics.Diagnostic(Kyloe.Diagnostics.DiagnosticKind.UnexpectedTokenError, msg, current.Location));
        }
        
        private SyntaxToken? CreateNode(SyntaxTokenKind kind, params SyntaxToken?[] tokens)
        {
            var arr = tokens.ToImmutableArray();
            if (arr.Length == 0) return null;
            else if (arr.Length == 1) return arr[0];
            else return new SyntaxNode(kind, arr);
        }
        
        public SyntaxToken? Parse()
        {
            var token = ParseStart();
            Expect(SyntaxTokenKind.End);
            return token;
        }
        
        private SyntaxToken? ParseStart()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.FuncKeyword:
                case SyntaxTokenKind.VarKeyword:
                case SyntaxTokenKind.ConstKeyword:
                {
                    var n0 = ParseCompilationUnit();
                    return CreateNode(SyntaxTokenKind.Start, n0);
                }
                default: return null;
            }
        }
        
        private SyntaxToken? ParseStop()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.SemiColon:
                {
                    var n0 = Advance();
                    return CreateNode(SyntaxTokenKind.Stop, n0);
                }
                case SyntaxTokenKind.LeftCurly:
                {
                    var n0 = Advance();
                    return CreateNode(SyntaxTokenKind.Stop, n0);
                }
                case SyntaxTokenKind.RightCurly:
                {
                    var n0 = Advance();
                    return CreateNode(SyntaxTokenKind.Stop, n0);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.SemiColon, SyntaxTokenKind.LeftCurly, SyntaxTokenKind.RightCurly);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseCompilationUnit()
        {
            switch (current.Kind)
            {
                default:
                {
                    SyntaxToken? node = null;
                    while (current.Kind == SyntaxTokenKind.FuncKeyword || current.Kind == SyntaxTokenKind.VarKeyword || current.Kind == SyntaxTokenKind.ConstKeyword || current.Kind == SyntaxTokenKind.End)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.FuncKeyword:
                            case SyntaxTokenKind.VarKeyword:
                            case SyntaxTokenKind.ConstKeyword:
                            {
                                var x0 = ParseTopLevelItem();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.CompilationUnit, x0);
                                node = CreateNode(SyntaxTokenKind.CompilationUnit, node, temp);
                                break;
                            }
                            default: return node;
                        }
                    }
                    return node;
                }
            }
        }
        
        private SyntaxToken? ParseTopLevelItem()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.FuncKeyword:
                {
                    var n0 = ParseFunctionDefinition();
                    return CreateNode(SyntaxTokenKind.TopLevelItem, n0);
                }
                case SyntaxTokenKind.VarKeyword:
                case SyntaxTokenKind.ConstKeyword:
                {
                    var n0 = ParseDeclarationStatement();
                    return CreateNode(SyntaxTokenKind.TopLevelItem, n0);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.FuncKeyword, SyntaxTokenKind.VarKeyword, SyntaxTokenKind.ConstKeyword);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseFunctionDefinition()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.FuncKeyword:
                {
                    var n0 = Advance();
                    var n1 = Expect(SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n2 = Expect(SyntaxTokenKind.LeftParen, SyntaxTokenKind.Identifier, SyntaxTokenKind.Epsilon);
                    var n3 = ParseOptionalParameters();
                    var n4 = Expect(SyntaxTokenKind.RightParen, SyntaxTokenKind.SmallArrow, SyntaxTokenKind.Epsilon);
                    var n5 = ParseTrailingTypeClause();
                    var n6 = ParseBlockStatement();
                    return CreateNode(SyntaxTokenKind.FunctionDefinition, n0, n1, n2, n3, n4, n5, n6);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.FuncKeyword);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseTrailingTypeClause()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.SmallArrow:
                {
                    var n0 = Advance();
                    var n1 = Expect(SyntaxTokenKind.Identifier);
                    return CreateNode(SyntaxTokenKind.TrailingTypeClause, n0, n1);
                }
                default: return null;
            }
        }
        
        private SyntaxToken? ParseOptionalParameters()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Identifier:
                {
                    var n0 = ParseParameters();
                    return CreateNode(SyntaxTokenKind.OptionalParameters, n0);
                }
                default: return null;
            }
        }
        
        private SyntaxToken? ParseParameters()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Identifier:
                {
                    var n0 = ParseParameterDeclaration();
                    SyntaxToken? node = CreateNode(SyntaxTokenKind.Parameters, n0);
                    while (current.Kind == SyntaxTokenKind.Comma)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.Comma:
                            {
                                var x0 = Advance();
                                var x1 = ParseParameterDeclaration();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Parameters, x0, x1);
                                node = CreateNode(SyntaxTokenKind.Parameters, node, temp);
                                break;
                            }
                        }
                    }
                    return node;
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Identifier);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseParameterDeclaration()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Identifier:
                {
                    var n0 = Advance();
                    var n1 = ParseTypeClause();
                    return CreateNode(SyntaxTokenKind.ParameterDeclaration, n0, n1);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Identifier);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseTypeClause()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Colon:
                {
                    var n0 = Advance();
                    var n1 = Expect(SyntaxTokenKind.Identifier);
                    return CreateNode(SyntaxTokenKind.TypeClause, n0, n1);
                }
                default: return null;
            }
        }
        
        private SyntaxToken? ParseStatement()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.LeftCurly:
                {
                    var n0 = ParseBlockStatement();
                    return CreateNode(SyntaxTokenKind.Statement, n0);
                }
                case SyntaxTokenKind.IfKeyword:
                {
                    var n0 = ParseIfStatement();
                    return CreateNode(SyntaxTokenKind.Statement, n0);
                }
                case SyntaxTokenKind.VarKeyword:
                case SyntaxTokenKind.ConstKeyword:
                {
                    var n0 = ParseDeclarationStatement();
                    return CreateNode(SyntaxTokenKind.Statement, n0);
                }
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseExpressionStatement();
                    return CreateNode(SyntaxTokenKind.Statement, n0);
                }
                case SyntaxTokenKind.SemiColon:
                {
                    var n0 = Advance();
                    return CreateNode(SyntaxTokenKind.Statement, n0);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.LeftCurly, SyntaxTokenKind.IfKeyword, SyntaxTokenKind.VarKeyword, SyntaxTokenKind.ConstKeyword, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen, SyntaxTokenKind.SemiColon);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseExpressionStatement()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseExpression();
                    var n1 = Expect(SyntaxTokenKind.SemiColon);
                    return CreateNode(SyntaxTokenKind.ExpressionStatement, n0, n1);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseBlockStatement()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.LeftCurly:
                {
                    var n0 = Advance();
                    var n1 = ParseRepeatedStatement();
                    var n2 = Expect(SyntaxTokenKind.RightCurly);
                    return CreateNode(SyntaxTokenKind.BlockStatement, n0, n1, n2);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.LeftCurly);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseRepeatedStatement()
        {
            switch (current.Kind)
            {
                default:
                {
                    SyntaxToken? node = null;
                    while (current.Kind == SyntaxTokenKind.LeftCurly || current.Kind == SyntaxTokenKind.IfKeyword || current.Kind == SyntaxTokenKind.VarKeyword || current.Kind == SyntaxTokenKind.ConstKeyword || current.Kind == SyntaxTokenKind.Plus || current.Kind == SyntaxTokenKind.Minus || current.Kind == SyntaxTokenKind.Int || current.Kind == SyntaxTokenKind.Float || current.Kind == SyntaxTokenKind.Bool || current.Kind == SyntaxTokenKind.String || current.Kind == SyntaxTokenKind.Identifier || current.Kind == SyntaxTokenKind.LeftParen || current.Kind == SyntaxTokenKind.SemiColon || current.Kind == SyntaxTokenKind.RightCurly)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.LeftCurly:
                            case SyntaxTokenKind.IfKeyword:
                            case SyntaxTokenKind.VarKeyword:
                            case SyntaxTokenKind.ConstKeyword:
                            case SyntaxTokenKind.Plus:
                            case SyntaxTokenKind.Minus:
                            case SyntaxTokenKind.Int:
                            case SyntaxTokenKind.Float:
                            case SyntaxTokenKind.Bool:
                            case SyntaxTokenKind.String:
                            case SyntaxTokenKind.Identifier:
                            case SyntaxTokenKind.LeftParen:
                            case SyntaxTokenKind.SemiColon:
                            {
                                var x0 = ParseStatement();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.RepeatedStatement, x0);
                                node = CreateNode(SyntaxTokenKind.RepeatedStatement, node, temp);
                                break;
                            }
                            default: return node;
                        }
                    }
                    return node;
                }
            }
        }
        
        private SyntaxToken? ParseIfStatement()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.IfKeyword:
                {
                    var n0 = Advance();
                    var n1 = ParseExpression();
                    var n2 = ParseBlockStatement();
                    var n3 = ParseOptionalElseStatement();
                    return CreateNode(SyntaxTokenKind.IfStatement, n0, n1, n2, n3);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.IfKeyword);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseOptionalElseStatement()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.ElseKeyword:
                {
                    var n0 = ParseElseStatement();
                    return CreateNode(SyntaxTokenKind.OptionalElseStatement, n0);
                }
                default: return null;
            }
        }
        
        private SyntaxToken? ParseElseStatement()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.ElseKeyword:
                {
                    var n0 = Advance();
                    var n1 = ParseIfStatementOrBlockStatement();
                    return CreateNode(SyntaxTokenKind.ElseStatement, n0, n1);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.ElseKeyword);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseIfStatementOrBlockStatement()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.IfKeyword:
                {
                    var n0 = ParseIfStatement();
                    return CreateNode(SyntaxTokenKind.IfStatementOrBlockStatement, n0);
                }
                case SyntaxTokenKind.LeftCurly:
                {
                    var n0 = ParseBlockStatement();
                    return CreateNode(SyntaxTokenKind.IfStatementOrBlockStatement, n0);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.IfKeyword, SyntaxTokenKind.LeftCurly);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseDeclarationStatement()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.VarKeyword:
                {
                    var n0 = Advance();
                    var n1 = Expect(SyntaxTokenKind.Identifier, SyntaxTokenKind.Equal);
                    var n2 = Expect(SyntaxTokenKind.Equal, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n3 = ParseExpression();
                    var n4 = Expect(SyntaxTokenKind.SemiColon);
                    return CreateNode(SyntaxTokenKind.DeclarationStatement, n0, n1, n2, n3, n4);
                }
                case SyntaxTokenKind.ConstKeyword:
                {
                    var n0 = Advance();
                    var n1 = Expect(SyntaxTokenKind.Identifier, SyntaxTokenKind.Equal);
                    var n2 = Expect(SyntaxTokenKind.Equal, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n3 = ParseExpression();
                    var n4 = Expect(SyntaxTokenKind.SemiColon);
                    return CreateNode(SyntaxTokenKind.DeclarationStatement, n0, n1, n2, n3, n4);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.VarKeyword, SyntaxTokenKind.ConstKeyword);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseExpression()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseAssignmentHelper();
                    return CreateNode(SyntaxTokenKind.Expression, n0);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseAssignmentHelper()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseLogicalOr();
                    var n1 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.AssignmentHelper, n0, n1);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseAssignment()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Equal:
                {
                    var n0 = Advance();
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.PlusEqual:
                {
                    var n0 = Advance();
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.MinusEqual:
                {
                    var n0 = Advance();
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.StarEqual:
                {
                    var n0 = Advance();
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.SlashEqual:
                {
                    var n0 = Advance();
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.PercentEqual:
                {
                    var n0 = Advance();
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.AmpersandEqual:
                {
                    var n0 = Advance();
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.PipeEqual:
                {
                    var n0 = Advance();
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.HatEqual:
                {
                    var n0 = Advance();
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                default: return null;
            }
        }
        
        private SyntaxToken? ParseLogicalOr()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseLogicalAnd();
                    SyntaxToken? node = CreateNode(SyntaxTokenKind.LogicalOr, n0);
                    while (current.Kind == SyntaxTokenKind.DoublePipe)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.DoublePipe:
                            {
                                var x0 = Advance();
                                var x1 = ParseLogicalAnd();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.LogicalOr, x0, x1);
                                node = CreateNode(SyntaxTokenKind.LogicalOr, node, temp);
                                break;
                            }
                        }
                    }
                    return node;
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseLogicalAnd()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseBitOr();
                    SyntaxToken? node = CreateNode(SyntaxTokenKind.LogicalAnd, n0);
                    while (current.Kind == SyntaxTokenKind.DoubleAmpersand)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.DoubleAmpersand:
                            {
                                var x0 = Advance();
                                var x1 = ParseBitOr();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.LogicalAnd, x0, x1);
                                node = CreateNode(SyntaxTokenKind.LogicalAnd, node, temp);
                                break;
                            }
                        }
                    }
                    return node;
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseBitOr()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseBitXor();
                    SyntaxToken? node = CreateNode(SyntaxTokenKind.BitOr, n0);
                    while (current.Kind == SyntaxTokenKind.Pipe)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.Pipe:
                            {
                                var x0 = Advance();
                                var x1 = ParseBitXor();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.BitOr, x0, x1);
                                node = CreateNode(SyntaxTokenKind.BitOr, node, temp);
                                break;
                            }
                        }
                    }
                    return node;
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseBitXor()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseBitAnd();
                    SyntaxToken? node = CreateNode(SyntaxTokenKind.BitXor, n0);
                    while (current.Kind == SyntaxTokenKind.Hat)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.Hat:
                            {
                                var x0 = Advance();
                                var x1 = ParseBitAnd();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.BitXor, x0, x1);
                                node = CreateNode(SyntaxTokenKind.BitXor, node, temp);
                                break;
                            }
                        }
                    }
                    return node;
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseBitAnd()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseEquality();
                    SyntaxToken? node = CreateNode(SyntaxTokenKind.BitAnd, n0);
                    while (current.Kind == SyntaxTokenKind.Ampersand)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.Ampersand:
                            {
                                var x0 = Advance();
                                var x1 = ParseEquality();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.BitAnd, x0, x1);
                                node = CreateNode(SyntaxTokenKind.BitAnd, node, temp);
                                break;
                            }
                        }
                    }
                    return node;
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseEquality()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseComparison();
                    SyntaxToken? node = CreateNode(SyntaxTokenKind.Equality, n0);
                    while (current.Kind == SyntaxTokenKind.DoubleEqual || current.Kind == SyntaxTokenKind.NotEqual)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.DoubleEqual:
                            {
                                var x0 = Advance();
                                var x1 = ParseComparison();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Equality, x0, x1);
                                node = CreateNode(SyntaxTokenKind.Equality, node, temp);
                                break;
                            }
                            case SyntaxTokenKind.NotEqual:
                            {
                                var x0 = Advance();
                                var x1 = ParseComparison();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Equality, x0, x1);
                                node = CreateNode(SyntaxTokenKind.Equality, node, temp);
                                break;
                            }
                        }
                    }
                    return node;
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseComparison()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseSum();
                    SyntaxToken? node = CreateNode(SyntaxTokenKind.Comparison, n0);
                    while (current.Kind == SyntaxTokenKind.Less || current.Kind == SyntaxTokenKind.LessEqual || current.Kind == SyntaxTokenKind.Greater || current.Kind == SyntaxTokenKind.GreaterEqual)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.Less:
                            {
                                var x0 = Advance();
                                var x1 = ParseSum();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Comparison, x0, x1);
                                node = CreateNode(SyntaxTokenKind.Comparison, node, temp);
                                break;
                            }
                            case SyntaxTokenKind.LessEqual:
                            {
                                var x0 = Advance();
                                var x1 = ParseSum();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Comparison, x0, x1);
                                node = CreateNode(SyntaxTokenKind.Comparison, node, temp);
                                break;
                            }
                            case SyntaxTokenKind.Greater:
                            {
                                var x0 = Advance();
                                var x1 = ParseSum();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Comparison, x0, x1);
                                node = CreateNode(SyntaxTokenKind.Comparison, node, temp);
                                break;
                            }
                            case SyntaxTokenKind.GreaterEqual:
                            {
                                var x0 = Advance();
                                var x1 = ParseSum();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Comparison, x0, x1);
                                node = CreateNode(SyntaxTokenKind.Comparison, node, temp);
                                break;
                            }
                        }
                    }
                    return node;
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseSum()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseMult();
                    SyntaxToken? node = CreateNode(SyntaxTokenKind.Sum, n0);
                    while (current.Kind == SyntaxTokenKind.Plus || current.Kind == SyntaxTokenKind.Minus)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.Plus:
                            {
                                var x0 = Advance();
                                var x1 = ParseMult();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Sum, x0, x1);
                                node = CreateNode(SyntaxTokenKind.Sum, node, temp);
                                break;
                            }
                            case SyntaxTokenKind.Minus:
                            {
                                var x0 = Advance();
                                var x1 = ParseMult();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Sum, x0, x1);
                                node = CreateNode(SyntaxTokenKind.Sum, node, temp);
                                break;
                            }
                        }
                    }
                    return node;
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseMult()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParsePrefix();
                    SyntaxToken? node = CreateNode(SyntaxTokenKind.Mult, n0);
                    while (current.Kind == SyntaxTokenKind.Star || current.Kind == SyntaxTokenKind.Slash)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.Star:
                            {
                                var x0 = Advance();
                                var x1 = ParsePrefix();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Mult, x0, x1);
                                node = CreateNode(SyntaxTokenKind.Mult, node, temp);
                                break;
                            }
                            case SyntaxTokenKind.Slash:
                            {
                                var x0 = Advance();
                                var x1 = ParsePrefix();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Mult, x0, x1);
                                node = CreateNode(SyntaxTokenKind.Mult, node, temp);
                                break;
                            }
                        }
                    }
                    return node;
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParsePrefix()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                {
                    var n0 = Advance();
                    var n1 = ParsePrefix();
                    return CreateNode(SyntaxTokenKind.Prefix, n0, n1);
                }
                case SyntaxTokenKind.Minus:
                {
                    var n0 = Advance();
                    var n1 = ParsePrefix();
                    return CreateNode(SyntaxTokenKind.Prefix, n0, n1);
                }
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParsePostfix();
                    return CreateNode(SyntaxTokenKind.Prefix, n0);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParsePostfix()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParsePrimary();
                    SyntaxToken? node = CreateNode(SyntaxTokenKind.Postfix, n0);
                    while (current.Kind == SyntaxTokenKind.LeftParen || current.Kind == SyntaxTokenKind.LeftSquare || current.Kind == SyntaxTokenKind.Dot)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.LeftParen:
                            {
                                var x0 = Advance();
                                var x1 = ParseArguments();
                                var x2 = Expect(SyntaxTokenKind.RightParen);
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Postfix, x0, x1, x2);
                                node = CreateNode(SyntaxTokenKind.Postfix, node, temp);
                                break;
                            }
                            case SyntaxTokenKind.LeftSquare:
                            {
                                var x0 = Advance();
                                var x1 = ParseArguments();
                                var x2 = Expect(SyntaxTokenKind.RightSquare);
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Postfix, x0, x1, x2);
                                node = CreateNode(SyntaxTokenKind.Postfix, node, temp);
                                break;
                            }
                            case SyntaxTokenKind.Dot:
                            {
                                var x0 = Advance();
                                var x1 = ParsePrimary();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Postfix, x0, x1);
                                node = CreateNode(SyntaxTokenKind.Postfix, node, temp);
                                break;
                            }
                        }
                    }
                    return node;
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseArguments()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseExpression();
                    SyntaxToken? node = CreateNode(SyntaxTokenKind.Arguments, n0);
                    while (current.Kind == SyntaxTokenKind.Comma || current.Kind == SyntaxTokenKind.Plus || current.Kind == SyntaxTokenKind.Minus || current.Kind == SyntaxTokenKind.Int || current.Kind == SyntaxTokenKind.Float || current.Kind == SyntaxTokenKind.Bool || current.Kind == SyntaxTokenKind.String || current.Kind == SyntaxTokenKind.Identifier || current.Kind == SyntaxTokenKind.LeftParen || current.Kind == SyntaxTokenKind.RightParen || current.Kind == SyntaxTokenKind.RightSquare)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.Comma:
                            {
                                var x0 = Advance();
                                var x1 = ParseExpression();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Arguments, x0, x1);
                                node = CreateNode(SyntaxTokenKind.Arguments, node, temp);
                                break;
                            }
                            case SyntaxTokenKind.Plus:
                            case SyntaxTokenKind.Minus:
                            case SyntaxTokenKind.Int:
                            case SyntaxTokenKind.Float:
                            case SyntaxTokenKind.Bool:
                            case SyntaxTokenKind.String:
                            case SyntaxTokenKind.Identifier:
                            case SyntaxTokenKind.LeftParen:
                            {
                                var x0 = ParseExpression();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Arguments, x0);
                                node = CreateNode(SyntaxTokenKind.Arguments, node, temp);
                                break;
                            }
                            default: return node;
                        }
                    }
                    return node;
                }
                default:
                {
                    SyntaxToken? node = null;
                    while (current.Kind == SyntaxTokenKind.Comma || current.Kind == SyntaxTokenKind.Plus || current.Kind == SyntaxTokenKind.Minus || current.Kind == SyntaxTokenKind.Int || current.Kind == SyntaxTokenKind.Float || current.Kind == SyntaxTokenKind.Bool || current.Kind == SyntaxTokenKind.String || current.Kind == SyntaxTokenKind.Identifier || current.Kind == SyntaxTokenKind.LeftParen || current.Kind == SyntaxTokenKind.RightParen || current.Kind == SyntaxTokenKind.RightSquare)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.Comma:
                            {
                                var x0 = Advance();
                                var x1 = ParseExpression();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Arguments, x0, x1);
                                node = CreateNode(SyntaxTokenKind.Arguments, node, temp);
                                break;
                            }
                            case SyntaxTokenKind.Plus:
                            case SyntaxTokenKind.Minus:
                            case SyntaxTokenKind.Int:
                            case SyntaxTokenKind.Float:
                            case SyntaxTokenKind.Bool:
                            case SyntaxTokenKind.String:
                            case SyntaxTokenKind.Identifier:
                            case SyntaxTokenKind.LeftParen:
                            {
                                var x0 = ParseExpression();
                                SyntaxToken? temp = CreateNode(SyntaxTokenKind.Arguments, x0);
                                node = CreateNode(SyntaxTokenKind.Arguments, node, temp);
                                break;
                            }
                            default: return node;
                        }
                    }
                    return node;
                }
            }
        }
        
        private SyntaxToken? ParsePrimary()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Int:
                {
                    var n0 = Advance();
                    return CreateNode(SyntaxTokenKind.Primary, n0);
                }
                case SyntaxTokenKind.Float:
                {
                    var n0 = Advance();
                    return CreateNode(SyntaxTokenKind.Primary, n0);
                }
                case SyntaxTokenKind.Bool:
                {
                    var n0 = Advance();
                    return CreateNode(SyntaxTokenKind.Primary, n0);
                }
                case SyntaxTokenKind.String:
                {
                    var n0 = Advance();
                    return CreateNode(SyntaxTokenKind.Primary, n0);
                }
                case SyntaxTokenKind.Identifier:
                {
                    var n0 = Advance();
                    return CreateNode(SyntaxTokenKind.Primary, n0);
                }
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseParenthesized();
                    return CreateNode(SyntaxTokenKind.Primary, n0);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
        
        private SyntaxToken? ParseParenthesized()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = Advance();
                    var n1 = ParseSum();
                    var n2 = Expect(SyntaxTokenKind.RightParen);
                    return CreateNode(SyntaxTokenKind.Parenthesized, n0, n1, n2);
                }
                default:
                {
                    Unexpected(SyntaxTokenKind.LeftParen);
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken?>(current));
                }
            }
        }
    }
}
