using System.Linq;
using System.Threading;

namespace CashCard
{
    public class PrepaidCard
    {
        private readonly int[] _pin;
        private readonly object _locker;
        private readonly ThreadLocal<bool> _authenticated;

        public bool Authenticated
        {
            get { return _authenticated.Value; }
            private set { _authenticated.Value = value; }
        }

        public int Balance { get; private set; }

        public PrepaidCard(int[] pin)
        {
            _pin = pin;
            _locker = new object();
            _authenticated = new ThreadLocal<bool>(false);
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
            GuardAgainstNegative(amount);

            GuardAgainstNotAuthenticated();

            lock (_locker)
            {
                Balance += amount;
            }
        }

        public void Withdraw(int amount)
        {
            GuardAgainstNegative(amount);

            GuardAgainstNotAuthenticated();

            lock (_locker)
            {
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