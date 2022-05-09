using Kyloe;
using Kyloe.Utility;
using Mono.Cecil;
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

            var options = new OptionSet() {
                {"i|interactive", "starts a interactive kyloe shell", value => interactive = value is not null },
                {"h|help", "show this message and exit", value => help = value is not null },
                {"print-ir", "print intermidiate representation to the console", value => printLoweredTree = value is not null },
                {"print-tree", "print syntax tree to the console", value => printSyntaxTree = value is not null },
                {"o|output=", "the path to the output file", value => outputPath = value },
                {"r|reference=", "the path to a reference dll", value => referencePaths.Add(value) },
            };

            List<string> extra;

            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                WrongUsage(options, e.Message);
                return -1;
            }

            if (help)
            {
                ShowHelp(options);
                return 0;
            }

            if (interactive)
            {
                if (extra.Count > 0 || outputPath is not null)
                {
                    WrongUsage(options, "too many arguments for interactive mode");
                    return -1;
                }

                var i = new InteractiveKyloeShell();
                i.Run();
                return 0;
            }


            if (extra.Count == 0)
            {
                WrongUsage(options, "no input files");
                return -1;
            }

            if (extra.Count != 1)
            {
                WrongUsage(options, "for now only one file allowed");
                return -1;
            }

            if (outputPath is null)
            {
                WrongUsage(options, "output path required");
                return -1;
            }

            try
            {
                var filePath = extra[0];
                var programName = Path.GetFileNameWithoutExtension(outputPath);
                var text = SourceText.FromFile(filePath);
                var opts = new CompilationOptions() { RequireMain = true };
                var compilation = Compilation.Compile(text, opts);
                compilation.GetDiagnostics().WriteTo(Console.Out);
                
                if (printSyntaxTree)
                    compilation.WriteSyntaxTree(Console.Out);

                if (printLoweredTree)
                    compilation.WriteLoweredTree(Console.Out);

                compilation.CreateProgram(programName, outputPath);
            }
            catch (IOException ioException)
            {
                Console.WriteLine(ioException);
                WrongUsage(options, ioException.Message);
                return -1;
            }

            return 0;
        }

        private static void WrongUsage(OptionSet options, string message)
        {
            Console.Write("kyc: ");
            Console.WriteLine(message);
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