using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ServiceStack.Plugins.PartialResponse.UnitTests
{
    [TestClass]
    public class FieldSelectorConstantTests
    {
        [TestMethod]
        public void StartsWithReservedToken_IterateThroughAllReservedTokens_ReturnsTrue()
        {
            FieldInfo[] properties = typeof (FieldSelectorConstants).GetFields(
                BindingFlags.Public | BindingFlags.Static);
            foreach (FieldInfo fieldInfo in properties)
            {
                object reservedToken = fieldInfo.GetValue(typeof (FieldSelectorConstants));
                string reservedTokenAtBeginning = reservedToken + "ThisJustSome S T r i n G";
                Assert.IsTrue(FieldSelectorConstants.StartsWithReservedToken(reservedTokenAtBeginning));
            }
        }

        [TestMethod]
        public void StartsWithReservedToken_NullString_ReturnsFalse()
        {
            Assert.IsFalse(FieldSelectorConstants.StartsWithReservedToken(null));
        }

        [TestMethod]
        public void StartsWithReservedToken_EmptyString_ReturnsFalse()
        {
            Assert.IsFalse(FieldSelectorConstants.StartsWithReservedToken(string.Empty));
        }

        [TestMethod]
        public void StartsWithReservedToken_WhitespaceString_ReturnsFalse()
        {
            Assert.IsFalse(FieldSelectorConstants.StartsWithReservedToken("   \t  "));
        }
    }
}