namespace CodeGen
{
    public sealed class Argument : IGeneratorItem
    {
        public Argument(string type, string name)
        {
            Type = type;
            Name = name;
        }

        public string Type { get; }
        public string Name { get; }

        public void Generate(GeneratorWriter writer)
        {
            writer.Write(Type);
            writer.Write(" ");
            writer.Write(Name);
        }
    }
}