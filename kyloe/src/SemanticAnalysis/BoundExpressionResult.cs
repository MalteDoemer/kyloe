using Mono.Cecil;

namespace Kyloe.Semantics
{
    internal class BoundExpressionResult
    {
        private readonly Namespace? @namespace;
        private readonly TypeReference? typeReference;
        private readonly bool isInstance;

        public BoundExpressionResult(Namespace @namespace)
        {
            this.@namespace = @namespace;
        }

        public BoundExpressionResult(TypeReference typeReference, bool isInstance)
        {
            this.typeReference = typeReference;
            this.isInstance = isInstance;
        }


        public bool IsNamespace => @namespace is not null;
        public bool IsTypeName => typeReference is not null && !isInstance;
        public bool IsTypeInstance => typeReference is not null && isInstance;
    }
}