using Kyloe.Semantics;

namespace Kyloe.Symbols
{
    internal sealed class OperationSymbol : Symbol
    {
        public OperationSymbol(BoundOperation operation, FunctionGroupType functionGroup)
        {
            Operation = operation;
            FunctionGroup = functionGroup;
        }

        public BoundOperation Operation { get; }

        public FunctionGroupType FunctionGroup { get; }

        public override string Name => SemanticInfo.GetFunctionNameFromOperation(Operation);

        public override SymbolKind Kind => SymbolKind.OperationSymbol;

        public override TypeSpecifier Type => FunctionGroup;

        public override ValueCategory ValueCategory => ValueCategory.None;
    }
}