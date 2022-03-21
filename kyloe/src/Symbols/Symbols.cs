using System.Collections.Generic;

namespace Kyloe.Symbols
{
    enum SymbolKind
    {
        NamespaceSymbol,
        TypeNameSymbol,
        MethodGroupSymbol,
        LocalVariableSymbol,
        FieldSymbol,
        PropertySymbol,
    }

    interface IReadOnlySymbolScope
    {
        Symbol? LookupSymbol(string name);
    }

    interface ISymbolScope : IReadOnlySymbolScope
    {
        bool DeclareSymbol(Symbol symbol);
    }

    struct SymbolScope : ISymbolScope
    {
        private readonly Dictionary<string, Symbol> symbols;

        public SymbolScope()
        {
            symbols = new Dictionary<string, Symbol>();
        }

        public bool DeclareSymbol(Symbol symbol)
        {
            return symbols.TryAdd(symbol.Name, symbol);
        }

        public Symbol? LookupSymbol(string name)
        {
            return symbols.GetValueOrDefault(name);
        }
    }

    abstract class Symbol
    {
        public abstract string Name { get; }
        public abstract SymbolKind Kind { get; }
        public abstract TypeSpecifier Type { get; }
    }

    sealed class NamespaceSymbol : Symbol
    {
        public NamespaceSymbol(NamespaceType namespaceType)
        {
            Name = namespaceType.Name;
            Type = namespaceType;
        }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.NamespaceSymbol;

        public override TypeSpecifier Type { get; }
    }

    sealed class TypeNameSymbol : Symbol
    {
        public TypeNameSymbol(ClassType classType)
        {
            Name = classType.Name;
            Type = classType;
        }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.TypeNameSymbol;

        public override TypeSpecifier Type { get; }
    }

    sealed class MethodGroupSymbol : Symbol
    {
        public MethodGroupSymbol(MethodGroupType groupType)
        {
            Name = groupType.Name;
            Type = groupType;
        }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.MethodGroupSymbol;

        public override TypeSpecifier Type { get; }
    }

    sealed class LocalVariableSymbol : Symbol
    {
        public LocalVariableSymbol(string name, TypeSpecifier type, bool isConst)
        {
            Name = name;
            Type = type;
            IsConst = isConst;
        }

        public bool IsConst { get; }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.LocalVariableSymbol;

        public override TypeSpecifier Type { get; }
    }

    sealed class FieldSymbol : Symbol
    {
        public FieldSymbol(string name, TypeSpecifier type, bool isReadonly, bool isStatic, AccessModifiers accessModifiers)
        {
            Name = name;
            Type = type;
            IsReadonly = isReadonly;
            IsStatic = isStatic;
            AccessModifiers = accessModifiers;
        }

        public bool IsReadonly { get; }
        public bool IsStatic { get; }
        public AccessModifiers AccessModifiers { get; }

        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.FieldSymbol;

        public override TypeSpecifier Type { get; }
    }

    sealed class PropertySymbol : Symbol
    {
        public PropertySymbol(string name, TypeSpecifier type, bool isStatic, MethodType? getMethod, MethodType? setMethod)
        {
            Name = name;
            Type = type;
            IsStatic = isStatic;
            GetMethod = getMethod;
            SetMethod = setMethod;
        }

        public bool IsStatic { get; }
        public MethodType? GetMethod { get; }
        public MethodType? SetMethod { get; }
        public override string Name { get; }

        public override SymbolKind Kind => SymbolKind.PropertySymbol;

        public override TypeSpecifier Type { get; }
    }
}