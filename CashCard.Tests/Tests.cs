using NUnit.Framework;

namespace CashCard.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void DefaultUnauthenticated()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);

            Assert.False(card.Authenticated);
        }

        [Test]
        public void DefaultZeroBalance()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);

            Assert.AreEqual(0, card.Balance);
        }

        [Test]
        public void AuthenticatedWithWrongPinRemainsUnauthenticated()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);

            card.Authenticate(new[] {0, 0, 0, 0});

            Assert.False(card.Authenticated);
        }

        [Test]
        public void AuthenticatedWithCorrectPinAuthenticates()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);

            card.Authenticate(pin);

            Assert.True(card.Authenticated);
        }

        [Test]
        public void CannotAddIfUnauthenticated()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);
            var isAdded = card.Add(10);

            Assert.False(isAdded);
            Assert.AreEqual(0, card.Balance);
        }

        [Test]
        public void CanAddIfAuthenticated()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);

            var isAuthenticated = card.Authenticate(pin);
            var isAdded = card.Add(10);

            Assert.True(isAuthenticated);
            Assert.True(isAdded);
            Assert.AreEqual(10, card.Balance);
        }

        [Test]
        public void CannotWithdrawIfUnauthenticated()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);

            var canWithdraw = card.Withdraw(10);

            Assert.False(canWithdraw);
            Assert.AreEqual(0, card.Balance);
        }

        [Test]
        public void CanWithdrawIfAuthenticatedAndSufficientAmount()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);

            card.Authenticate(pin);
            var isAdded = card.Add(20);
            var canWithdraw = card.Withdraw(10);

            Assert.True(isAdded);
            Assert.True(canWithdraw);
            Assert.AreEqual(10, card.Balance);
        }

        [Test]
        public void CannotWithdrawIfAuthenticatedButInsufficientAmount()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);

            card.Authenticate(pin);
            var isAdded = card.Add(10);
            var canWithdraw = card.Withdraw(20);

            Assert.True(isAdded);
            Assert.False(canWithdraw);
            Assert.AreEqual(10, card.Balance);
        }
    }
}