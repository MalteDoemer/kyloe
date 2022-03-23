using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    internal sealed class OperationSymbol : Symbol
    {
        public OperationSymbol(BoundOperation operation, MethodGroupType methodGroup)
        {
            Operation = operation;
            MethodGroup = methodGroup;
        }

        public BoundOperation Operation { get; }

        public MethodGroupType MethodGroup { get; }

        public override string Name => SemanticInfo.GetMethodNameFromOperation(Operation);

        public override SymbolKind Kind => SymbolKind.OperationSymbol;

        public override TypeSpecifier Type => MethodGroup;
    }
}