namespace CodeGen
{
    public sealed class IfStatement : ICodeStatement
    {
        public IfStatement(string condition)
        {
            Condition = condition;
            Body = new BlockStatement();
        }

        public string Condition { get; }
        public BlockStatement Body { get; }

        public IfStatement AddLine(string code)
        {
            Body.AddLine(code);
            return this;
        }

        public IfStatement AddLines(IEnumerable<string> code)
        {
            Body.AddLines(code);
            return this;
        }

        public IfStatement AddStatement(ICodeStatement statement)
        {
            Body.Add(statement);
            return this;
        }

        public IfStatement AddStatementRange(IEnumerable<ICodeStatement> statements)
        {
            Body.AddRange(statements);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            writer.Write("if (");
            writer.Write(Condition);
            writer.Write(")");
            Body.Generate(writer);
        }
    }
}