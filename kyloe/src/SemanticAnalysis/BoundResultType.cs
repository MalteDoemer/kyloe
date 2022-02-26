using System.Diagnostics;
using Mono.Cecil;

namespace Kyloe.Semantics
{
    internal class BoundResultType
    {
        enum Kind
        {
            Namespace,
            TypeValue,
            TypeName,
            Error,
        }

        public static readonly BoundResultType ErrorResult = new BoundResultType(Kind.Error);

        private readonly Namespace? @namespace;
        private readonly TypeReference? typeReference;
        private readonly Kind kind;

        private BoundResultType(Kind kind)
        {
            this.kind = kind;
        }

        public BoundResultType(Namespace @namespace) : this(Kind.Namespace)
        {
            this.@namespace = @namespace;
        }

        public BoundResultType(TypeReference typeReference, bool isValue) : this(isValue ? Kind.TypeValue : Kind.TypeName)
        {
            this.typeReference = typeReference;
        }

        public bool IsNamespace => kind == Kind.Namespace;
        public bool IsTypeName => kind == Kind.TypeName;
        public bool IsTypeValue => kind == Kind.TypeValue;
        public bool IsError => kind == Kind.Error;

        public Namespace Namespace
        {
            get
            {
                Debug.Assert(IsNamespace);
                return @namespace!;
            }
        }

        public TypeReference TypeName
        {
            get
            {
                Debug.Assert(IsTypeName);
                return typeReference!;
            }
        }

        public TypeReference TypeValue
        {
            get
            {
                Debug.Assert(IsTypeValue);
                return typeReference!;
            }
        }


        public override string? ToString()
        {
            switch (kind)
            {
                case Kind.Namespace:
                    return Namespace.ToString();
                case Kind.TypeName:
                case Kind.TypeValue:
                    return typeReference!.FullName;
                case Kind.Error:
                    return "<error-type>";
                default:
                    throw new System.Exception($"Unexpected result kind: {kind}");
            }
        }
    }
}