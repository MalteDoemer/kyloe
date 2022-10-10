using System;

namespace Kyloe
{
    public class Builtins
    {
        private static Random rnd = new Random();

        public static void println(object arg) => Console.WriteLine(arg);
        public static void println(string arg) => Console.WriteLine(arg);
        public static void println(long arg) => Console.WriteLine(arg);
        public static void println(int arg) => Console.WriteLine(arg);
        public static void println(short arg) => Console.WriteLine(arg);
        public static void println(sbyte arg) => Console.WriteLine(arg);
        public static void println(ulong arg) => Console.WriteLine(arg);
        public static void println(uint arg) => Console.WriteLine(arg);
        public static void println(ushort arg) => Console.WriteLine(arg);
        public static void println(byte arg) => Console.WriteLine(arg);
        public static void println(double arg) => Console.WriteLine(arg);
        public static void println(float arg) => Console.WriteLine(arg);

        public static void print(object arg) => Console.Write(arg);
        public static void print(string arg) => Console.Write(arg);
        public static void print(long arg) => Console.Write(arg);
        public static void print(int arg) => Console.Write(arg);
        public static void print(short arg) => Console.Write(arg);
        public static void print(sbyte arg) => Console.Write(arg);
        public static void print(ulong arg) => Console.Write(arg);
        public static void print(uint arg) => Console.Write(arg);
        public static void print(ushort arg) => Console.Write(arg);
        public static void print(byte arg) => Console.Write(arg);
        public static void print(double arg) => Console.Write(arg);
        public static void print(float arg) => Console.Write(arg);

        public static double random() => rnd.NextDouble();
        public static double random(double max) => rnd.NextDouble() * max;
        public static double random(double min, double max) => rnd.NextDouble() * (max - min) + min;

        public static void exit() => System.Environment.Exit(0);
        public static void exit(int code) => System.Environment.Exit(code);

        public static string input(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? "";
        }

        public static int len(string arg) => arg.Length;
    }
}