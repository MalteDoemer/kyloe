using System.Collections.Generic;
using Kyloe.Utility;
using System.Collections.Immutable;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private sealed class MethodSymbol : IMethodSymbol
        {
            private ITypeSymbol? returnType;
            private bool isStatic;
            private bool isOperator;
            private bool isBuiltinOperator;

            private readonly Dictionary<string, ParameterSymbol> parameters;

            public MethodSymbol(string name)
            {
                Name = name;
                parameters = new Dictionary<string, ParameterSymbol>();
            }

            public string Name { get; }

            public SymbolKind Kind => SymbolKind.MethodSymbol;

            public ITypeSymbol ReturnType => returnType ?? throw new NotInitializedException(nameof(returnType));

            public IEnumerable<IParameterSymbol> Parameters => parameters.Values;

            public bool IsBuiltinOperator => isBuiltinOperator;

            public bool IsOperator => isOperator;

            public bool IsStatic => isStatic;

            public MethodSymbol SetReturnType(ITypeSymbol type)
            {
                returnType = type;
                return this;
            }

            public MethodSymbol SetOperator(bool op)
            {
                isOperator = op;
                return this;
            }

            public MethodSymbol SetBuiltinOperator(bool builtin)
            {
                isBuiltinOperator = builtin;
                return this;
            }

            public MethodSymbol SetStatic(bool isStatic) 
            {
                this.isStatic = isStatic;
                return this;
            }

            public MethodSymbol AddParameter(ParameterSymbol parameter)
            {
                parameters.Add(parameter.Name, parameter);
                return this;
            }

            public ParameterSymbol? GetParameter(string name)
            {
                return parameters.GetValueOrDefault(name);
            }

            public ParameterSymbol GetOrAddParameter(string name)
            {
                if (parameters.TryGetValue(name, out var res))
                    return res;

                var parameter = new ParameterSymbol(name);
                parameters.Add(name, parameter);

                return parameter;
            }

            public override string ToString() => Name;

            public bool Equals(ISymbol? other) => object.ReferenceEquals(this, other);
        }
    }
}