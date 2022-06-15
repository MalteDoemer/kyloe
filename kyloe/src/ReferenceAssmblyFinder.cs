using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Immutable;
using System.Reflection;

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
            yield return Path.Join(assemblyDir, "System.Runtime.dll");
            yield return Path.Join(assemblyDir, "System.Runtime.Extensions.dll");

            var currentAssemlby = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            System.Console.WriteLine(currentAssemlby);
            yield return Path.Join(currentAssemlby, "kyloe.builtins.dll");
        }

        private static string GetDotnetRoot()
        {
            var dotnetRoot = System.Environment.GetEnvironmentVariable("DOTNET_ROOT");

            if (!string.IsNullOrEmpty(dotnetRoot))
                return dotnetRoot;

            // DOTNET_ROOT variable was not set, try out some default values
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                if (Directory.Exists("C:\\Program Files\\dotnet"))
                    return "C:\\Program Files\\dotnet";
            }
            else if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                if (Directory.Exists("/usr/share/dotnet"))
                    return "/usr/share/dotnet";
            }

            throw new Exception("unable to find .NET installation");
        }

        private static string GetDotnetRefrenceDirectory()
        {
            var dotnetRoot = GetDotnetRoot();

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