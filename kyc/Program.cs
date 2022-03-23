using Kyloe;
using Kyloe.Utility;
using Mono.Cecil;
using Mono.Options;

namespace Kyc
{
    class Program
    {
        public static void Main(string[] args)
        {
            bool help = false;
            bool interactive = false;

            var refrences = new List<string>();

            var options = new OptionSet() {
                {"r|reference=", "adds a assembly to the reference assemblies", value => refrences.Add(value) },
                {"i|interactive", "starts a interactive kyloe shell", value => interactive = value is not null },
                {"h|help", "show this message and exit", value => help = value is not null },
            };

            List<string> extra;

            try
            {
                extra = options.Parse(args);
            }
            catch (OptionException e)
            {
                WrongUsage(options, e.Message);
                return;
            }

            if (help)
            {
                ShowHelp(options);
                return;
            }

            if (interactive)
            {
                if (extra.Count > 0)
                {
                    WrongUsage(options, "too many arguments for interactive mode");
                    return;
                }

                var i = new InteractiveKyloeShell();
                i.Run();
                return;
            }


            if (extra.Count == 0)
            {
                WrongUsage(options, "no input files");
                return;
            }

            if (extra.Count != 1)
            {
                WrongUsage(options, "for now only one file allowed");
                return;
            }

            try
            {
                var referenceAssemblies = refrences.Select(path => AssemblyDefinition.ReadAssembly(path));
                var text = SourceText.FromFile(extra[0]);
                var compilation = Compilation.Compile(text, referenceAssemblies);
                compilation.GetDiagnostics().WriteTo(Console.Out);
            }
            catch (IOException ioException)
            {
                WrongUsage(options, ioException.Message);
                return;
            }
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