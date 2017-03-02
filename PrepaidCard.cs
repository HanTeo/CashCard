using System;
using System.Linq;

namespace CashCard
{
    public class PrepaidCard
    {
        private readonly int[] _pin;
        private readonly object _locker;
        private volatile int _balance;

        public bool Authenticated { get; private set; }

        public int Balance
        {
            get
            {
                return _balance;
            }

            private set
            {
                _balance = value;
            }
        }

        public PrepaidCard(int[] pin)
        {
            _pin = pin;
            _locker = new object();
        }

        public void Authenticate(int[] pin)
        {
            lock (_locker)
            {
                if (!pin.SequenceEqual(_pin))
                {
                    throw new WrongPinException();
                }

                Authenticated = true;
            }
        }

        public void Add(int amount)
        {
            lock(_locker)
            {
                GuardAgainstNotAuthenticated();

                GuardAgainstNegative(amount);

                Balance += amount;
            }
        }

        public void Withdraw(int amount)
        {
            lock (_locker)
            {
                GuardAgainstNotAuthenticated();

                GuardAgainstNegative(amount);

                GuardAgainstInsufficientBalance(amount);

                Balance -= amount;
            }
        }

        private void GuardAgainstInsufficientBalance(int amount)
        {
            if (Balance < amount)
            {
                throw new InsufficientBalanceException();
            }
        }

        private static void GuardAgainstNegative(int amount)
        {
            if (amount < 0)
            {
                throw new NegativeAmountException();
            }
        }

        private void GuardAgainstNotAuthenticated()
        {
            if (!Authenticated)
            {
                throw new UnauthenticatedException();
            }
        }
    }
}