using System.Collections.Generic;
using System.IO;

namespace Kyloe
{
    abstract class SyntaxNode
    {


        public abstract void PrettyWrite(TextWriter writer, string indent);

    }

    class MalformedSyntaxNode : SyntaxNode
    {
        public MalformedSyntaxNode(SyntaxToken token)
        {
            Token = token;
        }

        public SyntaxToken Token { get; }

        public override void PrettyWrite(TextWriter writer, string indent)
        {
            writer.Write(indent);
            writer.Write(nameof(MalformedSyntaxNode));
            writer.WriteLine(":");
            writer.Write(indent + "    ");
            writer.WriteLine(Token);
        }
    }

    class LiteralSyntaxNode : SyntaxNode
    {
        public LiteralSyntaxNode(SyntaxToken literalToken)
        {
            LiteralToken = literalToken;
        }

        public SyntaxToken LiteralToken { get; }

        public override void PrettyWrite(TextWriter writer, string indent)
        {
            writer.Write(indent);
            writer.Write(nameof(LiteralSyntaxNode));
            writer.WriteLine(":");
            writer.Write(indent + "    ");
            writer.WriteLine(LiteralToken);
        }
    }

    class BinaryExpressionNode : SyntaxNode
    {
        public BinaryExpressionNode(SyntaxToken operatorToken, SyntaxNode leftChild, SyntaxNode rightChild)
        {
            OperatorToken = operatorToken;
            LeftChild = leftChild;
            RightChild = rightChild;
        }

        public SyntaxToken OperatorToken { get; }
        public SyntaxNode LeftChild { get; }
        public SyntaxNode RightChild { get; }

        public override void PrettyWrite(TextWriter writer, string indent)
        {

            var nextIndent = indent + "    ";

            writer.Write(indent);
            writer.Write(nameof(BinaryExpressionNode));
            writer.WriteLine(":");
            LeftChild.PrettyWrite(writer, nextIndent);
            writer.Write(nextIndent);
            writer.WriteLine(OperatorToken);
            RightChild.PrettyWrite(writer, nextIndent);
        }
    }


    class Parser
    {
        private Lexer lexer;

        private SyntaxToken current;
        private SyntaxToken next;

        public Parser(string text)
        {
            lexer = new Lexer(text);
            current = lexer.NextToken();
            next = lexer.NextToken();
        }

        /// Returns the current Token and then advances to the next one.
        private SyntaxToken Advance()
        {
            var temp = current;
            current = next;
            next = lexer.NextToken();
            return temp;
        }

        public SyntaxNode Parse()
        {
            return ParseExpression();
        }

        private SyntaxNode ParseExpression()
        {
            return ParseFactor();
        }

        private SyntaxNode ParseFactor()
        {
            if (current.Type.IsLiteralToken())
            {
                return new LiteralSyntaxNode(Advance());
            }
            else if (current.Type == SyntaxTokenType.LeftParen)
            {
                Advance(); // skip the left parenthesis
                var expr = ParseExpression();
                Expect(SyntaxTokenType.RightParen); // skip the right parenthesis
                return expr;
            }
            else if (current.Type == SyntaxTokenType.Identifier)
            {
                throw new System.NotImplementedException();
            }
            else
            {
                return new MalformedSyntaxNode(Advance());
            }
        }

        private SyntaxToken Expect(SyntaxTokenType type)
        {
            if (current.Type != type)
            {
                throw new System.NotImplementedException();
            }

            return Advance();
        }
    }
}