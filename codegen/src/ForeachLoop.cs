namespace CodeGen
{
    public sealed class ForeachLoop : ICodeStatement
    {
        public ForeachLoop(string clause)
        {
            this.clause = clause;
            Body = new BlockStatement();
        }

        public string clause { get; }
        public BlockStatement Body { get; }

        public ForeachLoop AddLine(string code)
        {
            Body.AddLine(code);
            return this;
        }

        public ForeachLoop AddLines(IEnumerable<string> code)
        {
            Body.AddLines(code);
            return this;
        }

        public ForeachLoop AddStatement(ICodeStatement statement)
        {
            Body.Add(statement);
            return this;
        }

        public ForeachLoop AddStatementRange(IEnumerable<ICodeStatement> statements)
        {
            Body.AddRange(statements);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            writer.Write("foreach (");
            writer.Write(clause);
            writer.Write(")");
            Body.Generate(writer);
        }
    }
}