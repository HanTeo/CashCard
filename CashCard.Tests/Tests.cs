using System.Threading.Tasks;
using NUnit.Framework;

namespace CashCard.Tests
{
    [TestFixture]
    public class Tests
    {
        private int[] _pin;
        private PrepaidCard _card;

        [SetUp]
        public void Setup()
        {
            _pin = new[] {1, 2, 3, 4};
            _card = new PrepaidCard(_pin);
        }

        [Test]
        public void DefaultUnauthenticated()
        {
            Assert.False(_card.Authenticated);
        }

        [Test]
        public void DefaultZeroBalance()
        {
            Assert.AreEqual(0, _card.Balance);
        }

        [Test]
        public void AuthenticatedWithWrongPinRemainsUnauthenticated()
        {
            var wrongPin = new[] {4, 3, 2, 1};

            Assert.Throws<WrongPinException>(() => _card.Authenticate(wrongPin));
            Assert.False(_card.Authenticated);
        }

        [Test]
        public void AuthenticatedWithCorrectPinAuthenticates()
        {
            _card.Authenticate(_pin);

            Assert.True(_card.Authenticated);
        }

        [Test]
        public void CannotAddIfUnauthenticated()
        {
            Assert.Throws<UnauthenticatedException>(() => _card.Add(10));
            Assert.AreEqual(0, _card.Balance);
        }

        [Test]
        public void CanAddIfAuthenticated()
        {
            _card.Authenticate(_pin);
            _card.Add(10);

            Assert.AreEqual(10, _card.Balance);
        }

        [Test]
        public void CannotAddNegativeAmount()
        {
            _card.Authenticate(_pin);

            Assert.Throws<NegativeAmountException>(() => _card.Add(-10));
            Assert.AreEqual(0, _card.Balance);
        }

        [Test]
        public void CannotWithdrawIfUnauthenticated()
        {
            Assert.Throws<UnauthenticatedException>(() => _card.Withdraw(10));
            Assert.AreEqual(0, _card.Balance);
        }

        [Test]
        public void CannotWithdrawNegativeAmount()
        {
            _card.Authenticate(_pin);

            Assert.Throws<NegativeAmountException>(() => _card.Withdraw(-10));
            Assert.AreEqual(0, _card.Balance);
        }

        [Test]
        public void CanWithdrawIfAuthenticatedAndSufficientBalance()
        {
            _card.Authenticate(_pin);
            _card.Add(20);
            _card.Withdraw(10);

            Assert.AreEqual(10, _card.Balance);
        }

        [Test]
        public void CannotWithdrawIfAuthenticatedButInsufficientBalance()
        {
            _card.Authenticate(_pin);
            _card.Add(10);

            Assert.Throws<InsufficientBalanceException>(() => _card.Withdraw(20));
            Assert.AreEqual(10, _card.Balance);
        }

        [Test]
        public async Task AuthenticatedIsPerThread()
        {
            var threadA = Task.Run(() =>
            {
                _card.Authenticate(_pin);
                return _card.Authenticated;
            });

            var threadB = Task.Run(() => _card.Authenticated);

            Assert.AreNotEqual(await threadA, await threadB);
        }
    }
}