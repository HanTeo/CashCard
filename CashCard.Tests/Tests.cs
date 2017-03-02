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
            var wrongPin = new[] {4, 3, 2, 1};

            Assert.Throws<WrongPinException>(() => card.Authenticate(wrongPin));
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

            Assert.Throws<UnauthenticatedException>(() => card.Add(10));
            Assert.AreEqual(0, card.Balance);
        }

        [Test]
        public void CanAddIfAuthenticated()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);

            card.Authenticate(pin);
            card.Add(10);

            Assert.AreEqual(10, card.Balance);
        }

        [Test]
        public void CannotAddNegativeAmount()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);

            card.Authenticate(pin);

            Assert.Throws<NegativeAmountException>(() => card.Add(-10));
            Assert.AreEqual(0, card.Balance);
        }

        [Test]
        public void CannotWithdrawIfUnauthenticated()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);

            Assert.Throws<UnauthenticatedException>(() => card.Withdraw(10));
            Assert.AreEqual(0, card.Balance);
        }

        [Test]
        public void CannotWithdrawNegativeAmount()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);

            card.Authenticate(pin);

            Assert.Throws<NegativeAmountException>(() => card.Withdraw(-10));
            Assert.AreEqual(0, card.Balance);
        }

        [Test]
        public void CanWithdrawIfAuthenticatedAndSufficientBalance()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);

            card.Authenticate(pin);
            card.Add(20);
            card.Withdraw(10);

            Assert.AreEqual(10, card.Balance);
        }

        [Test]
        public void CannotWithdrawIfAuthenticatedButInsufficientBalance()
        {
            var pin = new[] {1, 2, 3, 4};
            var card = new PrepaidCard(pin);

            card.Authenticate(pin);
            card.Add(10);

            Assert.Throws<InsufficientBalanceException>(() => card.Withdraw(20));
            Assert.AreEqual(10, card.Balance);
        }
    }
}