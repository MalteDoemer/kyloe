namespace CodeGen
{
    public sealed class Class : INamespaceItem, IClassItem
    {
        public Class(AccessModifier accessModifiers, InheritanceModifier inheritanceModifiers, string name, string? baseClass = null)
        {
            AccessModifiers = accessModifiers;
            InheritanceModifiers = inheritanceModifiers;
            Name = name;
            BaseClass = baseClass;
            ClassItems = new List<IClassItem>();
        }

        public AccessModifier AccessModifiers { get; }
        public InheritanceModifier InheritanceModifiers { get; }
        public string Name { get; }
        public string? BaseClass { get; }
        public List<IClassItem> ClassItems { get; }

        public Class Add(IClassItem item)
        {
            ClassItems.Add(item);
            return this;
        }

        public Class AddRange(IEnumerable<IClassItem> items)
        {
            ClassItems.AddRange(items);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            writer.WriteAccessModifiers(AccessModifiers);
            writer.WriteInheritanceModifiers(InheritanceModifiers);
            writer.Write("class ");
            writer.Write(Name);

            if (!string.IsNullOrEmpty(BaseClass))
            {
                writer.Write(" : ");
                writer.Write(BaseClass);
            }

            writer.EnterScope();

            foreach (var (i, item) in ClassItems.EnumerateIndex())
            {
                writer.WriteNewLine();
                item.Generate(writer);

                if (i != ClassItems.Count - 1)
                    writer.WriteNewLine();
            }

            writer.ExitScope();
        }
    }
}