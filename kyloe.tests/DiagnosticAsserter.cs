using System.Linq;
using Xunit;
using Kyloe.Diagnostics;


namespace Kyloe.Tests
{

    internal static class DiagnosticAssert
    {
        public static void NoDiagnostics(DiagnosticResult result)
        {
            Assert.Empty(result.GetAll());
        }

        public static void NoErrors(DiagnosticResult result)
        {
            Assert.Empty(result.GetErrors());
        }

        public static void NoWarnings(DiagnosticResult result)
        {
            Assert.Empty(result.GetWarnings());
        }

        public static void HasAll(DiagnosticResult result, params DiagnosticType[] types)
        {
            foreach (var type in types)
                Assert.Contains(result.GetAll(), d => d.Type == type);
        }
    }

}