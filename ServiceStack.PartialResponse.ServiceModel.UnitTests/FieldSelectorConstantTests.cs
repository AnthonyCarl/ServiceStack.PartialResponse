using System.Reflection;
using Xunit;

namespace ServiceStack.Plugins.PartialResponse.UnitTests
{
    public class FieldSelectorConstantTests
    {
        [Fact]
        public void StartsWithReservedToken_IterateThroughAllReservedTokens_ReturnsTrue()
        {
            FieldInfo[] properties = typeof (FieldSelectorConstants).GetFields(
                BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fieldInfo in properties)
            {
                object reservedToken = fieldInfo.GetValue(typeof (FieldSelectorConstants));
                string reservedTokenAtBeginning = reservedToken + "ThisJustSome S T r i n G";
                Assert.True(FieldSelectorConstants.StartsWithReservedToken(reservedTokenAtBeginning));
            }
        }

        [Fact]
        public void StartsWithReservedToken_NullString_ReturnsFalse()
        {
            Assert.False(FieldSelectorConstants.StartsWithReservedToken(null));
        }

        [Fact]
        public void StartsWithReservedToken_EmptyString_ReturnsFalse()
        {
            Assert.False(FieldSelectorConstants.StartsWithReservedToken(string.Empty));
        }

        [Fact]
        public void StartsWithReservedToken_WhitespaceString_ReturnsFalse()
        {
            Assert.False(FieldSelectorConstants.StartsWithReservedToken("   \t  "));
        }
    }
}