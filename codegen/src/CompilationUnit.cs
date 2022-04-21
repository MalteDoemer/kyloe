namespace CodeGen
{

    public sealed class CompilationUnit : IGeneratorItem
    {
        public CompilationUnit()
        {
            UsingDirectives = new List<string>();
            Items = new List<INamespaceItem>();
        }

        public List<string> UsingDirectives { get; }
        public List<INamespaceItem> Items { get; }

        public CompilationUnit AddUsing(string line)
        {
            UsingDirectives.Add(line);
            return this;
        }

        public CompilationUnit AddUsingsRange(IEnumerable<string> lines)
        {
            UsingDirectives.AddRange(lines);
            return this;
        }

        public CompilationUnit AddItem(INamespaceItem item)
        {
            Items.Add(item);
            return this;
        }

        public CompilationUnit AddItemRange(IEnumerable<INamespaceItem> items)
        {
            Items.AddRange(items);
            return this;
        }

        public void Generate(GeneratorWriter writer)
        {
            foreach (var usingDirective in UsingDirectives)
            {
                writer.Write(usingDirective);
                writer.WriteNewLine();
            }

            foreach (var item in Items)
            {
                writer.WriteNewLine();
                item.Generate(writer);
                writer.WriteNewLine();
            }
        }
    }
}