using System.Linq;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace Kyloe.Syntax
{
    internal sealed class Parser
    {
        private readonly ImmutableArray<SyntaxTerminal> terminals;
        
        private readonly HashSet<SyntaxTokenKind> stopTerminals;
        
        private readonly Kyloe.Diagnostics.DiagnosticCollector errors;
        
        private int pos;
        
        private bool isValid;
        
        private SyntaxTerminal current => pos < terminals.Length ? terminals[pos] : terminals[terminals.Length - 1];
        
        public Parser(Kyloe.Utility.SourceText source, Kyloe.Diagnostics.DiagnosticCollector errors)
        {
            this.pos = 0;
            this.isValid = true;
            this.errors = errors;
            var lexer = new Lexer(source);
            var builder = ImmutableArray.CreateBuilder<SyntaxTerminal>();
            foreach (var terminal in lexer.Terminals())
            {
                if (terminal.Kind == SyntaxTokenKind.Error)
                    errors.InvalidCharacterError(terminal.Location, terminal.Text[0]);
                else
                    builder.Add(terminal);
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
        
        private SyntaxTerminal Expect(SyntaxTokenKind expected, params SyntaxTokenKind[] next)
        {
            if (current.Kind == expected) return Advance();
            Unexpected(expected);
            var forged = new SyntaxTerminal(expected, current.Text, current.Location, invalid: true);
            if (next.Length != 0) SkipInput(expected, next);
            if (current.Kind == expected) return Advance();
            return forged;
        }
        
        private void SkipInput(SyntaxTokenKind expected, params SyntaxTokenKind[] next)
        {
            var skipSet = next.ToHashSet();
            skipSet.Add(expected);
            while (!skipSet.Contains(current.Kind) && !stopTerminals.Contains(current.Kind))
            {
                pos += 1;
            }
        }
        
        private void Unexpected(params SyntaxTokenKind[] expected)
        {
            if (!isValid) return;
            isValid = false;
            errors.UnexpectedTokenError(current, expected);
        }
        
        private SyntaxToken CreateNode(SyntaxTokenKind kind, params SyntaxToken[] tokens)
        {
            var arr = tokens.ToImmutableArray();
            if (arr.Length == 0) return new SyntaxNode(SyntaxTokenKind.Epsilon, ImmutableArray<SyntaxToken>.Empty);
            else if (arr.Length == 1) return arr[0];
            else return new SyntaxNode(kind, arr);
        }
        
        public SyntaxToken Parse()
        {
            var token = ParseStart();
            Expect(SyntaxTokenKind.End);
            return token;
        }
        
        private SyntaxToken ParseStart()
        {
            var n0 = ParseCompilationUnit();
            return CreateNode(SyntaxTokenKind.Start, n0);
        }
        
        private SyntaxToken ParseCompilationUnit()
        {
            var n0 = ParseTopLevelItem();
            SyntaxToken node = CreateNode(SyntaxTokenKind.CompilationUnit, n0);
            while (current.Kind == SyntaxTokenKind.FuncKeyword || current.Kind == SyntaxTokenKind.VarKeyword || current.Kind == SyntaxTokenKind.ConstKeyword)
            {
                var x0 = ParseTopLevelItem();
                node = CreateNode(SyntaxTokenKind.CompilationUnit, node, x0);
            }
            return node;
        }
        
        private SyntaxToken ParseTopLevelItem()
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
                    var erroneous = current;
                    Unexpected(SyntaxTokenKind.FuncKeyword, SyntaxTokenKind.ConstKeyword, SyntaxTokenKind.VarKeyword);
                    SkipInput(SyntaxTokenKind.FuncKeyword, SyntaxTokenKind.ConstKeyword, SyntaxTokenKind.VarKeyword, SyntaxTokenKind.End);
                    if (current.Kind == SyntaxTokenKind.FuncKeyword || current.Kind == SyntaxTokenKind.ConstKeyword || current.Kind == SyntaxTokenKind.VarKeyword)
                    {
                        return ParseTopLevelItem();
                    }
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken>(erroneous));
                }
            }
        }
        
        private SyntaxToken ParseFunctionDefinition()
        {
            var n0 = Expect(SyntaxTokenKind.FuncKeyword, SyntaxTokenKind.Identifier);
            var n1 = Expect(SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
            var n2 = Expect(SyntaxTokenKind.LeftParen, SyntaxTokenKind.Identifier, SyntaxTokenKind.RightParen);
            var n3 = ParseOptionalParameters();
            var n4 = Expect(SyntaxTokenKind.RightParen, SyntaxTokenKind.SmallArrow, SyntaxTokenKind.LeftCurly);
            var n5 = ParseTrailingTypeClause();
            var n6 = ParseBlockStatement();
            return CreateNode(SyntaxTokenKind.FunctionDefinition, n0, n1, n2, n3, n4, n5, n6);
        }
        
        private SyntaxToken ParseTrailingTypeClause()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.SmallArrow:
                {
                    var n0 = Expect(SyntaxTokenKind.SmallArrow, SyntaxTokenKind.Identifier);
                    var n1 = Expect(SyntaxTokenKind.Identifier);
                    return CreateNode(SyntaxTokenKind.TrailingTypeClause, n0, n1);
                }
                default: return new SyntaxNode(SyntaxTokenKind.Epsilon, ImmutableArray<SyntaxToken>.Empty);
            }
        }
        
        private SyntaxToken ParseOptionalParameters()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Identifier:
                {
                    var n0 = ParseParameters();
                    return CreateNode(SyntaxTokenKind.OptionalParameters, n0);
                }
                default: return new SyntaxNode(SyntaxTokenKind.Epsilon, ImmutableArray<SyntaxToken>.Empty);
            }
        }
        
        private SyntaxToken ParseParameters()
        {
            var n0 = ParseParameterDeclaration();
            SyntaxToken node = CreateNode(SyntaxTokenKind.Parameters, n0);
            while (current.Kind == SyntaxTokenKind.Comma)
            {
                var x0 = Expect(SyntaxTokenKind.Comma, SyntaxTokenKind.Identifier);
                var x1 = ParseParameterDeclaration();
                node = CreateNode(SyntaxTokenKind.Parameters, node, x0, x1);
            }
            return node;
        }
        
        private SyntaxToken ParseParameterDeclaration()
        {
            var n0 = Expect(SyntaxTokenKind.Identifier, SyntaxTokenKind.Colon);
            var n1 = ParseTypeClause();
            return CreateNode(SyntaxTokenKind.ParameterDeclaration, n0, n1);
        }
        
        private SyntaxToken ParseTypeClause()
        {
            var n0 = Expect(SyntaxTokenKind.Colon, SyntaxTokenKind.Identifier);
            var n1 = Expect(SyntaxTokenKind.Identifier);
            return CreateNode(SyntaxTokenKind.TypeClause, n0, n1);
        }
        
        private SyntaxToken ParseOptionalTypeClause()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Colon:
                {
                    var n0 = ParseTypeClause();
                    return CreateNode(SyntaxTokenKind.OptionalTypeClause, n0);
                }
                default: return new SyntaxNode(SyntaxTokenKind.Epsilon, ImmutableArray<SyntaxToken>.Empty);
            }
        }
        
        private SyntaxToken ParseStatement()
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
                case SyntaxTokenKind.WhileKeyword:
                {
                    var n0 = ParseWhileStatement();
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
                case SyntaxTokenKind.Bang:
                case SyntaxTokenKind.Tilde:
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
                case SyntaxTokenKind.BreakKeyword:
                {
                    var n0 = ParseBreakStatement();
                    return CreateNode(SyntaxTokenKind.Statement, n0);
                }
                case SyntaxTokenKind.ContinueKeyword:
                {
                    var n0 = ParseContinueStatement();
                    return CreateNode(SyntaxTokenKind.Statement, n0);
                }
                case SyntaxTokenKind.ReturnKeyword:
                {
                    var n0 = ParseReturnStatement();
                    return CreateNode(SyntaxTokenKind.Statement, n0);
                }
                case SyntaxTokenKind.SemiColon:
                {
                    var n0 = Expect(SyntaxTokenKind.SemiColon);
                    return CreateNode(SyntaxTokenKind.Statement, n0);
                }
                default:
                {
                    var erroneous = current;
                    Unexpected(SyntaxTokenKind.Identifier, SyntaxTokenKind.ReturnKeyword, SyntaxTokenKind.ContinueKeyword, SyntaxTokenKind.BreakKeyword, SyntaxTokenKind.WhileKeyword, SyntaxTokenKind.IfKeyword, SyntaxTokenKind.ConstKeyword, SyntaxTokenKind.VarKeyword, SyntaxTokenKind.String, SyntaxTokenKind.Bool, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.LeftCurly, SyntaxTokenKind.LeftParen, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Minus, SyntaxTokenKind.Plus, SyntaxTokenKind.SemiColon);
                    SkipInput(SyntaxTokenKind.Identifier, SyntaxTokenKind.ReturnKeyword, SyntaxTokenKind.ContinueKeyword, SyntaxTokenKind.BreakKeyword, SyntaxTokenKind.WhileKeyword, SyntaxTokenKind.IfKeyword, SyntaxTokenKind.ConstKeyword, SyntaxTokenKind.VarKeyword, SyntaxTokenKind.String, SyntaxTokenKind.Bool, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.LeftCurly, SyntaxTokenKind.LeftParen, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Minus, SyntaxTokenKind.Plus, SyntaxTokenKind.SemiColon, SyntaxTokenKind.RightCurly);
                    if (current.Kind == SyntaxTokenKind.Identifier || current.Kind == SyntaxTokenKind.ReturnKeyword || current.Kind == SyntaxTokenKind.ContinueKeyword || current.Kind == SyntaxTokenKind.BreakKeyword || current.Kind == SyntaxTokenKind.WhileKeyword || current.Kind == SyntaxTokenKind.IfKeyword || current.Kind == SyntaxTokenKind.ConstKeyword || current.Kind == SyntaxTokenKind.VarKeyword || current.Kind == SyntaxTokenKind.String || current.Kind == SyntaxTokenKind.Bool || current.Kind == SyntaxTokenKind.Int || current.Kind == SyntaxTokenKind.Float || current.Kind == SyntaxTokenKind.LeftCurly || current.Kind == SyntaxTokenKind.LeftParen || current.Kind == SyntaxTokenKind.Bang || current.Kind == SyntaxTokenKind.Tilde || current.Kind == SyntaxTokenKind.Minus || current.Kind == SyntaxTokenKind.Plus || current.Kind == SyntaxTokenKind.SemiColon)
                    {
                        return ParseStatement();
                    }
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken>(erroneous));
                }
            }
        }
        
        private SyntaxToken ParseExpressionStatement()
        {
            var n0 = ParseExpression();
            var n1 = Expect(SyntaxTokenKind.SemiColon);
            return CreateNode(SyntaxTokenKind.ExpressionStatement, n0, n1);
        }
        
        private SyntaxToken ParseBlockStatement()
        {
            var n0 = Expect(SyntaxTokenKind.LeftCurly, SyntaxTokenKind.LeftCurly, SyntaxTokenKind.IfKeyword, SyntaxTokenKind.WhileKeyword, SyntaxTokenKind.VarKeyword, SyntaxTokenKind.ConstKeyword, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen, SyntaxTokenKind.BreakKeyword, SyntaxTokenKind.ContinueKeyword, SyntaxTokenKind.ReturnKeyword, SyntaxTokenKind.SemiColon, SyntaxTokenKind.RightCurly);
            var n1 = ParseRepeatedStatement();
            var n2 = Expect(SyntaxTokenKind.RightCurly);
            return CreateNode(SyntaxTokenKind.BlockStatement, n0, n1, n2);
        }
        
        private SyntaxToken ParseRepeatedStatement()
        {
            switch (current.Kind)
            {
                default:
                {
                    SyntaxToken node = new SyntaxNode(SyntaxTokenKind.Epsilon, ImmutableArray<SyntaxToken>.Empty);
                    while (current.Kind == SyntaxTokenKind.LeftCurly || current.Kind == SyntaxTokenKind.IfKeyword || current.Kind == SyntaxTokenKind.WhileKeyword || current.Kind == SyntaxTokenKind.VarKeyword || current.Kind == SyntaxTokenKind.ConstKeyword || current.Kind == SyntaxTokenKind.Plus || current.Kind == SyntaxTokenKind.Minus || current.Kind == SyntaxTokenKind.Bang || current.Kind == SyntaxTokenKind.Tilde || current.Kind == SyntaxTokenKind.Int || current.Kind == SyntaxTokenKind.Float || current.Kind == SyntaxTokenKind.Bool || current.Kind == SyntaxTokenKind.String || current.Kind == SyntaxTokenKind.Identifier || current.Kind == SyntaxTokenKind.LeftParen || current.Kind == SyntaxTokenKind.BreakKeyword || current.Kind == SyntaxTokenKind.ContinueKeyword || current.Kind == SyntaxTokenKind.ReturnKeyword || current.Kind == SyntaxTokenKind.SemiColon || current.Kind == SyntaxTokenKind.RightCurly)
                    {
                        switch (current.Kind)
                        {
                            case SyntaxTokenKind.LeftCurly:
                            case SyntaxTokenKind.IfKeyword:
                            case SyntaxTokenKind.WhileKeyword:
                            case SyntaxTokenKind.VarKeyword:
                            case SyntaxTokenKind.ConstKeyword:
                            case SyntaxTokenKind.Plus:
                            case SyntaxTokenKind.Minus:
                            case SyntaxTokenKind.Bang:
                            case SyntaxTokenKind.Tilde:
                            case SyntaxTokenKind.Int:
                            case SyntaxTokenKind.Float:
                            case SyntaxTokenKind.Bool:
                            case SyntaxTokenKind.String:
                            case SyntaxTokenKind.Identifier:
                            case SyntaxTokenKind.LeftParen:
                            case SyntaxTokenKind.BreakKeyword:
                            case SyntaxTokenKind.ContinueKeyword:
                            case SyntaxTokenKind.ReturnKeyword:
                            case SyntaxTokenKind.SemiColon:
                            {
                                var x0 = ParseStatement();
                                node = CreateNode(SyntaxTokenKind.RepeatedStatement, node, x0);
                                break;
                            }
                            default: return node;
                        }
                    }
                    return node;
                }
            }
        }
        
        private SyntaxToken ParseIfStatement()
        {
            var n0 = Expect(SyntaxTokenKind.IfKeyword, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
            var n1 = ParseExpression();
            var n2 = ParseBlockStatement();
            var n3 = ParseElifStatement();
            return CreateNode(SyntaxTokenKind.IfStatement, n0, n1, n2, n3);
        }
        
        private SyntaxToken ParseElifClause()
        {
            var n0 = Expect(SyntaxTokenKind.ElifKeyword, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
            var n1 = ParseExpression();
            return CreateNode(SyntaxTokenKind.ElifClause, n0, n1);
        }
        
        private SyntaxToken ParseElifStatement()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.ElseKeyword:
                {
                    var n0 = Expect(SyntaxTokenKind.ElseKeyword, SyntaxTokenKind.LeftCurly);
                    var n1 = ParseBlockStatement();
                    var n2 = ParseElifStatement();
                    return CreateNode(SyntaxTokenKind.ElifStatement, n0, n1, n2);
                }
                case SyntaxTokenKind.ElifKeyword:
                {
                    var n0 = ParseElifClause();
                    var n1 = ParseBlockStatement();
                    var n2 = ParseElifStatement();
                    return CreateNode(SyntaxTokenKind.ElifStatement, n0, n1, n2);
                }
                default: return new SyntaxNode(SyntaxTokenKind.Epsilon, ImmutableArray<SyntaxToken>.Empty);
            }
        }
        
        private SyntaxToken ParseWhileStatement()
        {
            var n0 = Expect(SyntaxTokenKind.WhileKeyword, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
            var n1 = ParseExpression();
            var n2 = ParseBlockStatement();
            return CreateNode(SyntaxTokenKind.WhileStatement, n0, n1, n2);
        }
        
        private SyntaxToken ParseBreakStatement()
        {
            var n0 = Expect(SyntaxTokenKind.BreakKeyword, SyntaxTokenKind.SemiColon);
            var n1 = Expect(SyntaxTokenKind.SemiColon);
            return CreateNode(SyntaxTokenKind.BreakStatement, n0, n1);
        }
        
        private SyntaxToken ParseContinueStatement()
        {
            var n0 = Expect(SyntaxTokenKind.ContinueKeyword, SyntaxTokenKind.SemiColon);
            var n1 = Expect(SyntaxTokenKind.SemiColon);
            return CreateNode(SyntaxTokenKind.ContinueStatement, n0, n1);
        }
        
        private SyntaxToken ParseReturnStatement()
        {
            var n0 = Expect(SyntaxTokenKind.ReturnKeyword, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen, SyntaxTokenKind.SemiColon);
            var n1 = ParseOptionalExpression();
            var n2 = Expect(SyntaxTokenKind.SemiColon);
            return CreateNode(SyntaxTokenKind.ReturnStatement, n0, n1, n2);
        }
        
        private SyntaxToken ParseDeclarationStatement()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.VarKeyword:
                {
                    var n0 = Expect(SyntaxTokenKind.VarKeyword, SyntaxTokenKind.Identifier);
                    var n1 = Expect(SyntaxTokenKind.Identifier, SyntaxTokenKind.Colon, SyntaxTokenKind.Equal);
                    var n2 = ParseOptionalTypeClause();
                    var n3 = Expect(SyntaxTokenKind.Equal, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n4 = ParseExpression();
                    var n5 = Expect(SyntaxTokenKind.SemiColon);
                    return CreateNode(SyntaxTokenKind.DeclarationStatement, n0, n1, n2, n3, n4, n5);
                }
                case SyntaxTokenKind.ConstKeyword:
                {
                    var n0 = Expect(SyntaxTokenKind.ConstKeyword, SyntaxTokenKind.Identifier);
                    var n1 = Expect(SyntaxTokenKind.Identifier, SyntaxTokenKind.Colon, SyntaxTokenKind.Equal);
                    var n2 = ParseOptionalTypeClause();
                    var n3 = Expect(SyntaxTokenKind.Equal, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n4 = ParseExpression();
                    var n5 = Expect(SyntaxTokenKind.SemiColon);
                    return CreateNode(SyntaxTokenKind.DeclarationStatement, n0, n1, n2, n3, n4, n5);
                }
                default:
                {
                    var erroneous = current;
                    Unexpected(SyntaxTokenKind.ConstKeyword, SyntaxTokenKind.VarKeyword);
                    SkipInput(SyntaxTokenKind.ConstKeyword, SyntaxTokenKind.VarKeyword, SyntaxTokenKind.Identifier, SyntaxTokenKind.ReturnKeyword, SyntaxTokenKind.ContinueKeyword, SyntaxTokenKind.BreakKeyword, SyntaxTokenKind.WhileKeyword, SyntaxTokenKind.IfKeyword, SyntaxTokenKind.FuncKeyword, SyntaxTokenKind.String, SyntaxTokenKind.Bool, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.RightCurly, SyntaxTokenKind.LeftCurly, SyntaxTokenKind.LeftParen, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Minus, SyntaxTokenKind.Plus, SyntaxTokenKind.SemiColon, SyntaxTokenKind.End);
                    if (current.Kind == SyntaxTokenKind.ConstKeyword || current.Kind == SyntaxTokenKind.VarKeyword)
                    {
                        return ParseDeclarationStatement();
                    }
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken>(erroneous));
                }
            }
        }
        
        private SyntaxToken ParseExpression()
        {
            var n0 = ParseAssignmentHelper();
            return CreateNode(SyntaxTokenKind.Expression, n0);
        }
        
        private SyntaxToken ParseOptionalExpression()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Bang:
                case SyntaxTokenKind.Tilde:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseExpression();
                    return CreateNode(SyntaxTokenKind.OptionalExpression, n0);
                }
                default: return new SyntaxNode(SyntaxTokenKind.Epsilon, ImmutableArray<SyntaxToken>.Empty);
            }
        }
        
        private SyntaxToken ParseAssignmentHelper()
        {
            var n0 = ParseLogicalOr();
            var n1 = ParseAssignment();
            return CreateNode(SyntaxTokenKind.AssignmentHelper, n0, n1);
        }
        
        private SyntaxToken ParseAssignment()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Equal:
                {
                    var n0 = Expect(SyntaxTokenKind.Equal, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.PlusEqual:
                {
                    var n0 = Expect(SyntaxTokenKind.PlusEqual, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.MinusEqual:
                {
                    var n0 = Expect(SyntaxTokenKind.MinusEqual, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.StarEqual:
                {
                    var n0 = Expect(SyntaxTokenKind.StarEqual, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.SlashEqual:
                {
                    var n0 = Expect(SyntaxTokenKind.SlashEqual, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.PercentEqual:
                {
                    var n0 = Expect(SyntaxTokenKind.PercentEqual, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.AmpersandEqual:
                {
                    var n0 = Expect(SyntaxTokenKind.AmpersandEqual, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.PipeEqual:
                {
                    var n0 = Expect(SyntaxTokenKind.PipeEqual, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                case SyntaxTokenKind.HatEqual:
                {
                    var n0 = Expect(SyntaxTokenKind.HatEqual, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n1 = ParseLogicalOr();
                    var n2 = ParseAssignment();
                    return CreateNode(SyntaxTokenKind.Assignment, n0, n1, n2);
                }
                default: return new SyntaxNode(SyntaxTokenKind.Epsilon, ImmutableArray<SyntaxToken>.Empty);
            }
        }
        
        private SyntaxToken ParseLogicalOr()
        {
            var n0 = ParseLogicalAnd();
            SyntaxToken node = CreateNode(SyntaxTokenKind.LogicalOr, n0);
            while (current.Kind == SyntaxTokenKind.DoublePipe)
            {
                var x0 = Expect(SyntaxTokenKind.DoublePipe, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                var x1 = ParseLogicalAnd();
                node = CreateNode(SyntaxTokenKind.LogicalOr, node, x0, x1);
            }
            return node;
        }
        
        private SyntaxToken ParseLogicalAnd()
        {
            var n0 = ParseBitOr();
            SyntaxToken node = CreateNode(SyntaxTokenKind.LogicalAnd, n0);
            while (current.Kind == SyntaxTokenKind.DoubleAmpersand)
            {
                var x0 = Expect(SyntaxTokenKind.DoubleAmpersand, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                var x1 = ParseBitOr();
                node = CreateNode(SyntaxTokenKind.LogicalAnd, node, x0, x1);
            }
            return node;
        }
        
        private SyntaxToken ParseBitOr()
        {
            var n0 = ParseBitXor();
            SyntaxToken node = CreateNode(SyntaxTokenKind.BitOr, n0);
            while (current.Kind == SyntaxTokenKind.Pipe)
            {
                var x0 = Expect(SyntaxTokenKind.Pipe, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                var x1 = ParseBitXor();
                node = CreateNode(SyntaxTokenKind.BitOr, node, x0, x1);
            }
            return node;
        }
        
        private SyntaxToken ParseBitXor()
        {
            var n0 = ParseBitAnd();
            SyntaxToken node = CreateNode(SyntaxTokenKind.BitXor, n0);
            while (current.Kind == SyntaxTokenKind.Hat)
            {
                var x0 = Expect(SyntaxTokenKind.Hat, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                var x1 = ParseBitAnd();
                node = CreateNode(SyntaxTokenKind.BitXor, node, x0, x1);
            }
            return node;
        }
        
        private SyntaxToken ParseBitAnd()
        {
            var n0 = ParseEquality();
            SyntaxToken node = CreateNode(SyntaxTokenKind.BitAnd, n0);
            while (current.Kind == SyntaxTokenKind.Ampersand)
            {
                var x0 = Expect(SyntaxTokenKind.Ampersand, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                var x1 = ParseEquality();
                node = CreateNode(SyntaxTokenKind.BitAnd, node, x0, x1);
            }
            return node;
        }
        
        private SyntaxToken ParseEquality()
        {
            var n0 = ParseComparison();
            SyntaxToken node = CreateNode(SyntaxTokenKind.Equality, n0);
            while (current.Kind == SyntaxTokenKind.DoubleEqual || current.Kind == SyntaxTokenKind.NotEqual)
            {
                switch (current.Kind)
                {
                    case SyntaxTokenKind.DoubleEqual:
                    {
                        var x0 = Expect(SyntaxTokenKind.DoubleEqual, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                        var x1 = ParseComparison();
                        node = CreateNode(SyntaxTokenKind.Equality, node, x0, x1);
                        break;
                    }
                    case SyntaxTokenKind.NotEqual:
                    {
                        var x0 = Expect(SyntaxTokenKind.NotEqual, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                        var x1 = ParseComparison();
                        node = CreateNode(SyntaxTokenKind.Equality, node, x0, x1);
                        break;
                    }
                }
            }
            return node;
        }
        
        private SyntaxToken ParseComparison()
        {
            var n0 = ParseSum();
            SyntaxToken node = CreateNode(SyntaxTokenKind.Comparison, n0);
            while (current.Kind == SyntaxTokenKind.Less || current.Kind == SyntaxTokenKind.LessEqual || current.Kind == SyntaxTokenKind.Greater || current.Kind == SyntaxTokenKind.GreaterEqual)
            {
                switch (current.Kind)
                {
                    case SyntaxTokenKind.Less:
                    {
                        var x0 = Expect(SyntaxTokenKind.Less, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                        var x1 = ParseSum();
                        node = CreateNode(SyntaxTokenKind.Comparison, node, x0, x1);
                        break;
                    }
                    case SyntaxTokenKind.LessEqual:
                    {
                        var x0 = Expect(SyntaxTokenKind.LessEqual, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                        var x1 = ParseSum();
                        node = CreateNode(SyntaxTokenKind.Comparison, node, x0, x1);
                        break;
                    }
                    case SyntaxTokenKind.Greater:
                    {
                        var x0 = Expect(SyntaxTokenKind.Greater, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                        var x1 = ParseSum();
                        node = CreateNode(SyntaxTokenKind.Comparison, node, x0, x1);
                        break;
                    }
                    case SyntaxTokenKind.GreaterEqual:
                    {
                        var x0 = Expect(SyntaxTokenKind.GreaterEqual, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                        var x1 = ParseSum();
                        node = CreateNode(SyntaxTokenKind.Comparison, node, x0, x1);
                        break;
                    }
                }
            }
            return node;
        }
        
        private SyntaxToken ParseSum()
        {
            var n0 = ParseMult();
            SyntaxToken node = CreateNode(SyntaxTokenKind.Sum, n0);
            while (current.Kind == SyntaxTokenKind.Plus || current.Kind == SyntaxTokenKind.Minus)
            {
                switch (current.Kind)
                {
                    case SyntaxTokenKind.Plus:
                    {
                        var x0 = Expect(SyntaxTokenKind.Plus, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                        var x1 = ParseMult();
                        node = CreateNode(SyntaxTokenKind.Sum, node, x0, x1);
                        break;
                    }
                    case SyntaxTokenKind.Minus:
                    {
                        var x0 = Expect(SyntaxTokenKind.Minus, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                        var x1 = ParseMult();
                        node = CreateNode(SyntaxTokenKind.Sum, node, x0, x1);
                        break;
                    }
                }
            }
            return node;
        }
        
        private SyntaxToken ParseMult()
        {
            var n0 = ParsePrefix();
            SyntaxToken node = CreateNode(SyntaxTokenKind.Mult, n0);
            while (current.Kind == SyntaxTokenKind.Star || current.Kind == SyntaxTokenKind.Slash || current.Kind == SyntaxTokenKind.Percent)
            {
                switch (current.Kind)
                {
                    case SyntaxTokenKind.Star:
                    {
                        var x0 = Expect(SyntaxTokenKind.Star, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                        var x1 = ParsePrefix();
                        node = CreateNode(SyntaxTokenKind.Mult, node, x0, x1);
                        break;
                    }
                    case SyntaxTokenKind.Slash:
                    {
                        var x0 = Expect(SyntaxTokenKind.Slash, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                        var x1 = ParsePrefix();
                        node = CreateNode(SyntaxTokenKind.Mult, node, x0, x1);
                        break;
                    }
                    case SyntaxTokenKind.Percent:
                    {
                        var x0 = Expect(SyntaxTokenKind.Percent, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                        var x1 = ParsePrefix();
                        node = CreateNode(SyntaxTokenKind.Mult, node, x0, x1);
                        break;
                    }
                }
            }
            return node;
        }
        
        private SyntaxToken ParsePrefix()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                {
                    var n0 = Expect(SyntaxTokenKind.Plus, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n1 = ParsePrefix();
                    return CreateNode(SyntaxTokenKind.Prefix, n0, n1);
                }
                case SyntaxTokenKind.Minus:
                {
                    var n0 = Expect(SyntaxTokenKind.Minus, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n1 = ParsePrefix();
                    return CreateNode(SyntaxTokenKind.Prefix, n0, n1);
                }
                case SyntaxTokenKind.Bang:
                {
                    var n0 = Expect(SyntaxTokenKind.Bang, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                    var n1 = ParsePrefix();
                    return CreateNode(SyntaxTokenKind.Prefix, n0, n1);
                }
                case SyntaxTokenKind.Tilde:
                {
                    var n0 = Expect(SyntaxTokenKind.Tilde, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
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
                    var erroneous = current;
                    Unexpected(SyntaxTokenKind.Identifier, SyntaxTokenKind.String, SyntaxTokenKind.Bool, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.LeftParen, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Minus, SyntaxTokenKind.Plus);
                    SkipInput(SyntaxTokenKind.Identifier, SyntaxTokenKind.String, SyntaxTokenKind.Bool, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.LeftParen, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Minus, SyntaxTokenKind.Plus, SyntaxTokenKind.LeftCurly, SyntaxTokenKind.RightSquare, SyntaxTokenKind.RightParen, SyntaxTokenKind.Hat, SyntaxTokenKind.Pipe, SyntaxTokenKind.DoublePipe, SyntaxTokenKind.Ampersand, SyntaxTokenKind.DoubleAmpersand, SyntaxTokenKind.Percent, SyntaxTokenKind.Slash, SyntaxTokenKind.Star, SyntaxTokenKind.Equal, SyntaxTokenKind.HatEqual, SyntaxTokenKind.PipeEqual, SyntaxTokenKind.AmpersandEqual, SyntaxTokenKind.PercentEqual, SyntaxTokenKind.SlashEqual, SyntaxTokenKind.StarEqual, SyntaxTokenKind.MinusEqual, SyntaxTokenKind.PlusEqual, SyntaxTokenKind.NotEqual, SyntaxTokenKind.DoubleEqual, SyntaxTokenKind.Greater, SyntaxTokenKind.GreaterEqual, SyntaxTokenKind.Less, SyntaxTokenKind.LessEqual, SyntaxTokenKind.SemiColon, SyntaxTokenKind.Comma, SyntaxTokenKind.Epsilon);
                    if (current.Kind == SyntaxTokenKind.Identifier || current.Kind == SyntaxTokenKind.String || current.Kind == SyntaxTokenKind.Bool || current.Kind == SyntaxTokenKind.Int || current.Kind == SyntaxTokenKind.Float || current.Kind == SyntaxTokenKind.LeftParen || current.Kind == SyntaxTokenKind.Bang || current.Kind == SyntaxTokenKind.Tilde || current.Kind == SyntaxTokenKind.Minus || current.Kind == SyntaxTokenKind.Plus)
                    {
                        return ParsePrefix();
                    }
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken>(erroneous));
                }
            }
        }
        
        private SyntaxToken ParsePostfix()
        {
            var n0 = ParsePrimary();
            SyntaxToken node = CreateNode(SyntaxTokenKind.Postfix, n0);
            while (current.Kind == SyntaxTokenKind.LeftParen || current.Kind == SyntaxTokenKind.LeftSquare || current.Kind == SyntaxTokenKind.Dot)
            {
                switch (current.Kind)
                {
                    case SyntaxTokenKind.LeftParen:
                    {
                        var x0 = Expect(SyntaxTokenKind.LeftParen, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen, SyntaxTokenKind.RightParen);
                        var x1 = ParseOptionalArguments();
                        var x2 = Expect(SyntaxTokenKind.RightParen);
                        node = CreateNode(SyntaxTokenKind.Postfix, node, x0, x1, x2);
                        break;
                    }
                    case SyntaxTokenKind.LeftSquare:
                    {
                        var x0 = Expect(SyntaxTokenKind.LeftSquare, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                        var x1 = ParseArguments();
                        var x2 = Expect(SyntaxTokenKind.RightSquare);
                        node = CreateNode(SyntaxTokenKind.Postfix, node, x0, x1, x2);
                        break;
                    }
                    case SyntaxTokenKind.Dot:
                    {
                        var x0 = Expect(SyntaxTokenKind.Dot, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                        var x1 = ParsePrimary();
                        node = CreateNode(SyntaxTokenKind.Postfix, node, x0, x1);
                        break;
                    }
                }
            }
            return node;
        }
        
        private SyntaxToken ParseOptionalArguments()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Plus:
                case SyntaxTokenKind.Minus:
                case SyntaxTokenKind.Bang:
                case SyntaxTokenKind.Tilde:
                case SyntaxTokenKind.Int:
                case SyntaxTokenKind.Float:
                case SyntaxTokenKind.Bool:
                case SyntaxTokenKind.String:
                case SyntaxTokenKind.Identifier:
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseArguments();
                    return CreateNode(SyntaxTokenKind.OptionalArguments, n0);
                }
                default: return new SyntaxNode(SyntaxTokenKind.Epsilon, ImmutableArray<SyntaxToken>.Empty);
            }
        }
        
        private SyntaxToken ParseArguments()
        {
            var n0 = ParseExpression();
            SyntaxToken node = CreateNode(SyntaxTokenKind.Arguments, n0);
            while (current.Kind == SyntaxTokenKind.Comma)
            {
                var x0 = Expect(SyntaxTokenKind.Comma, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
                var x1 = ParseExpression();
                node = CreateNode(SyntaxTokenKind.Arguments, node, x0, x1);
            }
            return node;
        }
        
        private SyntaxToken ParsePrimary()
        {
            switch (current.Kind)
            {
                case SyntaxTokenKind.Int:
                {
                    var n0 = Expect(SyntaxTokenKind.Int);
                    return CreateNode(SyntaxTokenKind.Primary, n0);
                }
                case SyntaxTokenKind.Float:
                {
                    var n0 = Expect(SyntaxTokenKind.Float);
                    return CreateNode(SyntaxTokenKind.Primary, n0);
                }
                case SyntaxTokenKind.Bool:
                {
                    var n0 = Expect(SyntaxTokenKind.Bool);
                    return CreateNode(SyntaxTokenKind.Primary, n0);
                }
                case SyntaxTokenKind.String:
                {
                    var n0 = Expect(SyntaxTokenKind.String);
                    return CreateNode(SyntaxTokenKind.Primary, n0);
                }
                case SyntaxTokenKind.Identifier:
                {
                    var n0 = Expect(SyntaxTokenKind.Identifier);
                    return CreateNode(SyntaxTokenKind.Primary, n0);
                }
                case SyntaxTokenKind.LeftParen:
                {
                    var n0 = ParseParenthesized();
                    return CreateNode(SyntaxTokenKind.Primary, n0);
                }
                default:
                {
                    var erroneous = current;
                    Unexpected(SyntaxTokenKind.Identifier, SyntaxTokenKind.String, SyntaxTokenKind.Bool, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.LeftParen);
                    SkipInput(SyntaxTokenKind.Identifier, SyntaxTokenKind.String, SyntaxTokenKind.Bool, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.LeftParen, SyntaxTokenKind.LeftCurly, SyntaxTokenKind.RightSquare, SyntaxTokenKind.LeftSquare, SyntaxTokenKind.RightParen, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Hat, SyntaxTokenKind.Pipe, SyntaxTokenKind.DoublePipe, SyntaxTokenKind.Ampersand, SyntaxTokenKind.DoubleAmpersand, SyntaxTokenKind.Percent, SyntaxTokenKind.Slash, SyntaxTokenKind.Star, SyntaxTokenKind.Minus, SyntaxTokenKind.Plus, SyntaxTokenKind.Equal, SyntaxTokenKind.HatEqual, SyntaxTokenKind.PipeEqual, SyntaxTokenKind.AmpersandEqual, SyntaxTokenKind.PercentEqual, SyntaxTokenKind.SlashEqual, SyntaxTokenKind.StarEqual, SyntaxTokenKind.MinusEqual, SyntaxTokenKind.PlusEqual, SyntaxTokenKind.NotEqual, SyntaxTokenKind.DoubleEqual, SyntaxTokenKind.Greater, SyntaxTokenKind.GreaterEqual, SyntaxTokenKind.Less, SyntaxTokenKind.LessEqual, SyntaxTokenKind.SemiColon, SyntaxTokenKind.Dot, SyntaxTokenKind.Comma, SyntaxTokenKind.Epsilon);
                    if (current.Kind == SyntaxTokenKind.Identifier || current.Kind == SyntaxTokenKind.String || current.Kind == SyntaxTokenKind.Bool || current.Kind == SyntaxTokenKind.Int || current.Kind == SyntaxTokenKind.Float || current.Kind == SyntaxTokenKind.LeftParen)
                    {
                        return ParsePrimary();
                    }
                    return new SyntaxNode(SyntaxTokenKind.Error, ImmutableArray.Create<SyntaxToken>(erroneous));
                }
            }
        }
        
        private SyntaxToken ParseParenthesized()
        {
            var n0 = Expect(SyntaxTokenKind.LeftParen, SyntaxTokenKind.Plus, SyntaxTokenKind.Minus, SyntaxTokenKind.Bang, SyntaxTokenKind.Tilde, SyntaxTokenKind.Int, SyntaxTokenKind.Float, SyntaxTokenKind.Bool, SyntaxTokenKind.String, SyntaxTokenKind.Identifier, SyntaxTokenKind.LeftParen);
            var n1 = ParseExpression();
            var n2 = Expect(SyntaxTokenKind.RightParen);
            return CreateNode(SyntaxTokenKind.Parenthesized, n0, n1, n2);
        }
    }
}
