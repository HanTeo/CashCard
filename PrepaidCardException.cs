using System;

namespace CashCard
{
    public class PrepaidCardException : Exception
    {
        public PrepaidCardException(string message): base(message)
        {
        }
    }

    public class UnauthenticatedException : PrepaidCardException
    {
        public UnauthenticatedException() : base("Unauthenticated")
        {
        }
    }

    public class InsufficientBalanceException : PrepaidCardException
    {
        public InsufficientBalanceException() : base("Insufficient balance")
        {
        }
    }

    public class NegativeAmountException : PrepaidCardException
    {
        public NegativeAmountException() : base("Amount must be positive")
        {
        }
    }

    public class WrongPinException : PrepaidCardException
    {
        public WrongPinException() : base("Wrong PIN")
        {
        }
    }
}