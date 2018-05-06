using System;
using System.Collections.Generic;
using System.Text;
using CryptoApi;

namespace TransactionManager
{
    class Transaction
    {
        public Guid ID { get; private set; }
        public string TransactionName { get; private set; }
        public string TransactionHash { get; private set; }
        public byte TransactionVersion { get; set; }
        public string Issuer { get; private set; }
        public string Reciepient { get; private set; }
        public double Amount { get; set; }
        public uint Sequence { get; private set; }
        private Signture _signture { get; set; }
        public string Signture { get => _signture.Value; }
        public DateTime IssuanceTime { get; private set; }
        public DateTime ConfirmationTime { get; set; }
        public string FirstBlockHash { get; set; }
        public List<TransactionInput> TransactionInputs { get; private set; }
        public List<TransactionOutput> TtransactionOutputs { get; private set; }
        public double InputsBalance
        {
            get
            {
                double retValue = 0;
                foreach (var item in TransactionInputs)
                {
                    if (item.UTXO != null) retValue += item.UTXO.Amount;
                }
                return retValue;
            }
        }
        public double OutputsBalance
        {
            get
            {
                double retValue = 0;
                foreach (var item in TtransactionOutputs)
                {
                    retValue += item.Amount;
                }
                return retValue;
            }
        }
        #region ctor
        public Transaction(string name, byte version, string issuer, string reciepient, double amount, uint seq, List<TransactionInput> transactionInputs)
        {
            ID = Guid.NewGuid();
            TransactionName = name;
            TransactionVersion = version;
            Issuer = issuer;
            Reciepient = reciepient;
            Amount = amount;
            Sequence = seq;
            TransactionInputs = transactionInputs;
            TtransactionOutputs = new List<TransactionOutput>();
            IssuanceTime = DateTime.UtcNow;
        }
        /// <summary>
        /// Genesis Creator
        /// </summary>
        public Transaction(string name, byte version, string issuer, string reciepient, double amount, uint seq, List<TransactionInput> transactionInputs, string HashString) :
            this(name, version, issuer, reciepient, amount, seq, transactionInputs)
        {
            TransactionHash = HashString;
        }
        public string GetHashString()
        {
            return (Issuer + Reciepient + Amount + TransactionVersion.ToString() + Sequence.ToString()).GetHashString();
        }
        #endregion
        #region signture
        /// <summary>
        /// using inside wallet for signing transaction
        /// </summary>
        /// <param name="PrivateKey">issuer privatekey path</param>
        public void GenerateSignture(string PrivateKey)
        {
            RSASignatureProvider rsa = new RSASignatureProvider(PrivateKey);
            var data = (Issuer + Reciepient + Amount + TransactionVersion.ToString() + Sequence.ToString());
            _signture = rsa.CreateSigntureUnicode(data);
        }
        public bool IsSignatureVerified
        {
            get
            {
                RSASignatureProvider rsa = new RSASignatureProvider(Convert.FromBase64String(Issuer));
                var data = (Issuer + Reciepient + Amount + TransactionVersion.ToString() + Sequence.ToString());
                return rsa.VerifySignatureUnicode(data, _signture);
            }
        }
        #endregion
        #region Process

        #endregion
    }
}
