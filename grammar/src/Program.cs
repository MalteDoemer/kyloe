using System;
using System.Text.Json;
using System.IO;
using System.Linq;
using CodeGen;
using System.Text.Json.Serialization;

namespace Kyloe.Grammar
{
    public class Program
    {

        public static void Main(string[]? args)
        {
            // var testopts = new JsonSerializerOptions() { Converters = { new JsonStringEnumConverter() }, WriteIndented = true };
            // var test = new GeneratorInfo();
            // test.Namespace = "Test";
            // test.TokenKindEnum = new ClassInfo("TokenKindEnum", AccessModifier.Public);
            // test.ExtensionClass = new ClassInfo("ExtensionClass", AccessModifier.Public);
            // test.TokenClass = new ClassInfo("TokenClass", AccessModifier.Public);
            // test.TerminalClass = new ClassInfo("TerminalClass", AccessModifier.Public);
            // test.NodeClass = new ClassInfo("NodeClass", AccessModifier.Public);
            // test.LocationClass = new ClassInfo("LocationClass", AccessModifier.Public);
            // test.ErrorClass = new ClassInfo("ErrorClass", AccessModifier.Public);
            // test.ErrorKindEnum = new ClassInfo("ErrorKindEnum", AccessModifier.Public);
            // test.LexerClass = new ClassInfo("LexerClass", AccessModifier.Internal);
            // test.ParserClass = new ClassInfo("ParserClass", AccessModifier.Internal);

            // var gen = JsonSerializer.Serialize<GeneratorInfo>(test, testopts);
            // Console.WriteLine(gen);

            if (args is null || args.Length <= 1)
            {
                Usage();
                return;
            }

            var file = args[0];
            var command = args[1];

            var text = File.ReadAllText(file);
            var grammar = FinalGrammar.CreateFromText(text);

            if (command == "first")
            {
                if (args.Length != 3)
                {
                    Usage();
                    return;
                }

                var ruleName = args[2];
                var rule = grammar.LookupRule(ruleName);

                if (rule is null)
                {
                    Console.WriteLine($"error: the rule '{ruleName}' does not exist");
                    return;
                }

                var first = grammar.FirstSet(rule.Kind);
                var names = first.Select(token => grammar.GetName(token));
                Console.WriteLine($"FIRST = {{ {string.Join(", ", names)} }}");
                return;
            }
            else if (command == "follow")
            {
                if (args.Length != 3)
                {
                    Usage();
                    return;
                }

                var ruleName = args[2];
                var rule = grammar.LookupRule(ruleName);

                if (rule is null)
                {
                    Console.WriteLine($"error: the rule '{ruleName}' does not exist");
                    return;
                }

                var first = grammar.FollowSet(rule.Kind);
                var names = first.Select(token => grammar.GetName(token));
                Console.WriteLine($"FOLLOW = {{ {string.Join(", ", names)} }}");
                return;
            }
            else if (command == "rules")
            {
                if (args.Length != 2)
                {
                    Usage();
                    return;
                }

                foreach (var rule in grammar.Rules.Values)
                    Console.WriteLine(rule);

                return;
            }
            else if (command == "terminals")
            {
                if (args.Length != 2)
                {
                    Usage();
                    return;
                }

                foreach (var terminal in grammar.Terminals.Values)
                    Console.WriteLine(terminal);

                return;
            }
            else if (command == "generate")
            {
                if (args.Length != 4)
                {
                    Usage();
                    return;
                }

                var json = File.ReadAllText(args[2]);
                var outDir = args[3];
                var opts = new JsonSerializerOptions() { Converters = { new JsonStringEnumConverter() } };
                var info = JsonSerializer.Deserialize<GeneratorInfo>(json, opts);

                var generator = new ParserGenerator(grammar, info);

                foreach (var (name, unit) in generator.CreateMultipleCompilationUnits())
                {
                    var outfile = $"{outDir}{Path.DirectorySeparatorChar}{name}.cs";
                    using (var stream = new StreamWriter(outfile))
                    {
                        var writer = new GeneratorWriter(stream, 4);
                        unit.Generate(writer);
                    }
                }

                return;
            }




            Usage();
            return;
        }

        private static void Usage()
        {
            Console.WriteLine($"usage: {System.AppDomain.CurrentDomain.FriendlyName} GRAMMAR COMMAND [ARGS]");
            Console.WriteLine();
            Console.WriteLine("GRAMMAR:");
            Console.WriteLine("\tthe path to a grammar defintion file");
            Console.WriteLine();
            Console.WriteLine("RULE:");
            Console.WriteLine("\ta name of a rule defined in the grammar definition file");
            Console.WriteLine();
            Console.WriteLine("OUTPUT:");
            Console.WriteLine("\tthe output directory");
            Console.WriteLine();
            Console.WriteLine("CLASSES:");
            Console.WriteLine("\tthe path to a .json file that defines a ParserGeneratorInfo struct");
            Console.WriteLine();
            Console.WriteLine($"COMMAND:");
            Console.WriteLine($"\tfirst RULE\t\tprints the first set of RULE");
            Console.WriteLine($"\tfollow RULE\t\tprints the follow set of RULE");
            Console.WriteLine($"\trules\t\t\tprints all rules");
            Console.WriteLine($"\tterminals\t\tprints all terminals");
            Console.WriteLine($"\tgenerate CLASSES OUTPUT\tgenerates the code to OUTPUT using the names in CLASSES");
        }
    }
}