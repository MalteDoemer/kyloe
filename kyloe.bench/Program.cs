using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;

namespace Kyloe.Benchmarks
{
    public class KyloeBencher
    {
        [Benchmark]
        public void LexSmall()
        {
            var text = "func main() { var x = 5; if true { x += 1; }  else { x -= 1; } }";
            var terminals = SyntaxTree.Terminals(text);
            var consumer = new Consumer();
            terminals.Consume(consumer);
        }

        [Benchmark]
        public Compilation CompileSmall()
        {
            var text = "func main() { var x = 5; if true { x += 1; }  else { x -= 1; } }";
            var compilation = Compilation.Compile(text);
            return compilation;
        }

        [Benchmark]
        public void LexLarge()
        {
            var terminals = SyntaxTree.Terminals(LargeCode.code);

            var consumer = new Consumer();
            terminals.Consume(consumer);
        }

        [Benchmark]
        public Compilation CompileLarge()
        {
            var compilation = Compilation.Compile(LargeCode.code);
            return compilation;
        }
    }

    class Program
    {
        public static void Main()
        {
            var summary = BenchmarkRunner.Run<KyloeBencher>();
        }
    }
}