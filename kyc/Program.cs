using Kyloe.Syntax;

namespace Kyc
{
    class Program
    {
        public static void Main()
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


                var tree = SyntaxTree.Parse(input);
                Console.WriteLine();
                tree.GetDiagnostics().WriteTo(Console.Out);
                Console.WriteLine();
                tree.GetRoot().WriteTo(Console.Out);

            }
        }

        private static bool EvaluteDollarCommand(string input)
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