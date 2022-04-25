namespace CodeGen
{
    public sealed class Method : IClassItem
    {
        public Method(AccessModifier accessModifiers, InheritanceModifier inheritanceModifiers, string type, string? name)
        {
            AccessModifiers = accessModifiers;
            InheritanceModifiers = inheritanceModifiers;
            Type = type;
            Name = name;
            Arguments = new Arguments();
            Body = new BlockStatement();
        }

        public AccessModifier AccessModifiers { get; }
        public InheritanceModifier InheritanceModifiers { get; }
        public string Type { get; }
        public string? Name { get; }
        public Arguments Arguments { get; }
        public BlockStatement Body { get; }

        public Method AddArg(string arg)
        {
            Arguments.Add(arg);
            return this;
        }

        public Method AddArgRange(IEnumerable<string> args)
        {
            Arguments.AddRange(args);
            return this;
        }

        public Method AddLine(string code)
        {
            Body.AddLine(code);
            return this;
        }

        public Method AddLines(IEnumerable<string> code)
        {
            Body.AddLines(code);
            return this;
        }

        public Method AddStatement(ICodeStatement statement)
        {
            Body.Add(statement);
            return this;
        }

        public Method AddStatementRange(IEnumerable<ICodeStatement> statements)
        {
            Body.AddRange(statements);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            writer.WriteAccessModifiers(AccessModifiers);
            writer.WriteInheritanceModifiers(InheritanceModifiers);
            writer.Write(Type);

            if (!string.IsNullOrEmpty(Name))
            {
                writer.Write(" ");
                writer.Write(Name);
            }

            Arguments.Generate(writer);

            if (InheritanceModifiers == InheritanceModifier.Abstract)
            {
                writer.Write(";");
                return;
            }

            Body.Generate(writer);
        }
    }
}