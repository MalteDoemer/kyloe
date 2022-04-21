namespace CodeGen
{
    public sealed class Namespace : INamespaceItem
    {
        public Namespace(string name)
        {
            Name = name;
            Items = new List<INamespaceItem>();
        }

        public string Name { get; }
        public List<INamespaceItem> Items { get; }

        public Namespace Add(INamespaceItem item)
        {
            Items.Add(item);
            return this;
        }

        public Namespace AddRange(IEnumerable<INamespaceItem> items)
        {
            Items.AddRange(items);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            writer.Write("namespace ");
            writer.Write(Name);
            writer.EnterScope();

            foreach (var (i, item) in Items.EnumerateIndex())
            {
                writer.WriteNewLine();
                item.Generate(writer);

                if (i != Items.Count - 1)
                    writer.WriteNewLine();
            }

            writer.ExitScope();
        }
    }
}