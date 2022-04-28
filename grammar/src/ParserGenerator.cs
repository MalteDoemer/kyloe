using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeGen;

namespace Kyloe.Grammar
{
    public sealed class ParserGenerator
    {
        private readonly FinalGrammar grammar;
        private readonly GeneratorInfo info;

        private readonly List<TokenKind> terminals;
        private readonly List<TokenKind> nonTerminals;

        public ParserGenerator(FinalGrammar grammar, GeneratorInfo info)
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

        public IEnumerable<(string, CompilationUnit)> CreateMultipleCompilationUnits()
        {
            yield return (info.TokenKindEnum.Name, new CompilationUnit()
                .AddItem(new Namespace(info.Namespace).Add(CreateTokenKindEnum())));

            yield return (info.ExtensionClass.Name, new CompilationUnit()
                .AddItem(new Namespace(info.Namespace).Add(CreateExtensionClass())));

            yield return (info.TokenClass.Name, new CompilationUnit()
                .AddUsing("using System.Collections.Generic;")
                .AddItem(new Namespace(info.Namespace).Add(CreateTokenClass())));

            yield return (info.TerminalClass.Name, new CompilationUnit()
                .AddUsing("using System.Linq;")
                .AddUsing("using System.Collections.Generic;")
                .AddItem(new Namespace(info.Namespace).Add(CreateTerminalClass())));

            yield return (info.NodeClass.Name, new CompilationUnit()
                .AddUsing("using System.Linq;")
                .AddUsing("using System.Collections.Immutable;")
                .AddUsing("using System.Collections.Generic;")
                .AddItem(new Namespace(info.Namespace).Add(CreateNodeClass())));

            yield return (info.LexerClass.Name, new CompilationUnit()
                .AddUsing("using System;")
                .AddUsing("using System.Linq;")
                .AddUsing("using System.Collections.Generic;")
                .AddUsing("using System.Text.RegularExpressions;")
                .AddItem(new Namespace(info.Namespace).Add(CreateLexerClass())));

            yield return (info.ParserClass.Name, new CompilationUnit()
                .AddUsing("using System.Linq;")
                .AddUsing("using System.Collections.Immutable;")
                .AddUsing("using System.Collections.Generic;")
                .AddItem(new Namespace(info.Namespace).Add(CreateParserClass())));
        }

        public CompilationUnit CreateSingleCompilationUnit()
        {
            var ns = new Namespace(info.Namespace)
                .Add(CreateTokenKindEnum())
                .Add(CreateExtensionClass())
                .Add(CreateTokenClass())
                .Add(CreateTerminalClass())
                .Add(CreateNodeClass())
                .Add(CreateLexerClass())
                .Add(CreateParserClass());

            return new CompilationUnit()
                .AddUsing("using System;")
                .AddUsing("using System.Linq;")
                .AddUsing("using System.Collections.Generic;")
                .AddUsing("using System.Collections.Immutable;")
                .AddUsing("using System.Text.RegularExpressions;")
                .AddItem(ns);
        }

        public Enum CreateTokenKindEnum()
        {
            return new Enum(info.TokenKindEnum.AccessModifiers, info.TokenKindEnum.Name, "int").AddRange(
                Enumerable.Reverse(terminals)
                .Concat(nonTerminals)
                .Select(t => new EnumMember(grammar.GetName(t), t.Value.ToString()))
            );
        }

        public Class CreateExtensionClass()
        {
            var isTerminalMethod = new Method(
                AccessModifier.Public,
                InheritanceModifier.Static,
                "bool",
                "IsTerminal")
                .AddArg($"this {info.TokenKindEnum.Name} kind")
                .AddLine("return (int)kind < 0;");

            var isNonTerminalMethod = new Method(
                AccessModifier.Public,
                InheritanceModifier.Static,
                "bool",
                "IsNonTerminal")
                .AddArg($"this {info.TokenKindEnum.Name} kind")
                .AddLine("return (int)kind > 0;");

            return new Class(
                info.ExtensionClass.AccessModifiers,
                InheritanceModifier.Static,
                info.ExtensionClass.Name)
                .Add(isTerminalMethod)
                .Add(isNonTerminalMethod);
        }

        public Class CreateTokenClass()
        {
            var kindProp = new SimpleProperty(
                AccessModifier.Public,
                InheritanceModifier.Abstract,
                info.TokenKindEnum.Name,
                "Kind",
                "{ get; }");

            var locationProp = new SimpleProperty(
                AccessModifier.Public,
                InheritanceModifier.Abstract,
                info.LocationClass.Name,
                "Location",
                "{ get; }");

            var childrenMethod = new Method(
                AccessModifier.Public,
                InheritanceModifier.Abstract,
                $"IEnumerable<{info.TokenClass.Name}>",
                "Children");

            return new Class(
                info.TokenClass.AccessModifiers,
                InheritanceModifier.Abstract,
                info.TokenClass.Name)
                .Add(kindProp)
                .Add(locationProp)
                .Add(childrenMethod);
        }

        public Class CreateTerminalClass()
        {
            var kindProp = new SimpleProperty(
                AccessModifier.Public,
                InheritanceModifier.Override,
                info.TokenKindEnum.Name,
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
                info.LocationClass.Name,
                "Location",
                "{ get; }");

            var invalidProp = new SimpleProperty(
                AccessModifier.Public,
                InheritanceModifier.None,
                "bool",
                "Invalid",
                "{ get; }"
            );

            var childrenMethod = new Method(
                AccessModifier.Public,
                InheritanceModifier.Override,
                $"IEnumerable<{info.TokenClass.Name}>",
                "Children")
                .AddLine($"return Enumerable.Empty<{info.TokenClass.Name}>();");

            var ctor = new Method(
                AccessModifier.Public,
                InheritanceModifier.None,
                info.TerminalClass.Name,
                null)
                .AddArg($"{info.TokenKindEnum.Name} kind")
                .AddArg("string text")
                .AddArg($"{info.LocationClass.Name} location")
                .AddArg("bool invalid = false")
                .AddLine("Kind = kind;")
                .AddLine("Text = text;")
                .AddLine("Location = location;")
                .AddLine("Invalid = invalid;")
                ;

            return new Class(
                info.TerminalClass.AccessModifiers,
                InheritanceModifier.Sealed,
                info.TerminalClass.Name,
                info.TokenClass.Name)
                .Add(ctor)
                .Add(kindProp)
                .Add(textProp)
                .Add(locationProp)
                .Add(invalidProp)
                .Add(childrenMethod);
        }

        public Class CreateNodeClass()
        {
            var immutableArray = $"ImmutableArray<{info.TokenClass.Name}>";

            var nonEmptyChildrenProp = new SimpleProperty(
                AccessModifier.Private,
                InheritanceModifier.None,
                $"IEnumerable<{info.TokenClass.Name}>",
                "nonEmptyChildren",
                $"=> Tokens.Where(t => t.Kind != {TokenKindAccessString(TokenKind.Epsilon)});"
            );

            var kindProp = new SimpleProperty(
                AccessModifier.Public,
                InheritanceModifier.Override,
                info.TokenKindEnum.Name,
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
                info.LocationClass.Name,
                "Location",
                $"=> {info.LocationClass.Name}.CreateAround(nonEmptyChildren.First().Location, nonEmptyChildren.Last().Location);");

            var childrenMethod = new Method(
                AccessModifier.Public,
                InheritanceModifier.Override,
                $"IEnumerable<{info.TokenClass.Name}>",
                "Children")
                .AddLine($"return Tokens;");

            var ctor = new Method(
                AccessModifier.Public,
                InheritanceModifier.None,
                info.NodeClass.Name,
                null)
                .AddArg($"{info.TokenKindEnum.Name} kind")
                .AddArg($"{immutableArray} tokens")
                .AddLine("Kind = kind;")
                .AddLine("Tokens = tokens;");

            return new Class(
                info.NodeClass.AccessModifiers,
                InheritanceModifier.Sealed,
                info.NodeClass.Name,
                info.TokenClass.Name)
                .Add(nonEmptyChildrenProp)
                .Add(ctor)
                .Add(kindProp)
                .Add(tokensProp)
                .Add(locationProp)
                .Add(childrenMethod);
        }

        public Class CreateLexerClass()
        {
            var groups = new List<string>();

            foreach (var terminal in Enumerable.Reverse(terminals))
            {
                if (grammar.Terminals.TryGetValue(terminal, out var t))
                    groups.Add($"(?<{t.Name}>\\G{t.Text})");
            }

            var regexString = CreateRawString(string.Join('|', groups));

            var dict = $"Dictionary<string, {info.TokenKindEnum.Name}>";
            var set = $"HashSet<{info.TokenKindEnum.Name}>";


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
                info.LexerClass.Name,
                null)
                .AddArg("string text")
                .AddLine("this.pos = 0;")
                .AddLine("this.text = text;")
                .AddLine($"this.groupNames = new {dict}();")
                .AddLine($"this.regex = new Regex({regexString}, RegexOptions.Compiled | RegexOptions.Multiline);")
                .AddLine("")
                .AddLine($"var names = Enum.GetNames<{info.TokenKindEnum.Name}>();")
                .AddLine($"var values = Enum.GetValues<{info.TokenKindEnum.Name}>();")
                .AddStatement(new ForLoop("int i = 0; i < names.Length; i++")
                    .AddLine("groupNames.Add(names[i], values[i]);"));


            if (grammar.DiscardRule is not null)
            {
                ctor
                .AddLine("")
                .AddLine($"this.discardTerminals = new {set}();");

                foreach (var terminal in grammar.EnumerateTerminals(grammar.DiscardRule.Kind))
                    ctor.AddLine($"this.discardTerminals.Add({TokenKindAccessString(terminal)});");

                var allTerminalsMethod = CreateLexerMethod("AllTerminals");

                var terminalsMethod = new Method(
                    AccessModifier.Public,
                    InheritanceModifier.None,
                    $"IEnumerable<{info.TerminalClass.Name}>",
                    "Terminals")
                    .AddLine("return AllTerminals().Where(t => !discardTerminals.Contains(t.Kind));");

                return new Class(
                    info.LexerClass.AccessModifiers,
                    InheritanceModifier.Sealed,
                    info.LexerClass.Name)
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
                    info.LexerClass.AccessModifiers,
                    InheritanceModifier.Sealed,
                    info.LexerClass.Name)
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
                    $"IEnumerable<{info.TerminalClass.Name}>",
                    name)
                    .AddStatement(new WhileLoop("pos < text.Length")
                        .AddLine("var match = regex.Match(text, pos);")
                        .AddStatement(new IfStatement("match.Success")
                            .AddLine("var group = match.Groups.OfType<System.Text.RegularExpressions.Group>().Where(g => g.Success).Last();")
                            .AddLine($"var location = {info.LocationClass.Name}.FromLength(match.Index, match.Length);")
                            .AddLine($"var terminal = new {info.TerminalClass.Name}(groupNames[group.Name], match.Value, location);")
                            .AddLine("pos += location.Length;")
                            .AddLine("yield return terminal;"))
                        .AddStatement(new ElseStatement()
                            .AddLine($"var location = {info.LocationClass.Name}.FromLength(pos, 1);")
                            .AddLine($"var terminal = new {info.TerminalClass.Name}({info.TokenKindEnum.Name}.Error, text[pos].ToString(), location);")
                            .AddLine("pos += 1;")
                            .AddLine("yield return terminal;")))
                    .AddLine($"yield return new {info.TerminalClass.Name}({info.TokenKindEnum.Name}.End, \"<end>\", {info.LocationClass.Name}.FromLength(pos, 0));");
        }

        public Class CreateParserClass()
        {

            if (grammar.StartRule is null)
                throw new GrammarException("cannot create Parser class without a Start rule");

            if (grammar.StopRule is null)
                throw new GrammarException("cannot create Parser class without a Stop rule");

            var stopTerminals = grammar.EnumerateTerminals(grammar.StopRule.Kind);

            var terminalsField = new Field(
                AccessModifier.Private,
                InheritanceModifier.None,
                @readonly: true,
                type: $"ImmutableArray<{info.TerminalClass.Name}>",
                name: "terminals");

            var stopTerminalsField = new Field(
                AccessModifier.Private,
                InheritanceModifier.None,
                true,
                $"HashSet<{info.TokenKindEnum.Name}>",
                "stopTerminals"
            );

            var errorsField = new Field(
                AccessModifier.Private,
                InheritanceModifier.None,
                @readonly: true,
                type: $"ICollection<{info.ErrorClass.Name}>",
                name: "errors");

            var posField = new Field(
                AccessModifier.Private,
                InheritanceModifier.None,
                @readonly: false,
                type: "int",
                name: "pos");

            var isValidField = new Field(
                AccessModifier.Private,
                InheritanceModifier.None,
                @readonly: false,
                type: "bool",
                name: "isValid");

            var currentProp = new SimpleProperty(
                AccessModifier.Private,
                InheritanceModifier.None,
                info.TerminalClass.Name,
                "current",
                "=> pos < terminals.Length ? terminals[pos] : terminals[terminals.Length - 1];");

            var ctor = new Method(
                AccessModifier.Public,
                InheritanceModifier.None,
                info.ParserClass.Name,
                null)
                .AddArg("string text")
                .AddArg($"ICollection<{info.ErrorClass.Name}> errors")
                .AddLine("this.pos = 0;")
                .AddLine("this.isValid = true;")
                .AddLine("this.errors = errors;")
                .AddLine($"var lexer = new {info.LexerClass.Name}(text);")
                .AddLine($"var builder = ImmutableArray.CreateBuilder<{info.TerminalClass.Name}>();")
                .AddStatement(new ForeachLoop("var terminal in lexer.Terminals()")
                    .AddStatement(new IfStatement($"terminal.Kind == {info.TokenKindEnum.Name}.Error")
                        .AddLine($"errors.Add(new {info.ErrorClass.Name}({info.ErrorKindEnum.Name}.InvalidCharacterError, string.Format(\"invalid character: \\\\u{{0:x4}}\", (int)(terminal.Text[0])), terminal.Location));"))
                    .AddStatement(new ElseStatement()
                        .AddLine("builder.Add(terminal);")))
                .AddLine("this.terminals = builder.ToImmutable();")
                .AddLine($"this.stopTerminals = new HashSet<{info.TokenKindEnum.Name}>();")
                .AddLines(stopTerminals.Select(t => $"this.stopTerminals.Add({TokenKindAccessString(t)});"))
                .AddLine($"this.stopTerminals.Add({TokenKindAccessString(TokenKind.End)});");

            var advanceMethod = new Method(
                AccessModifier.Private,
                InheritanceModifier.None,
                info.TerminalClass.Name,
                "Advance")
                .AddLine("var temp = current;")
                .AddLine("pos += 1;")
                .AddLine("isValid = true;")
                .AddLine("return temp;");

            var expectMethod = new Method(
                AccessModifier.Private,
                InheritanceModifier.None,
                info.TerminalClass.Name,
                "Expect")
                .AddArg($"{info.TokenKindEnum.Name} expected")
                .AddArg($"params {info.TokenKindEnum.Name}[] next")
                .AddLine("if (current.Kind == expected) return Advance();")
                .AddLine("Unexpected(expected);")
                .AddLine($"var forged = new {info.TerminalClass.Name}(expected, current.Text, current.Location, invalid: true);")
                .AddLine($"if (next.Length != 0) SkipInput(next);")
                .AddLine("return forged;");

            var skipInputMethod = new Method(
                AccessModifier.Private,
                InheritanceModifier.None,
                "void",
                "SkipInput")
                .AddArg($"params {info.TokenKindEnum.Name}[] next")
                .AddLine("var nextSet = next.ToHashSet();")
                .AddStatement(new WhileLoop("!next.Contains(current.Kind) && ! stopTerminals.Contains(current.Kind)")
                    .AddLine("pos += 1;"));

            var unexpectedMethod = new Method(
                AccessModifier.Private,
                InheritanceModifier.None,
                "void",
                "Unexpected")
                .AddArg($"params {info.TokenKindEnum.Name}[] expected")
                .AddStatement(new IfStatement("!isValid")
                    .AddLine("return;"))
                .AddLine("isValid = false;")
                .AddLine($"string msg;")
                .AddStatement(new IfStatement("expected.Length == 1")
                    .AddLine("msg = $\"expected {expected[0]}, got {current.Kind}\";"))
                .AddStatement(new ElseStatement()
                    .AddLine("msg = $\"expected one of ({string.Join(\", \", expected)}), got {current.Kind}\";"))
                .AddLine($"errors.Add(new {info.ErrorClass.Name}({info.ErrorKindEnum.Name}.UnexpectedTokenError, msg, current.Location));");

            var createNodeMethod = new Method(
                AccessModifier.Private,
                InheritanceModifier.None,
                info.TokenClass.Name,
                "CreateNode")
                .AddArg($"{info.TokenKindEnum.Name} kind")
                .AddArg($"params {info.TokenClass.Name}[] tokens")
                .AddLine("var arr = tokens.ToImmutableArray();")
                .AddLine($"if (arr.Length == 0) return new {info.NodeClass.Name}({TokenKindAccessString(TokenKind.Epsilon)}, ImmutableArray<{info.TokenClass.Name}>.Empty);")
                .AddLine("else if (arr.Length == 1) return arr[0];")
                .AddLine($"else return new {info.NodeClass.Name}(kind, arr);");

            var parseMethod = new Method(
                AccessModifier.Public,
                InheritanceModifier.None,
                info.TokenClass.Name,
                "Parse")
                .AddLine($"var token = {ParseMethodName(grammar.StartRule.Kind)}();")
                .AddLine($"Expect({info.TokenKindEnum.Name}.End);")
                .AddLine("return token;");

            IEnumerable<TokenKind> rules;

            if (grammar.DiscardRule is null)
                rules = nonTerminals;
            else
                rules = nonTerminals.Where(t => t != grammar.DiscardRule.Kind);

            return new Class(
                info.ParserClass.AccessModifiers,
                InheritanceModifier.Sealed,
                info.ParserClass.Name)
                .Add(terminalsField)
                .Add(stopTerminalsField)
                .Add(errorsField)
                .Add(posField)
                .Add(isValidField)
                .Add(currentProp)
                .Add(ctor)
                .Add(advanceMethod)
                .Add(expectMethod)
                .Add(skipInputMethod)
                .Add(unexpectedMethod)
                .Add(createNodeMethod)
                .Add(parseMethod)
                .AddRange(rules.Select(t => CreateParseMethod(t)));
        }

        private Method CreateParseMethod(TokenKind nonTerminal)
        {

            var name = ParseMethodName(nonTerminal);

            var method = new Method(
                AccessModifier.Private,
                InheritanceModifier.None,
                info.TokenClass.Name,
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

            var expected = new List<TokenKind>();

            foreach (var prod in rule.Productions)
            {
                var first = grammar.FirstSet(rule.Kind, prod);

                bool hasAny = false;

                foreach (var token in first)
                {
                    if (token == TokenKind.Epsilon)
                        continue;

                    hasAny = true;
                    sw.AddCase(TokenKindAccessString(token));
                    expected.Add(token);
                }

                if (hasAny)
                {
                    var block = new BlockStatement();
                    GenerateProductionParsingBlock("return", "n", block, rule, prod);
                    sw.AddStatement(block);
                }

            }
            if (grammar.FirstSet(rule.Kind).Contains(TokenKind.Epsilon))
                sw.AddLine($"default: return new {info.NodeClass.Name}({TokenKindAccessString(TokenKind.Epsilon)}, ImmutableArray<{info.TokenClass.Name}>.Empty);");
            else
            {
                sw.AddLine("default:");
                var defaultBlock = new BlockStatement();

                var args = string.Join(", ", expected.Select(t => TokenKindAccessString(t)));

                defaultBlock.AddLine($"Unexpected({args});");
                defaultBlock.AddLine($"return new {info.NodeClass.Name}({TokenKindAccessString(TokenKind.Error)}, ImmutableArray.Create<{info.TokenClass.Name}>(current));");

                sw.AddStatement(defaultBlock);
            }

            method.AddStatement(sw);
        }

        private void GenerateLeftRecursiveParseMethodBody(Method method, ProductionRule rule)
        {
            var outerSwitch = new SwitchStatement("current.Kind");

            var expected = new List<TokenKind>();

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
                    outerSwitch.AddCase(TokenKindAccessString(token));
                    expected.Add(token);
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
                outerSwitch.AddLine("default:");
                var defaultBlock = new BlockStatement();

                var args = string.Join(", ", expected.Select(t => TokenKindAccessString(t)));

                defaultBlock.AddLine($"Unexpected({args});");
                defaultBlock.AddLine($"return new {info.NodeClass.Name}({TokenKindAccessString(TokenKind.Error)}, ImmutableArray.Create<{info.TokenClass.Name}>(current));");

                outerSwitch.AddStatement(defaultBlock);
            }

            method.AddStatement(outerSwitch);
        }

        private void GenerateLeftRecursiveLoop(BlockStatement block, ProductionRule rule, Production production)
        {
            GenerateProductionParsingBlock($"{info.TokenClass.Name} node =", "n", block, rule, production);

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

                    condition.Append($"current.Kind == {TokenKindAccessString(token)} || ");
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
                    innerSwitch.AddCase(TokenKindAccessString(token));
                }

                if (hasAny)
                {
                    var innerBlock = new BlockStatement();
                    GenerateProductionParsingBlock($"var temp =", "x", innerBlock, rule, prod);
                    innerBlock.AddLine($"node = CreateNode({TokenKindAccessString(rule.Kind)}, node, temp);");
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

                        innerSwitch.AddCase(TokenKindAccessString(token));
                        hasAny = true;


                    }

                    if (hasAny)
                    {
                        var innerBlock = new BlockStatement();
                        GenerateProductionParsingBlock($"var temp =", "x", innerBlock, rule, prod);
                        innerBlock.AddLine($"node = CreateNode({TokenKindAccessString(rule.Kind)}, node, temp);");
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
                statement.AddLine($"{result} new {info.NodeClass.Name}({TokenKindAccessString(TokenKind.Epsilon)}, ImmutableArray<{info.TokenClass.Name}>.Empty);");
                return;
            }

            var n = 0;
            var children = production.Children().ToArray();

            for (; n < children.Length; n++)
            {
                var child = children[n];

                if (!child.IsTerminal)
                    statement.AddLine($"var {varname}{n} = {ParseMethodName(child)}();");
                else if (n == 0)
                    statement.AddLine($"var {varname}{n} = Advance();");
                else if (n == children.Length - 1)
                    statement.AddLine($"var {varname}{n} = Expect({TokenKindAccessString(child)});");
                else
                {
                    var next = children[n + 1];
                    var firstNext = grammar.FirstSet(next);
                    var args = string.Join(", ", firstNext.Select(t => TokenKindAccessString(t)));
                    statement.AddLine($"var {varname}{n} = Expect({TokenKindAccessString(child)}, {args});");
                }
            }

            var code = new StringBuilder();

            code.Append(result);
            code.Append(" CreateNode(");
            code.Append(info.TokenKindEnum.Name);
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

        private string ParseMethodName(TokenKind nonTerminal)
        {
            return $"Parse{grammar.GetName(nonTerminal)}";
        }

        private string TokenKindAccessString(TokenKind kind)
        {
            return $"{info.TokenKindEnum.Name}.{grammar.GetName(kind)}";
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