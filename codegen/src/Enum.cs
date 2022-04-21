namespace CodeGen
{
    public sealed class Enum : INamespaceItem, IClassItem
    {
        public Enum(AccessModifier accessModifiers, string name, string? type)
        {
            AccessModifiers = accessModifiers;
            Name = name;
            Type = type;
            Members = new List<EnumMember>();
        }

        public AccessModifier AccessModifiers { get; }
        public string Name { get; }
        public string? Type { get; }
        public List<EnumMember> Members { get; }

        public Enum Add(EnumMember member)
        {
            Members.Add(member);
            return this;
        }

        public Enum AddRange(IEnumerable<EnumMember> members)
        {
            Members.AddRange(members);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            writer.WriteAccessModifiers(AccessModifiers);
            writer.Write("enum ");
            writer.Write(Name);

            if (!string.IsNullOrEmpty(Type))
            {
                writer.Write(" : ");
                writer.Write(Type);
            }

            writer.EnterScope();

            foreach (var member in Members)
            {
                writer.WriteNewLine();
                member.Generate(writer);
            }

            writer.ExitScope();
        }
    }
}