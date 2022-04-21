namespace CodeGen
{
    public sealed class Arguments : IGeneratorItem
    {
        public Arguments()
        {
            Args = new List<Argument>();
        }

        public List<Argument> Args { get; }

        public Arguments Add(Argument arg)
        {
            Args.Add(arg);
            return this;
        }

        public Arguments AddRange(IEnumerable<Argument> args)
        {
            Args.AddRange(args);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            writer.Write("(");
            foreach (var (i, arg) in Args.EnumerateIndex())
            {
                arg.Generate(writer);

                if (i != Args.Count - 1)
                    writer.Write(", ");
            }
            writer.Write(")");
        }
    }
}