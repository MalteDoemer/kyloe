using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kyloe.Symbols
{
    enum TypeKind
    {
        ErrorType,
        NamespaceType,
        ClassType,
        MethodType,
        MethodGroupType,
        ArrayType,
        PointerType,
        ByRefType,
    }

    abstract class TypeSpecifier : IEquatable<TypeSpecifier>
    {
        public abstract TypeKind Kind { get; }

        public abstract AccessModifiers AccessModifiers { get; }

        public abstract IReadOnlySymbolScope? ReadOnlyScope { get; }

        public abstract bool Equals(TypeSpecifier? other);

        public abstract string FullName();
    }

    sealed class ErrorType : TypeSpecifier
    {
        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override TypeKind Kind => TypeKind.ErrorType;

        public override AccessModifiers AccessModifiers => AccessModifiers.Public;

        public override bool Equals(TypeSpecifier? other) => other is ErrorType;

        public override string FullName() => "<error-type>";
    }

    sealed class NamespaceType : TypeSpecifier
    {
        public NamespaceType(string name, NamespaceType? parent)
        {
            Name = name;
            Parent = parent;
            Scope = new SymbolScope();
        }

        public string Name { get; }
        public NamespaceType? Parent { get; }
        public SymbolScope Scope { get; }

        public override TypeKind Kind => TypeKind.NamespaceType;

        public override IReadOnlySymbolScope? ReadOnlyScope => Scope;

        public override AccessModifiers AccessModifiers => AccessModifiers.Public;

        public override bool Equals(TypeSpecifier? other) => object.ReferenceEquals(this, other);

        public override string FullName() => (Parent is null ? "" : Parent.FullName() + ".") + Name;
    }

    sealed class ClassType : TypeSpecifier
    {
        public ClassType(string name, AccessModifiers accessModifiers, TypeSpecifier parent)
        {
            Name = name;
            AccessModifiers = accessModifiers;
            Parent = parent;
            Scope = new SymbolScope();
        }

        public string Name { get; }
        public TypeSpecifier Parent { get; }
        public SymbolScope Scope { get; }

        public override TypeKind Kind => TypeKind.ClassType;

        public override IReadOnlySymbolScope? ReadOnlyScope => Scope;

        public override AccessModifiers AccessModifiers { get; }

        public override bool Equals(TypeSpecifier? other) => object.ReferenceEquals(this, other);

        public override string FullName()
        {
            var parentName = Parent.FullName();

            if (parentName == "")
                return Name;

            return parentName + "." + Name;
        }
    }

    sealed class MethodType : TypeSpecifier
    {
        public MethodType(string name, AccessModifiers accessModifiers, TypeSpecifier parent, bool isStatic, TypeSpecifier returnType)
        {
            Name = name;
            AccessModifiers = accessModifiers;
            Parent = parent;
            IsStatic = isStatic;
            ReturnType = returnType;
            ParameterTypes = new List<TypeSpecifier>();
        }

        public string Name { get; }
        public TypeSpecifier Parent { get; }

        public bool IsStatic { get; }
        public TypeSpecifier ReturnType { get; }
        public List<TypeSpecifier> ParameterTypes { get; }

        public override TypeKind Kind => TypeKind.MethodType;

        public override AccessModifiers AccessModifiers { get; }

        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override bool Equals(TypeSpecifier? other) => object.ReferenceEquals(this, other);

        public override string FullName()
        {
            var builder = new StringBuilder();

            var parentName = Parent.FullName();


            builder.Append("func ");
            if (parentName != "")
                builder.Append(parentName).Append('.');
            builder.Append(Name);
            builder.Append('(');
            builder.AppendJoin(',', ParameterTypes.Select(param => param.FullName()));
            builder.Append(") -> ");
            builder.Append(ReturnType.FullName());

            return builder.ToString();
        }
    }

    sealed class MethodGroupType : TypeSpecifier
    {
        public MethodGroupType(string name, TypeSpecifier parent)
        {
            Name = name;
            Parent = parent;
            Methods = new List<MethodType>();
        }

        public string Name { get; }
        public TypeSpecifier Parent { get; }
        public List<MethodType> Methods { get; }

        public override TypeKind Kind => TypeKind.MethodGroupType;

        public override IReadOnlySymbolScope? ReadOnlyScope => null;

        public override AccessModifiers AccessModifiers => AccessModifiers.Public;

        public override bool Equals(TypeSpecifier? other) => object.ReferenceEquals(this, other);

        public override string FullName()
        {
            var builder = new StringBuilder();
            var parentName = Parent.FullName();

            builder.Append("func ");
            if (parentName != "")
                builder.Append(parentName).Append('.');
            builder.Append(Name);
            builder.Append("(...)");

            return builder.ToString();
        }
    }

    sealed class ArrayType : TypeSpecifier
    {
        public ArrayType(TypeSpecifier elementType)
        {
            ElementType = elementType;
            Scope = new SymbolScope();
        }

        public TypeSpecifier ElementType { get; }

        public SymbolScope Scope { get; }

        public override TypeKind Kind => TypeKind.ArrayType;

        public override IReadOnlySymbolScope? ReadOnlyScope => Scope;

        public override AccessModifiers AccessModifiers => ElementType.AccessModifiers;

        public override bool Equals(TypeSpecifier? other) => other is ArrayType array && array.ElementType.Equals(ElementType);

        public override string FullName() => ElementType.FullName() + "[]";
    }

    sealed class PointerType : TypeSpecifier
    {
        public PointerType(TypeSpecifier elementType)
        {
            ElementType = elementType;
            Scope = new SymbolScope();
        }

        public TypeSpecifier ElementType { get; }

        public SymbolScope Scope { get; }

        public override TypeKind Kind => TypeKind.PointerType;

        public override IReadOnlySymbolScope? ReadOnlyScope => Scope;

        public override AccessModifiers AccessModifiers => ElementType.AccessModifiers;

        public override bool Equals(TypeSpecifier? other) => other is PointerType pointer && pointer.ElementType.Equals(ElementType);

        public override string FullName() => ElementType.FullName() + "*";
    }


    sealed class ByRefType : TypeSpecifier
    {
        public ByRefType(TypeSpecifier elementType)
        {
            ElementType = elementType;
        }

        public TypeSpecifier ElementType { get; }

        public override IReadOnlySymbolScope? ReadOnlyScope => ElementType.ReadOnlyScope;

        public override TypeKind Kind => TypeKind.ByRefType;

        public override AccessModifiers AccessModifiers => ElementType.AccessModifiers;

        public override bool Equals(TypeSpecifier? other) => other is ByRefType byref && byref.ElementType.Equals(ElementType);

        public override string FullName() => ElementType.FullName() + "&";
    }
}