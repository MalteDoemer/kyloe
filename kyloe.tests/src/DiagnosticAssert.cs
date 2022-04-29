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

        public static void Equals(DiagnosticResult result, params DiagnosticKind[] kinds)
        {
            Assert.Equal(kinds, result.Select(d => d.Kind));
        }
    }

}