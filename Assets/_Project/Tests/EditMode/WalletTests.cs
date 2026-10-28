using NUnit.Framework;
using VertigoCase.Core;

namespace VertigoCase.Tests
{
    [TestFixture]
    public sealed class WalletTests
    {
        [Test]
        public void NewWallet_HasStartingBalance()
        {
            var wallet = new InMemoryWallet(100);
            Assert.AreEqual(100, wallet.Balance);
        }

        [Test]
        public void NewWallet_NegativeStart_ClampedToZero()
        {
            var wallet = new InMemoryWallet(-50);
            Assert.AreEqual(0, wallet.Balance);
        }

        [Test]
        public void CanAfford_Exactly_ReturnsTrue()
        {
            var wallet = new InMemoryWallet(50);
            Assert.IsTrue(wallet.CanAfford(50));
        }

        [Test]
        public void CanAfford_MoreThanBalance_ReturnsFalse()
        {
            var wallet = new InMemoryWallet(50);
            Assert.IsFalse(wallet.CanAfford(51));
        }

        [Test]
        public void CanAfford_NegativeAmount_ReturnsFalse()
        {
            var wallet = new InMemoryWallet(50);
            Assert.IsFalse(wallet.CanAfford(-1));
        }

        [Test]
        public void TrySpend_DeductsBalance_ReturnsTrue()
        {
            var wallet = new InMemoryWallet(100);
            bool result = wallet.TrySpend(40);

            Assert.IsTrue(result);
            Assert.AreEqual(60, wallet.Balance);
        }

        [Test]
        public void TrySpend_InsufficientFunds_ReturnsFalse_BalanceUnchanged()
        {
            var wallet = new InMemoryWallet(10);
            bool result = wallet.TrySpend(20);

            Assert.IsFalse(result);
            Assert.AreEqual(10, wallet.Balance);
        }

        [Test]
        public void TrySpend_FiresBalanceChanged()
        {
            var wallet = new InMemoryWallet(100);
            int? received = null;
            wallet.BalanceChanged += b => received = b;

            wallet.TrySpend(30);

            Assert.AreEqual(70, received);
        }

        [Test]
        public void Add_IncreasesBalance_FiresBalanceChanged()
        {
            var wallet = new InMemoryWallet(50);
            int? received = null;
            wallet.BalanceChanged += b => received = b;

            wallet.Add(25);

            Assert.AreEqual(75, wallet.Balance);
            Assert.AreEqual(75, received);
        }

        [Test]
        public void Add_ZeroOrNegative_DoesNothing()
        {
            var wallet = new InMemoryWallet(50);
            bool fired = false;
            wallet.BalanceChanged += _ => fired = true;

            wallet.Add(0);
            Assert.AreEqual(50, wallet.Balance);
            Assert.IsFalse(fired);

            wallet.Add(-10);
            Assert.AreEqual(50, wallet.Balance);
            Assert.IsFalse(fired);
        }
    }
}
