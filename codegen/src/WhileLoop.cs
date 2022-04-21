namespace CodeGen
{
    public sealed class WhileLoop : ICodeStatement
    {
        public WhileLoop(string condition)
        {
            Condition = condition;
            Body = new BlockStatement();
        }

        public string Condition { get; }
        public BlockStatement Body { get; }

        public WhileLoop AddLine(string code)
        {
            Body.AddLine(code);
            return this;
        }

        public WhileLoop AddLines(IEnumerable<string> code)
        {
            Body.AddLines(code);
            return this;
        }

        public WhileLoop AddStatement(ICodeStatement statement)
        {
            Body.Add(statement);
            return this;
        }

        public WhileLoop AddStatementRange(IEnumerable<ICodeStatement> statements)
        {
            Body.AddRange(statements);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            writer.Write("while (");
            writer.Write(Condition);
            writer.Write(")");
            Body.Generate(writer);
        }
    }
}