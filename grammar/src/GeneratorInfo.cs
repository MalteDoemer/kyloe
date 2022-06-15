using CodeGen;

namespace Kyloe.Grammar
{
    public struct ClassInfo
    {
        public string Name { get; set; }
        public AccessModifier AccessModifiers { get; set; }

        public ClassInfo(string name, AccessModifier accessModifier)
        {
            Name = name;
            AccessModifiers = accessModifier;
        }
    }

    public struct GeneratorInfo
    {
        public string Namespace { get; set; }
        public ClassInfo TokenKindEnum { get; set; }
        public ClassInfo ExtensionClass { get; set; }
        public ClassInfo TokenClass { get; set; }
        public ClassInfo TerminalClass { get; set; }
        public ClassInfo NodeClass { get; set; }
        public ClassInfo TextClass { get; set; }
        public ClassInfo LocationClass { get; set; }
        public ClassInfo ErrorCollectorClass { get; set; }
        public ClassInfo LexerClass { get; set; }
        public ClassInfo ParserClass { get; set; }
    }
}