using System.Linq;

namespace CashCard
{
    public class PrepaidCard
    {
        private readonly int[] _pin;
        private volatile int _balance;
        private volatile bool _authenticated;
        private readonly object _locker;

        public bool Authenticated => _authenticated;
        public int Balance => _balance;

        public PrepaidCard(int[] pin)
        {
            _pin = pin;
            _locker = new object();
        }

        public bool Authenticate(int[] pin)
        {
            lock (_locker)
            {
                if (!pin.SequenceEqual(_pin))
                {
                    return false;
                }

                _authenticated = true;
            }

            return true;
        }

        public bool Add(int amount)
        {
            lock(_locker)
            {
                if (!Authenticated)
                {
                    return false;
                }

                if (amount <= 0)
                {
                    return false;
                }

                _balance += amount;
            }

            return true;
        }

        public bool Withdraw(int amount)
        {
            lock (_locker)
            {
                if (!Authenticated)
                {
                    return false;
                }

                if (Balance < amount)
                {
                    return false;
                }

                _balance -= amount;
            }

            return true;
        }
    }
}