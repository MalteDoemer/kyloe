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

                
                var compilation = Compilation.Compile(input);
                Console.WriteLine();
                compilation.GetDiagnostics().WriteTo(Console.Out);
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