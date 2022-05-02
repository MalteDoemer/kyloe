using Xunit;

using System.Linq;
using Kyloe.Lowering;

namespace Kyloe.Tests.Lowering
{
    public static class LoweredTreeAssert
    {
        public static void AssertNode(VerifyNode verify, LoweredNode lowered)
        {
            Assert.Equal(verify.Kind, lowered.Kind);

            Assert.Equal(verify.Children.Length, lowered.Children().Count());

            foreach (var (verifyChild, loweredChild) in verify.Children.Zip(lowered.Children()))
                AssertNode(verifyChild, loweredChild);
        }
    }
}