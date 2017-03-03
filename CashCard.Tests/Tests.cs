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
        public void ThreadLocalAuthenticationTest()
        {
            var taskA = Task.Run(() =>
            {
                _card.Authenticate(_pin);
                _card.Add(100);
            });

            var taskB = Task.Run(() => _card.Add(50));

            Assert.DoesNotThrowAsync(async () => await taskA);
            Assert.ThrowsAsync<UnauthenticatedException>(async () => await taskB);
            Assert.AreEqual(100, _card.Balance);
            Assert.False(_card.Authenticated);
        }

        [Test]
        public void ThreadCollisionDeterministicTest()
        {
            _card.Authenticate(_pin);
            _card.Add(10000000);

            var taskA =Task.Run(() =>
            {
                _card.Authenticate(_pin);
                for (var i = 0; i < 5000000; i++)
                {
                    _card.Add(1);
                }
            });

            var taskB = Task.Run(() =>
            {
                _card.Authenticate(_pin);
                for (var i = 0; i < 5000000; i++)
                {
                    _card.Withdraw(1);
                }
            });

            var taskC =Task.Run(() =>
            {
                _card.Authenticate(_pin);
                for (var i = 0; i < 5000000; i++)
                {
                    _card.Add(1);
                }
            });

            var taskD = Task.Run(() =>
            {
                _card.Authenticate(_pin);
                for (var i = 0; i < 5000000; i++)
                {
                    _card.Withdraw(1);
                }
            });

            Task.WaitAll(taskA, taskB, taskC, taskD);

            Assert.AreEqual(10000000, _card.Balance);
        }
    }
}