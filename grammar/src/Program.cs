using System;
using System.IO;
using System.Linq;

namespace Kyloe.Grammar
{
    public class Program
    {

        public static void Main(string[]? args)
        {
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
                if (args.Length != 3)
                {
                    Usage();
                    return;
                }

                var output = args[2];

                using (var outFile = new StreamWriter(output))
                {
                    var writer = new CodeGen.GeneratorWriter(outFile, 4);
                    var info = new ParserGeneratorInfo(locationClassName: "SourceLocation");
                    var generator = new ParserGenerator(grammar, info);

                    generator
                        .CreateCompilationUnit("GeneratorTest", CodeGen.AccessModifier.Public)
                        .Generate(writer);

                    return;
                }
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
            Console.WriteLine("\tthe path to a .cs file that should be used as output");
            Console.WriteLine();
            Console.WriteLine($"COMMAND:");
            Console.WriteLine($"\tfirst RULE\t\tprints the first set of RULE");
            Console.WriteLine($"\tfollow RULE\t\tprints the follow set of RULE");
            Console.WriteLine($"\trules\t\t\tprints all rules");
            Console.WriteLine($"\tterminals\t\tprints all terminals");
            Console.WriteLine($"\tgenerate OUTPUT\t\tgenerates the parser to OUTPUT");
        }
    }
}