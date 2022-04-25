namespace CodeGen
{
    public sealed class Arguments : IGeneratorItem
    {
        public Arguments()
        {
            Args = new List<string>();
        }

        public List<string> Args { get; }

        public Arguments Add(string arg)
        {
            Args.Add(arg);
            return this;
        }

        public Arguments AddRange(IEnumerable<string> args)
        {
            Args.AddRange(args);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            writer.Write("(");
            foreach (var (i, arg) in Args.EnumerateIndex())
            {
                // arg.Generate(writer);
                writer.Write(arg);

                if (i != Args.Count - 1)
                    writer.Write(", ");
            }
            writer.Write(")");
        }
    }
}