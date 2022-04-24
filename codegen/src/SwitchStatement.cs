namespace CodeGen
{
    public sealed class SwitchStatement : ICodeStatement
    {
        public SwitchStatement(string @switch)
        {
            Switch = @switch;
            Body = new BlockStatement();
        }

        private sealed class CaseStatement : ICodeStatement
        {
            public CaseStatement(string @case)
            {
                Case = @case;
            }

            public string Case { get; }

            public void Generate(GeneratorWriter writer)
            {
                writer.Write("case ");
                writer.Write(Case);
                writer.Write(":");
            }
        }

        public string Switch { get; }
        public BlockStatement Body { get; }

        public SwitchStatement AddCase(string @case)
        {
            Body.Add(new CaseStatement(@case));
            return this;
        }

        public SwitchStatement AddCases(IEnumerable<string> cases)
        {
            Body.AddRange(cases.Select(c => new CaseStatement(c)));
            return this;
        }

        public SwitchStatement AddLine(string code)
        {
            Body.AddLine(code);
            return this;
        }

        public SwitchStatement AddLines(IEnumerable<string> code)
        {
            Body.AddLines(code);
            return this;
        }

        public SwitchStatement AddStatement(ICodeStatement statement)
        {
            Body.Add(statement);
            return this;
        }

        public SwitchStatement AddStatementRange(IEnumerable<ICodeStatement> statements)
        {
            Body.AddRange(statements);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            writer.Write("switch (");
            writer.Write(Switch);
            writer.Write(")");
            Body.Generate(writer);
        }
    }
}