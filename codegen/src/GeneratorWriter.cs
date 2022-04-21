namespace CodeGen
{
    public sealed class GeneratorWriter
    {
        private int current = 0;

        public GeneratorWriter(TextWriter writer, int spaceSize)
        {
            Writer = writer;
            SpaceSize = spaceSize;
        }

        public TextWriter Writer { get; }
        public int SpaceSize { get; }

        public void Write(string str)
        {
            Writer.Write(str);
        }

        public void WriteNewLine()
        {
            Writer.WriteLine();
            if (current > 0)
                Writer.Write(new string(' ', current));
        }

        public void WriteAccessModifiers(AccessModifier modifiers)
        {
            switch (modifiers)
            {
                case AccessModifier.Public:
                    Write("public "); break;
                case AccessModifier.Protected:
                    Write("protected "); break;
                case AccessModifier.Private:
                    Write("private "); break;
                case AccessModifier.Internal:
                    Write("internal "); break;
                case AccessModifier.None:
                    break;
                default:
                    throw new Exception($"unexpected access modifier: {modifiers}");
            }
        }

        public void WriteInheritanceModifiers(InheritanceModifier modifiers)
        {
            switch (modifiers)
            {
                case InheritanceModifier.Static:
                    Write("static "); break;
                case InheritanceModifier.Abstract:
                    Write("abstract "); break;
                case InheritanceModifier.Virtual:
                    Write("virtual "); break;
                case InheritanceModifier.Sealed:
                    Write("sealed "); break;
                case InheritanceModifier.Override:
                    Write("override "); break;
                case InheritanceModifier.None:
                    break;
                default:
                    throw new Exception($"unexpected modifier: {modifiers}");
            }
        }

        public void EnterScope()
        {
            WriteNewLine();
            Indent();
            Write("{");
        }

        public void ExitScope()
        {
            Unindent();
            WriteNewLine();
            Write("}");
        }

        public void Indent()
        {
            current += SpaceSize;
        }

        public void Unindent()
        {
            current -= SpaceSize;
        }
    }
}