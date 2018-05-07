using Newtonsoft.Json;
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
        public bool IsProcessing { get; set; }
        public string ParentTransactionHash { get; private set; }
        [JsonConstructor]
        public TransactionOutput(string Reciepient, double Amount, string ParentTransactionHash)
        {
            this.Reciepient = Reciepient;
            this.Amount = Amount;
            this.ParentTransactionHash = ParentTransactionHash;
            HashString = (Reciepient + Amount.ToString() + ParentTransactionHash).GetHashString();
            IsProcessing = false;
        }
        public bool IsMine(string Key)
        {
            return (Key == Reciepient) && !IsProcessing;
        }
    }
}
