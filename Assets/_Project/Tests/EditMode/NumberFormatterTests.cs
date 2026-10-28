using NUnit.Framework;
using VertigoCase.UI;

namespace VertigoCase.Tests
{
    [TestFixture]
    public sealed class NumberFormatterTests
    {
        [TestCase(0, "0")]
        [TestCase(1, "1")]
        [TestCase(99, "99")]
        [TestCase(999, "999")]
        [TestCase(1634, "1,634")]
        [TestCase(9999, "9,999")]
        [TestCase(10000, "10K")]
        [TestCase(15500, "15.5K")]
        [TestCase(100000, "100K")]
        [TestCase(1000000, "1M")]
        [TestCase(2500000, "2.5M")]
        [TestCase(10000000, "10M")]
        public void Format_ReturnsExpected(int value, string expected)
        {
            Assert.AreEqual(expected, NumberFormatter.Format(value));
        }

        [Test]
        public void Format_SmallValues_ReturnsCachedInstance()
        {
            string a = NumberFormatter.Format(42);
            string b = NumberFormatter.Format(42);
            Assert.AreSame(a, b);
        }
    }
}
