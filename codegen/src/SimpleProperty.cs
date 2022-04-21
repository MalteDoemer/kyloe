namespace CodeGen
{
    public sealed class SimpleProperty : IClassItem
    {
        public SimpleProperty(AccessModifier accessModifiers, InheritanceModifier inheritanceModifiers, string type, string name, string code)
        {
            AccessModifiers = accessModifiers;
            InheritanceModifiers = inheritanceModifiers;
            Type = type;
            Name = name;
            Code = code;
        }

        public AccessModifier AccessModifiers { get; }
        public InheritanceModifier InheritanceModifiers { get; }
        public string Type { get; }
        public string Name { get; }
        public string Code { get; }

        public void Generate(GeneratorWriter writer)
        {
            writer.WriteAccessModifiers(AccessModifiers);
            writer.WriteInheritanceModifiers(InheritanceModifiers);
            writer.Write(Type);
            writer.Write(" ");
            writer.Write(Name);
            writer.Write(" ");
            writer.Write(Code);
        }
    }
}