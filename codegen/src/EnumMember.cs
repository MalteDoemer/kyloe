namespace CodeGen
{
    public sealed class EnumMember : IGeneratorItem
    {
        public EnumMember(string name, string? value = null)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public string? Value { get; }

        public void Generate(GeneratorWriter writer)
        {
            writer.Write(Name);

            if (!string.IsNullOrEmpty(Value))
            {
                writer.Write(" = ");
                writer.Write(Value);
            }

            writer.Write(",");
        }
    }
}