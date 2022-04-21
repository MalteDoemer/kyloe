namespace CodeGen
{
    public sealed class ElseIfStatement : ICodeStatement
    {
        public ElseIfStatement(string condition)
        {
            Condition = condition;
            Body = new BlockStatement();
        }

        public string Condition { get; }
        public BlockStatement Body { get; }

        public ElseIfStatement AddLine(string code)
        {
            Body.AddLine(code);
            return this;
        }

        public ElseIfStatement AddLines(IEnumerable<string> code)
        {
            Body.AddLines(code);
            return this;
        }

        public ElseIfStatement AddStatement(ICodeStatement statement)
        {
            Body.Add(statement);
            return this;
        }

        public ElseIfStatement AddStatementRange(IEnumerable<ICodeStatement> statements)
        {
            Body.AddRange(statements);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            writer.Write("else if (");
            writer.Write(Condition);
            writer.Write(")");
            Body.Generate(writer);
        }
    }
}