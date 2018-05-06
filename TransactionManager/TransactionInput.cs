using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionManager
{
    public class TransactionInput
    {
        public string TransactionOutputHash { get; private set; }
        public TransactionOutput UTXO { get; set; }
        public TransactionInput(string TOH)
        {
            TransactionOutputHash = TOH;
        }
    }
}
