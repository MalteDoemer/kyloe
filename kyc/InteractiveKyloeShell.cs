using Kyloe;

namespace Kyc
{
    class InteractiveKyloeShell
    {

        public void Run()
        {
            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (input is null)
                    return;

                if (input.StartsWith("$"))
                    if (EvaluteDollarCommand(input))
                        continue;
                    else
                        break;

                // var tree = SyntaxTree.Parse(input);
                // Console.WriteLine();
                // tree.GetDiagnostics().WriteTo(Console.Out);
                // Console.WriteLine();
                // tree.WriteTo(Console.Out);

                var terminals = SyntaxTree.Terminals(input);

                foreach (var t in terminals)
                    Console.WriteLine($"{t.Kind}: {t.Text}");
            }
        }

        private bool EvaluteDollarCommand(string input)
        {
            if (input.StartsWith("$exit"))
            {
                return false;
            }
            else if (input.StartsWith("$clear"))
            {
                Console.Clear();
                return true;
            }
            else
            {
                Console.WriteLine($"Invalid dollar command: {input}");
                return true;
            }
        }
    }
}