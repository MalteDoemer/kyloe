using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using Kyloe.Syntax;

namespace Kyloe.Benchmarks
{
    public class BenchLexer
    {
        [Benchmark]
        public void LexSimple()
        {
            var text = "if true { } else { }";
            var terminals = SyntaxTree.Terminals(text);

            var consumer = new Consumer();
            terminals.Consume(consumer);
        }
    }

    class Program
    {
        public static void Main()
        {
            var summary = BenchmarkRunner.Run<BenchLexer>();
        }
    }
}