namespace CodeGen
{
    public sealed class Field : IClassItem
    {
        public Field(AccessModifier accessModifiers, InheritanceModifier inheritanceModifiers, bool @readonly, string type, string name)
        {
            AccessModifiers = accessModifiers;
            InheritanceModifiers = inheritanceModifiers;
            Readonly = @readonly;
            Type = type;
            Name = name;
        }

        public AccessModifier AccessModifiers { get; }
        public InheritanceModifier InheritanceModifiers { get; }
        public bool Readonly { get; }
        public string Type { get; }
        public string Name { get; }

        public void Generate(GeneratorWriter writer)
        {
            writer.WriteAccessModifiers(AccessModifiers);
            writer.WriteInheritanceModifiers(InheritanceModifiers);

            if (Readonly)
                writer.Write("readonly ");

            writer.Write(Type);
            writer.Write(" ");
            writer.Write(Name);
            writer.Write(";");
        }
    }
}