using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionManager
{
    public class TransactionOutput
    {
        public string HashString { get; private set; }
        public string Reciepient { get; private set; }
        public double Amount { get; private set; }
        public string ParentTransactionHash { get; private set; }
        public TransactionOutput(string reciepient, double amount, string parentTransactionhash)
        {
            Reciepient = reciepient;
            Amount = amount;
            ParentTransactionHash = parentTransactionhash;
            HashString = (Reciepient + Amount.ToString() + parentTransactionhash).GetHashString();
        }
        public bool IsMine(string Key)
        {
            return (Key == Reciepient);
        }
    }
}
