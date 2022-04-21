namespace CodeGen
{
    public sealed class BlockStatement : ICodeStatement
    {
        public BlockStatement()
        {
            Statements = new List<ICodeStatement>();
        }

        private sealed class SimpleStatement : ICodeStatement
        {
            public SimpleStatement(string statement)
            {
                Statement = statement;
            }

            public string Statement { get; }

            public void Generate(GeneratorWriter writer)
            {
                writer.Write(Statement);
            }
        }

        public List<ICodeStatement> Statements { get; }

        public BlockStatement AddLine(string code)
        {
            Statements.Add(new SimpleStatement(code));
            return this;
        }

        public BlockStatement AddLines(IEnumerable<string> code)
        {
            Statements.AddRange(code.Select(line => new SimpleStatement(line)));
            return this;
        }

        public BlockStatement Add(ICodeStatement statement)
        {
            Statements.Add(statement);
            return this;
        }

        public BlockStatement AddRange(IEnumerable<ICodeStatement> statements)
        {
            Statements.AddRange(statements);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            writer.EnterScope();

            foreach (var stmt in Statements)
            {
                writer.WriteNewLine();
                stmt.Generate(writer);
            }

            writer.ExitScope();
        }
    }
}