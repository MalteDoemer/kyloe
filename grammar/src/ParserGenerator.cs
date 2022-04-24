using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeGen;

namespace Kyloe.Grammar
{
    public sealed class ParserGenerator
    {
        private readonly FinalGrammar grammar;
        private readonly ParserGeneratorInfo info;

        private readonly List<TokenKind> terminals;
        private readonly List<TokenKind> nonTerminals;

        public ParserGenerator(FinalGrammar grammar, ParserGeneratorInfo info)
        {
            this.grammar = grammar;
            this.info = info;

            terminals = grammar.Terminals.Keys.ToList();
            terminals.Add(TokenKind.Epsilon);
            terminals.Add(TokenKind.Error);
            terminals.Add(TokenKind.End);
            terminals.Sort();

            nonTerminals = grammar.Rules.Keys.ToList();
            nonTerminals.Sort();
        }

        public CompilationUnit CreateCompilationUnit(string namespaceName, AccessModifier accessModifier)
        {
            var ns = new Namespace(namespaceName)
                .Add(CreateTokenKindEnum(accessModifier))
                .Add(CreateExtensionClass(accessModifier))
                .Add(CreateTokenClass(accessModifier))
                .Add(CreateTerminalClass(accessModifier))
                .Add(CreateNodeClass(accessModifier))
                .Add(CreateLexerClass(accessModifier))
                .Add(CreateParserClass(accessModifier));

            return new CompilationUnit()
                .AddUsing("using System;")
                .AddUsing("using System.Linq;")
                .AddUsing("using System.Collections.Generic;")
                .AddUsing("using System.Collections.Immutable;")
                .AddUsing("using System.Text.RegularExpressions;")
                .AddItem(ns);
        }

        public Enum CreateTokenKindEnum(AccessModifier accessModifier)
        {
            return new Enum(accessModifier, info.TokenKindEnumName, "int").AddRange(
                Enumerable.Reverse(terminals)
                .Concat(nonTerminals)
                .Select(t => new EnumMember(grammar.GetName(t), t.Value.ToString()))
            );
        }

        public Class CreateExtensionClass(AccessModifier accessModifier)
        {
            var isTerminalMethod = new Method(
                AccessModifier.Public,
                InheritanceModifier.Static,
                "bool",
                "IsTerminal")
                .AddArg(new Argument($"this {info.TokenKindEnumName}", "kind"))
                .AddLine("return (int)kind < 0;");

            var isNonTerminalMethod = new Method(
                AccessModifier.Public,
                InheritanceModifier.Static,
                "bool",
                "IsNonTerminal")
                .AddArg(new Argument($"this {info.TokenKindEnumName}", "kind"))
                .AddLine("return (int)kind > 0;");

            return new Class(
                accessModifier,
                InheritanceModifier.Static,
                info.ExtensionClassName)
                .Add(isTerminalMethod)
                .Add(isNonTerminalMethod);
        }

        public Class CreateTokenClass(AccessModifier accessModifier)
        {
            var kindProp = new SimpleProperty(
                AccessModifier.Public,
                InheritanceModifier.Abstract,
                info.TokenKindEnumName,
                "Kind",
                "{ get; }");

            var locationProp = new SimpleProperty(
                AccessModifier.Public,
                InheritanceModifier.Abstract,
                info.LocationClassName,
                "Location",
                "{ get; }");

            var childrenMethod = new Method(
                AccessModifier.Public,
                InheritanceModifier.Abstract,
                $"IEnumerable<{info.TokenClassName}>",
                "Children");

            return new Class(
                accessModifier,
                InheritanceModifier.Abstract,
                info.TokenClassName)
                .Add(kindProp)
                .Add(locationProp)
                .Add(childrenMethod);
        }

        public Class CreateTerminalClass(AccessModifier accessModifier)
        {
            var kindProp = new SimpleProperty(
                AccessModifier.Public,
                InheritanceModifier.Override,
                info.TokenKindEnumName,
                "Kind",
                "{ get; }");

            var textProp = new SimpleProperty(
                AccessModifier.Public,
                InheritanceModifier.None,
                "string",
                "Text",
                "{ get; }");

            var locationProp = new SimpleProperty(
                AccessModifier.Public,
                InheritanceModifier.Override,
                info.LocationClassName,
                "Location",
                "{ get; }");

            var childrenMethod = new Method(
                AccessModifier.Public,
                InheritanceModifier.Override,
                $"IEnumerable<{info.TokenClassName}>",
                "Children")
                .AddLine($"return Enumerable.Empty<{info.TokenClassName}>();");

            var ctor = new Method(
                AccessModifier.Public,
                InheritanceModifier.None,
                info.TerminalClassName,
                null)
                .AddArg(new Argument(info.TokenKindEnumName, "kind"))
                .AddArg(new Argument("string", "text"))
                .AddArg(new Argument(info.LocationClassName, "location"))
                .AddLine("Kind = kind;")
                .AddLine("Text = text;")
                .AddLine("Location = location;");

            return new Class(
                accessModifier,
                InheritanceModifier.Sealed,
                info.TerminalClassName,
                info.TokenClassName)
                .Add(ctor)
                .Add(kindProp)
                .Add(textProp)
                .Add(locationProp)
                .Add(childrenMethod);
        }

        public Class CreateNodeClass(AccessModifier accessModifier)
        {
            var immutableArray = $"ImmutableArray<{info.TokenClassName}?>";

            var kindProp = new SimpleProperty(
                AccessModifier.Public,
                InheritanceModifier.Override,
                info.TokenKindEnumName,
                "Kind",
                "{ get; }");

            var tokensProp = new SimpleProperty(
                AccessModifier.Public,
                InheritanceModifier.None,
                immutableArray,
                "Tokens",
                "{ get; }");

            var locationProp = new SimpleProperty(
                AccessModifier.Public,
                InheritanceModifier.Override,
                info.LocationClassName,
                "Location",
                $"=> {info.LocationClassName}.CreateAround(Children().First().Location, Children().Last().Location);");

            var childrenMethod = new Method(
                AccessModifier.Public,
                InheritanceModifier.Override,
                $"IEnumerable<{info.TokenClassName}>",
                "Children")
                .AddLine($"return Tokens.Where(t => t is not null)!;");



            var ctor = new Method(
                AccessModifier.Public,
                InheritanceModifier.None,
                info.NodeClassName,
                null)
                .AddArg(new Argument(info.TokenKindEnumName, "kind"))
                .AddArg(new Argument(immutableArray, "tokens"))
                .AddLine("Kind = kind;")
                .AddLine("Tokens = tokens;");

            return new Class(
                accessModifier,
                InheritanceModifier.Sealed,
                info.NodeClassName,
                info.TokenClassName)
                .Add(ctor)
                .Add(kindProp)
                .Add(tokensProp)
                .Add(locationProp)
                .Add(childrenMethod);
        }

        public Class CreateLexerClass(AccessModifier accessModifier)
        {
            var groups = new List<string>();

            foreach (var terminal in grammar.Terminals.Values)
                groups.Add($"(?<{terminal.Name}>\\G{terminal.Text})");

            var regexString = CreateRawString(string.Join('|', groups));

            var dict = $"Dictionary<string, {info.TokenKindEnumName}>";
            var set = $"HashSet<{info.TokenKindEnumName}>";


            var regexField = new Field(
                AccessModifier.Private,
                InheritanceModifier.None,
                @readonly: true,
                type: "Regex",
                name: "regex");

            var groupNamesField = new Field(
                AccessModifier.Private,
                InheritanceModifier.None,
                @readonly: true,
                type: dict,
                name: "groupNames");

            var discardField = new Field(
                AccessModifier.Private,
                InheritanceModifier.None,
                @readonly: true,
                type: set,
                name: "discardTerminals");

            var textField = new Field(
                AccessModifier.Private,
                InheritanceModifier.None,
                @readonly: true,
                type: "string",
                name: "text");

            var posField = new Field(
                AccessModifier.Private,
                InheritanceModifier.None,
                @readonly: false,
                type: "int",
                name: "pos");

            var ctor = new Method(
                AccessModifier.Public,
                InheritanceModifier.None,
                info.LexerClassName,
                null)
                .AddArg(new Argument("string", "text"))
                .AddLine("this.pos = 0;")
                .AddLine("this.text = text;")
                .AddLine($"this.groupNames = new {dict}();")
                .AddLine($"this.regex = new Regex({regexString}, RegexOptions.Compiled | RegexOptions.Multiline);")
                .AddLine("")
                .AddLine($"var names = Enum.GetNames<{info.TokenKindEnumName}>();")
                .AddLine($"var values = Enum.GetValues<{info.TokenKindEnumName}>();")
                .AddStatement(new ForLoop("int i = 0; i < names.Length; i++")
                    .AddLine("groupNames.Add(names[i], values[i]);"));


            if (grammar.DiscardRule is not null)
            {
                ctor
                .AddLine("")
                .AddLine($"this.discardTerminals = new {set}();");

                foreach (var terminal in grammar.EnumerateTerminals(grammar.DiscardRule.Kind))
                    ctor.AddLine($"this.discardTerminals.Add({info.TokenKindEnumName}.{grammar.GetName(terminal)});");

                var allTerminalsMethod = CreateLexerMethod("AllTerminals");

                var terminalsMethod = new Method(
                    AccessModifier.Public,
                    InheritanceModifier.None,
                    $"IEnumerable<{info.TerminalClassName}>",
                    "Terminals")
                    .AddLine("return AllTerminals().Where(t => !discardTerminals.Contains(t.Kind));");

                return new Class(
                    accessModifier,
                    InheritanceModifier.Sealed,
                    info.LexerClassName)
                    .Add(regexField)
                    .Add(groupNamesField)
                    .Add(discardField)
                    .Add(textField)
                    .Add(posField)
                    .Add(ctor)
                    .Add(allTerminalsMethod)
                    .Add(terminalsMethod);
            }
            else
            {
                return new Class(
                    accessModifier,
                    InheritanceModifier.Sealed,
                    info.LexerClassName)
                    .Add(regexField)
                    .Add(groupNamesField)
                    .Add(textField)
                    .Add(posField)
                    .Add(ctor)
                    .Add(CreateLexerMethod("Terminals"));
            }
        }

        private Method CreateLexerMethod(string name)
        {
            return new Method(
                    AccessModifier.Public,
                    InheritanceModifier.None,
                    $"IEnumerable<{info.TerminalClassName}>",
                    name)
                    .AddStatement(new WhileLoop("pos < text.Length")
                        .AddLine("var match = regex.Match(text, pos);")
                        .AddStatement(new IfStatement("match.Success")
                            .AddLine("var group = match.Groups.OfType<System.Text.RegularExpressions.Group>().Where(g => g.Success).Last();")
                            .AddLine($"var location = {info.LocationClassName}.FromLength(match.Index, match.Length);")
                            .AddLine($"var terminal = new {info.TerminalClassName}(groupNames[group.Name], match.Value, location);")
                            .AddLine("pos += location.Length;")
                            .AddLine("yield return terminal;"))
                        .AddStatement(new ElseStatement()
                            .AddLine($"var location = {info.LocationClassName}.FromLength(pos, 1);")
                            .AddLine($"var terminal = new {info.TerminalClassName}({info.TokenKindEnumName}.Error, text[pos].ToString(), location);")
                            .AddLine("pos += 1;")
                            .AddLine("yield return terminal;")))
                    .AddLine($"yield return new {info.TerminalClassName}({info.TokenKindEnumName}.End, \"<end>\", {info.LocationClassName}.FromLength(pos, 0));");
        }

        public Class CreateParserClass(AccessModifier accessModifier)
        {
            var terminalsField = new Field(
                AccessModifier.Private,
                InheritanceModifier.None,
                @readonly: true,
                type: $"ImmutableArray<{info.TerminalClassName}>",
                name: "terminals");

            var errorsField = new Field(
                AccessModifier.Private,
                InheritanceModifier.None,
                @readonly: true,
                type: $"ICollection<{info.ErrorClassName}>",
                name: "errors");

            var posField = new Field(
                AccessModifier.Private,
                InheritanceModifier.None,
                @readonly: false,
                type: "int",
                name: "pos");

            var currentProp = new SimpleProperty(
                AccessModifier.Private,
                InheritanceModifier.None,
                info.TerminalClassName,
                "current",
                "=> pos < terminals.Length ? terminals[pos] : terminals[terminals.Length - 1];");

            var ctor = new Method(
                AccessModifier.Public,
                InheritanceModifier.None,
                info.ParserClassName,
                null)
                .AddArg(new Argument("string", "text"))
                .AddArg(new Argument($"ICollection<{info.ErrorClassName}>", "errors"))
                .AddLine("this.pos = 0;")
                .AddLine("this.errors = errors;")
                .AddLine($"var lexer = new {info.LexerClassName}(text);")
                .AddLine($"var builder = ImmutableArray.CreateBuilder<{info.TerminalClassName}>();")
                .AddStatement(new ForeachLoop("var terminal in lexer.Terminals()")
                    .AddStatement(new IfStatement($"terminal.Kind == {info.TokenKindEnumName}.Error")
                        .AddLine($"errors.Add(new {info.ErrorClassName}({info.ErrorKindEnumName}.InvalidCharacterError, string.Format(\"invalid character: \\\\u{{0:x4}}\", (int)(terminal.Text[0])), terminal.Location));"))
                    .AddStatement(new ElseStatement()
                        .AddLine("builder.Add(terminal);")))
                .AddLine("this.terminals = builder.ToImmutable();");


            var advanceMethod = new Method(
                AccessModifier.Private,
                InheritanceModifier.None,
                info.TerminalClassName,
                "Advance")
                .AddLine("var temp = current;")
                .AddLine("pos += 1;")
                .AddLine("return temp;");

            var expectMethod = new Method(
                AccessModifier.Private,
                InheritanceModifier.None,
                $"{info.TokenClassName}?",
                "Expect")
                .AddArg(new Argument(info.TokenKindEnumName, "kind"))
                .AddStatement(new IfStatement("current.Kind == kind")
                    .AddLine("return Advance();"))
                .AddLine("throw new NotImplementedException();");

            var createNodeMethod = new Method(
                AccessModifier.Private,
                InheritanceModifier.None,
                $"{info.TokenClassName}?",
                "CreateNode")
                .AddArg(new Argument(info.TokenKindEnumName, "kind"))
                .AddArg(new Argument($"params {info.TokenClassName}?[]", "tokens"))
                .AddLine("var arr = tokens.ToImmutableArray();")
                .AddLine("if (arr.Length == 0) return null;")
                .AddLine("else if (arr.Length == 1) return arr[0];")
                .AddLine($"else return new {info.NodeClassName}(kind, arr);");

            if (grammar.StartRule is null)
                throw new GrammarException("cannot create Parser class without a Start rule");

            var parseMethod = new Method(
                AccessModifier.Public,
                InheritanceModifier.None,
                $"{info.TokenClassName}?",
                "Parse")
                .AddLine($"var token = {GetParseMethodName(grammar.StartRule.Kind)}();")
                .AddLine($"Expect({info.TokenKindEnumName}.End);")
                .AddLine("return token;");

            IEnumerable<TokenKind> rules;

            if (grammar.DiscardRule is null)
                rules = nonTerminals;
            else
                rules = nonTerminals.Where(t => t != grammar.DiscardRule.Kind);

            return new Class(
                accessModifier,
                InheritanceModifier.Sealed,
                info.ParserClassName)
                .Add(terminalsField)
                .Add(errorsField)
                .Add(posField)
                .Add(currentProp)
                .Add(ctor)
                .Add(advanceMethod)
                .Add(expectMethod)
                .Add(createNodeMethod)
                .Add(parseMethod)
                .AddRange(rules.Select(t => CreateParseMethod(t)));
        }

        private Method CreateParseMethod(TokenKind nonTerminal)
        {

            var name = GetParseMethodName(nonTerminal);

            var method = new Method(
                AccessModifier.Private,
                InheritanceModifier.None,
                $"{info.TokenClassName}?",
                name);

            var rule = grammar.Rules[nonTerminal];

            if (rule.IsLeftRecursive)
                GenerateLeftRecursiveParseMethodBody(method, rule);
            else
                GenerateParseMethodBody(method, rule);

            return method;
        }

        private void GenerateParseMethodBody(Method method, ProductionRule rule)
        {
            var sw = new SwitchStatement("current.Kind");

            foreach (var prod in rule.Productions)
            {
                var first = grammar.FirstSet(rule.Kind, prod);

                bool hasAny = false;

                foreach (var token in first)
                {
                    if (token == TokenKind.Epsilon)
                        continue;

                    hasAny = true;
                    sw.AddCase($"{info.TokenKindEnumName}.{grammar.GetName(token)}");
                }

                if (hasAny)
                {
                    var block = new BlockStatement();
                    GenerateProductionParsingBlock("return", "n", block, rule, prod);
                    sw.AddStatement(block);
                }

            }
            if (grammar.FirstSet(rule.Kind).Contains(TokenKind.Epsilon))
                sw.AddLine("default: return null;");
            else
                sw.AddLine("default: throw new NotImplementedException();");

            method.AddStatement(sw);
        }

        private void GenerateLeftRecursiveParseMethodBody(Method method, ProductionRule rule)
        {
            var outerSwitch = new SwitchStatement("current.Kind");

            for (int i = rule.FirstNonLeftRecursiveProduction; i < rule.Productions.Length; i++)
            {
                var prod = rule.Productions[i];
                var first = grammar.FirstSet(rule.Kind, prod);

                bool hasAny = false;

                foreach (var token in first)
                {
                    if (token == TokenKind.Epsilon)
                        continue;

                    hasAny = true;
                    outerSwitch.AddCase($"{info.TokenKindEnumName}.{grammar.GetName(token)}");
                }

                if (hasAny)
                {
                    var block = new BlockStatement();
                    GenerateLeftRecursiveLoop(block, rule, prod);
                    outerSwitch.AddStatement(block);
                }
            }

            if (grammar.FirstSet(rule.Kind).Contains(TokenKind.Epsilon))
            {
                outerSwitch.AddLine("default:");
                var block = new BlockStatement();
                GenerateLeftRecursiveLoop(block, rule, new EmptyProduction());
                outerSwitch.AddStatement(block);
            }
            else
            {
                outerSwitch.AddLine("default: throw new NotImplementedException();");
            }

            method.AddStatement(outerSwitch);
        }

        private void GenerateLeftRecursiveLoop(BlockStatement block, ProductionRule rule, Production production)
        {
            GenerateProductionParsingBlock($"{info.TokenClassName}? node =", "n", block, rule, production);

            var condition = new StringBuilder();

            for (int i = 0; i < rule.FirstNonLeftRecursiveProduction; i++)
            {
                var prod = GetProductionWithoutLeftRecursiveTerm(rule.Productions[i]);

                IEnumerable<TokenKind> first;

                if (grammar.FirstSet(rule.Kind).Contains(TokenKind.Epsilon))
                    first = grammar.FirstSet(rule.Kind).Union(grammar.FollowSet(rule.Kind));
                else
                    first = grammar.FirstSet(rule.Kind, prod);

                foreach (var token in first)
                {
                    if (token == TokenKind.Epsilon)
                        continue;

                    condition.Append($"current.Kind == {info.TokenKindEnumName}.{grammar.GetName(token)} || ");
                }
            }

            // remove the last || from the condition
            if (condition.Length != 0)
                condition.Remove(condition.Length - 4, 4);

            var whileLoop = new WhileLoop(condition.ToString());

            var innerSwitch = new SwitchStatement("current.Kind");

            for (int i = 0; i < rule.FirstNonLeftRecursiveProduction; i++)
            {
                var prod = GetProductionWithoutLeftRecursiveTerm(rule.Productions[i]);

                var first = grammar.FirstSet(rule.Kind, prod);

                bool hasAny = false;

                foreach (var token in first)
                {
                    if (token == TokenKind.Epsilon)
                        continue;

                    hasAny = true;
                    innerSwitch.AddCase($"{info.TokenKindEnumName}.{grammar.GetName(token)}");
                }

                if (hasAny)
                {
                    var innerBlock = new BlockStatement();
                    GenerateProductionParsingBlock($"{info.TokenClassName}? temp =", "x", innerBlock, rule, prod);
                    innerBlock.AddLine($"node = CreateNode({info.TokenKindEnumName}.{grammar.GetName(rule.Kind)}, node, temp);");
                    innerBlock.AddLine("break;");
                    innerSwitch.AddStatement(innerBlock);
                }
            }

            if (grammar.FirstSet(rule.Kind).Contains(TokenKind.Epsilon))
            {
                for (int i = rule.FirstNonLeftRecursiveProduction; i < rule.Productions.Length; i++)
                {
                    var prod = rule.Productions[i];
                    var first = grammar.FirstSet(rule.Kind, prod);

                    bool hasAny = false;

                    foreach (var token in first)
                    {
                        if (token == TokenKind.Epsilon)
                            continue;

                        innerSwitch.AddCase($"{info.TokenKindEnumName}.{grammar.GetName(token)}");
                        hasAny = true;


                    }

                    if (hasAny)
                    {
                        var innerBlock = new BlockStatement();
                        GenerateProductionParsingBlock($"{info.TokenClassName}? temp =", "x", block, rule, prod);
                        innerBlock.AddLine($"node = CreateNode({info.TokenKindEnumName}.{grammar.GetName(rule.Kind)}, node, x);");
                        innerBlock.AddLine("break;");

                        innerSwitch.AddStatement(innerBlock);
                    }
                }

                innerSwitch.AddLine("default: return node;");
            }

            whileLoop.AddStatement(innerSwitch);
            block.Add(whileLoop);
            block.AddLine("return node;");
        }

        private void GenerateProductionParsingBlock(string result, string varname, BlockStatement statement, ProductionRule rule, Production production)
        {
            if (production is EmptyProduction)
            {
                statement.AddLine($"{result} null;");
                return;
            }

            var n = 0;
            foreach (var child in production.Children())
            {
                if (child.IsTerminal)
                    statement.AddLine($"var {varname}{n} = Expect({info.TokenKindEnumName}.{grammar.GetName(child)});");
                else
                    statement.AddLine($"var {varname}{n} = {GetParseMethodName(child)}();");

                n++;
            }

            var code = new StringBuilder();

            code.Append(result);
            code.Append(" CreateNode(");
            code.Append(info.TokenKindEnumName);
            code.Append('.');
            code.Append(grammar.GetName(rule.Kind));

            for (int i = 0; i < n; i++)
                code.Append(", ").Append(varname).Append(i);

            code.Append(");");
            statement.AddLine(code.ToString());
        }

        private Production GetProductionWithoutLeftRecursiveTerm(Production production)
        {
            if (production is ConcatProduction concat)
                return DeleteLeftMostNode(concat);
            return new EmptyProduction();
        }

        private Production DeleteLeftMostNode(ConcatProduction production)
        {
            switch (production.Left)
            {
                case NameProduction:
                case EmptyProduction:
                    return production.Right;
                case ConcatProduction left:
                    var newleft = DeleteLeftMostNode(left);
                    return new ConcatProduction(newleft, production.Right);
                default:
                    throw new System.Exception($"unexpected production type: '{production.Left.GetType()}'");
            }
        }

        private string GetParseMethodName(TokenKind nonTerminal)
        {
            return $"Parse{grammar.GetName(nonTerminal)}";
        }

        private string CreateRawString(string str)
        {
            var builder = new StringBuilder(str);
            builder.Replace("\"", "\"\"");
            builder.Insert(0, "@\"");
            builder.Append("\"");
            return builder.ToString();
        }
    }
}