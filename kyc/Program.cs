using System.Diagnostics.CodeAnalysis;
using Kyloe;
using Kyloe.Backend;
using Kyloe.Utility;
using Mono.Options;

namespace Kyc
{
    class Program
    {
        public static int Main(string[] args)
        {
            bool help = false;
            bool interactive = false;
            bool printLoweredTree = false;
            bool printSyntaxTree = false;

            string? outputPath = null;

            var referencePaths = new List<string>();
            var sourcePaths = new List<string>();

            var options = new OptionSet() {
                {"i|interactive", "starts a interactive kyloe shell", value => interactive = value is not null },
                {"h|help", "show this message and exit", value => help = value is not null },
                {"print-ir", "print intermidiate representation to the console", value => printLoweredTree = value is not null },
                {"print-tree", "print syntax tree to the console", value => printSyntaxTree = value is not null },
                {"o|output=", "the path to the output file", value => outputPath = value },
                {"r|reference=", "the path to a reference dll", value => referencePaths.Add(value) },
                {"<>", value => sourcePaths.Add(value) }
            };

            try
            {
                options.Parse(args);
            }
            catch (OptionException e)
            {
                PrintErrorAndExit(e.Message);
            }

            if (help)
            {
                ShowHelp(options);
                return 0;
            }

            if (interactive)
            {
                if (sourcePaths.Count > 0 || outputPath is not null)
                    PrintErrorAndExit("too many arguments for interactive mode");

                var i = new InteractiveKyloeShell();
                i.Run();
                return 0;
            }

            if (sourcePaths.Count == 0)
                PrintErrorAndExit("no input files");

            if (outputPath is null)
                PrintErrorAndExit("output path required");


            try
            {
                var programName = Path.GetFileNameWithoutExtension(outputPath);
                var sources = sourcePaths.Select(path => SourceText.FromFile(path));

                var opts = new CompilationOptions()
                {
                    BackendKind = BackendKind.Cecil,
                    ProgramName = programName,
                    ProgramPath = outputPath,
                    GenerateOutput = true,
                    RequireMain = true,
                };

                var compilation = Compilation.Compile(sources, referencePaths, opts);

                if (printSyntaxTree)
                    compilation.WriteSyntaxTree(Console.Out);

                if (printLoweredTree)
                    compilation.WriteLoweredTree(Console.Out);

                compilation.GetDiagnostics().WriteTo(Console.Error);

                return compilation.GetDiagnostics().HasErrors() ? 1 : 0;
            }
            catch (IOException ioException)
            {
                PrintErrorAndExit(ioException.Message);
            }

            return 0;
        }

        [DoesNotReturnAttribute]
        private static void PrintErrorAndExit(string message)
        {
            Console.Error.Write("kyc: ");
            Console.Error.WriteLine(message);
            System.Environment.Exit(1);
        }

        private static void ShowHelp(OptionSet options)
        {
            Console.WriteLine("Usage: kyc [OPTION]... [FILE]...");
            Console.WriteLine();
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
        }
    }
}