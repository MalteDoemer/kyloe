namespace Kyloe
{
    public class Builtins
    {
        public static void println(object arg) => System.Console.WriteLine(arg);
        public static void println(string arg) => System.Console.WriteLine(arg);
        public static void println(long arg) => System.Console.WriteLine(arg);
        public static void println(int arg) => System.Console.WriteLine(arg);
        public static void println(short arg) => System.Console.WriteLine(arg);
        public static void println(sbyte arg) => System.Console.WriteLine(arg);
        public static void println(ulong arg) => System.Console.WriteLine(arg);
        public static void println(uint arg) => System.Console.WriteLine(arg);
        public static void println(ushort arg) => System.Console.WriteLine(arg);
        public static void println(byte arg) => System.Console.WriteLine(arg);
        public static void println(double arg) => System.Console.WriteLine(arg);
        public static void println(float arg) => System.Console.WriteLine(arg);

        public static void print(object arg) => System.Console.Write(arg);
        public static void print(string arg) => System.Console.Write(arg);
        public static void print(long arg) => System.Console.Write(arg);
        public static void print(int arg) => System.Console.Write(arg);
        public static void print(short arg) => System.Console.Write(arg);
        public static void print(sbyte arg) => System.Console.Write(arg);
        public static void print(ulong arg) => System.Console.Write(arg);
        public static void print(uint arg) => System.Console.Write(arg);
        public static void print(ushort arg) => System.Console.Write(arg);
        public static void print(byte arg) => System.Console.Write(arg);
        public static void print(double arg) => System.Console.Write(arg);
        public static void print(float arg) => System.Console.Write(arg);

        public static string input(string prompt)
        {
            System.Console.Write(prompt);
            return System.Console.ReadLine() ?? "";
        }

        public static int len(string arg) => arg.Length;
    }
}