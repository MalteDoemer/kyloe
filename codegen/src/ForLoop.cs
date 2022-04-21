namespace CodeGen
{
    public sealed class ForLoop : ICodeStatement
    {
        public ForLoop(string clause)
        {
            this.clause = clause;
            Body = new BlockStatement();
        }

        public string clause { get; }
        public BlockStatement Body { get; }

        public ForLoop AddLine(string code)
        {
            Body.AddLine(code);
            return this;
        }

        public ForLoop AddLines(IEnumerable<string> code)
        {
            Body.AddLines(code);
            return this;
        }

        public ForLoop AddStatement(ICodeStatement statement)
        {
            Body.Add(statement);
            return this;
        }

        public ForLoop AddStatementRange(IEnumerable<ICodeStatement> statements)
        {
            Body.AddRange(statements);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            writer.Write("for (");
            writer.Write(clause);
            writer.Write(")");
            Body.Generate(writer);
        }
    }
}