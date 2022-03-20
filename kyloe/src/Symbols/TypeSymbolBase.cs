using System.Linq;
using System.Collections.Generic;

namespace Kyloe.Symbols
{
    public partial class TypeSystem
    {
        private abstract class TypeSymbolBase
        {
            protected AccessModifiers accessModifiers;

            protected readonly Dictionary<string, ClassTypeSymbol> nestedClasses;
            protected readonly Dictionary<string, List<OperationSymbol>> operations;
            protected readonly Dictionary<string, List<MethodSymbol>> methods;
            protected readonly Dictionary<string, PropertySymbol> properties;
            protected readonly Dictionary<string, FieldSymbol> fields;

            public TypeSymbolBase(string name)
            {
                Name = name;
                nestedClasses = new Dictionary<string, ClassTypeSymbol>();
                operations = new Dictionary<string, List<OperationSymbol>>();
                methods = new Dictionary<string, List<MethodSymbol>>();
                properties = new Dictionary<string, PropertySymbol>();
                fields = new Dictionary<string, FieldSymbol>();
            }

            public string Name { get; }

            public AccessModifiers AccessModifiers => accessModifiers;

            public IEnumerable<IClassTypeSymbol> NestedClasses => nestedClasses.Values;

            public IEnumerable<IOperationSymbol> Operations => operations.Values.SelectMany(list => list);

            public IEnumerable<IMethodSymbol> Methods => methods.Values.SelectMany(list => list);

            public IEnumerable<IPropertySymbol> Properties => properties.Values;

            public IEnumerable<IFieldSymbol> Fields => fields.Values;

            public IEnumerable<IMemberSymbol> Members => NestedClasses.Cast<IMemberSymbol>().Concat(Operations).Concat(Methods).Concat(Properties).Concat(Fields);

            public bool Equals(ISymbol? other) => object.ReferenceEquals(this, other);

            public IEnumerable<IMemberSymbol> LookupMembers(string name)
            {
                if (nestedClasses.TryGetValue(name, out var nested))
                    yield return nested;

                if (operations.TryGetValue(name, out var opList))
                    foreach (var op in opList)
                        yield return op;

                if (methods.TryGetValue(name, out var methodList))
                    foreach (var method in methodList)
                        yield return method;

                if (properties.TryGetValue(name, out var property))
                    yield return property;

                if (fields.TryGetValue(name, out var field))
                    yield return field;
            }

            public ClassTypeSymbol GetOrAddNestedClass(string name)
            {
                if (nestedClasses.TryGetValue(name, out var nested))
                    return nested;

                var nestedClass = new ClassTypeSymbol(name);
                nestedClasses.Add(name, nestedClass);
                return nestedClass;
            }

            public TypeSymbolBase AddNestedClass(ClassTypeSymbol nestedClass)
            {
                System.Console.WriteLine(nestedClasses.Count);
                nestedClasses.Add(nestedClass.Name, nestedClass);
                return this;
            }

            public TypeSymbolBase AddMethod(MethodSymbol method)
            {
                if (methods.TryGetValue(method.Name, out var methodList))
                {
                    methodList.Add(method);
                }
                else
                {
                    var list = new List<MethodSymbol>();
                    list.Add(method);
                    methods.Add(method.Name, list);
                }

                return this;
            }

            public TypeSymbolBase AddOperation(OperationSymbol operation)
            {
                if (operations.TryGetValue(operation.Name, out var operationList))
                {
                    operationList.Add(operation);
                }
                else
                {
                    var list = new List<OperationSymbol>();
                    list.Add(operation);
                    operations.Add(operation.Name, list);
                }

                return this;
            }

            public TypeSymbolBase AddProperty(PropertySymbol property)
            {
                properties.Add(property.Name, property);
                return this;
            }

            public TypeSymbolBase AddField(FieldSymbol field)
            {
                fields.Add(field.Name, field);
                return this;
            }

            public TypeSymbolBase SetAccessModifiers(AccessModifiers modifiers)
            {
                this.accessModifiers = modifiers;
                return this;
            }
        }
    }
}