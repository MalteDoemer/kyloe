namespace CodeGen
{
    public sealed class ElseStatement : ICodeStatement
    {
        public ElseStatement()
        {
            Body = new BlockStatement();
        }

        public BlockStatement Body { get; }

        public ElseStatement AddLine(string code)
        {
            Body.AddLine(code);
            return this;
        }

        public ElseStatement AddLines(IEnumerable<string> code)
        {
            Body.AddLines(code);
            return this;
        }

        public ElseStatement AddStatement(ICodeStatement statement)
        {
            Body.Add(statement);
            return this;
        }

        public ElseStatement AddStatementRange(IEnumerable<ICodeStatement> statements)
        {
            Body.AddRange(statements);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            writer.Write("else ");
            Body.Generate(writer);
        }
    }
}