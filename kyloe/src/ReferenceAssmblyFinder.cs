using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Immutable;

namespace Kyloe
{
    internal static class ReferenceAssmblyFinder
    {

        private static ImmutableArray<string> RefrenceAssemblies;

        public static IEnumerable<string> GetReferenceAssemblies() => RefrenceAssemblies;

        static ReferenceAssmblyFinder()
        {
            RefrenceAssemblies = FindRefrenceAssemblies().ToImmutableArray();
        }

        private static IEnumerable<string> FindRefrenceAssemblies()
        {
            var assemblyDir = GetDotnetRefrenceDirectory();
            System.Console.WriteLine(assemblyDir);
            yield return Path.Join(assemblyDir, "System.Runtime.dll");
            yield return Path.Join(assemblyDir, "System.Runtime.Extensions.dll");
        }


        private static string GetDotnetRefrenceDirectory()
        {
            var dotnetRoot = System.Environment.GetEnvironmentVariable("DOTNET_ROOT");

            if (string.IsNullOrEmpty(dotnetRoot))
                throw new Exception("unable to find .NET installation");

            var dotnetProcess = new Process();
            dotnetProcess.StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                FileName = "dotnet",
                Arguments = "--version",
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            dotnetProcess.Start();
            dotnetProcess.WaitForExit();

            var dotnetVersion = Version.Parse(dotnetProcess.StandardOutput.ReadToEnd().TrimEnd());

            var dir = Path.Join(dotnetRoot, "packs", "Microsoft.NETCore.App.Ref");

            foreach (var subdir in Directory.EnumerateDirectories(dir))
            {
                var subdirName = Path.GetFileName(subdir);
                var dirVersion = Version.Parse(subdirName);

                if (dirVersion.Major == dotnetVersion.Major && dirVersion.Minor == dotnetVersion.Minor)
                    return Path.Join(dir, subdirName, "ref", "net6.0"); // TODO: don't hadcode net6.0
            }

            throw new Exception("unable to find sdk directory");
        }
    }
}