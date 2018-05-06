using System;
using System.Collections.Generic;
using System.Text;
using CryptoApi;
using Newtonsoft.Json;

namespace TransactionManager
{
    public class Transaction
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
        public string Signture { get; private set; }
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
        [JsonConstructor]
        public Transaction(Guid ID, string TransactionName, byte TransactionVersion, string Issuer, string Reciepient, double Amount, uint Sequence, string Signture,
    DateTime IssuanceTime, List<TransactionInput> TransactionInputs, List<TransactionOutput> TtransactionOutputs)
    : this(TransactionName, TransactionVersion, Issuer, Reciepient, Amount, Sequence, TransactionInputs)
        {
            this.ID = ID;
            this.Signture = Signture;
            this.TtransactionOutputs = TtransactionOutputs;
            this.IssuanceTime = IssuanceTime;
        }
        /// <summary>
        /// for issuing new transaction in wallet
        /// </summary>
        /// <param name="name">transaction name</param>
        /// <param name="version">transaction version</param>
        /// <param name="issuer">the issuer public key</param>
        /// <param name="reciepient">the reciepent public key</param>
        /// <param name="amount">amout of cash for sending</param>
        /// <param name="seq">sequance number</param>
        /// <param name="transactionInputs">inputs</param>
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
        public Transaction(Guid ID, string name, byte version, string issuer, string reciepient, double amount, uint seq, List<TransactionInput> transactionInputs, string HashString) :
            this(name, version, issuer, reciepient, amount, seq, transactionInputs)
        {
            TransactionHash = HashString;
        }
        /// <summary>
        /// in miner side for proccesing and verifying transaction
        /// </summary>
        /// <param name="json">transaction properties</param>
        public Transaction(string json)
        {
            var j = JsonConvert.DeserializeObject<Transaction>(json);
            ID = j.ID;
            TransactionVersion = j.TransactionVersion;
            TransactionName = j.TransactionName;
            Issuer = j.Issuer;
            Reciepient = j.Reciepient;
            Amount = j.Amount;
            Sequence = j.Sequence;
            Signture = j.Signture;
            _signture = new Signture(Signture);
            IssuanceTime = j.IssuanceTime;
            TransactionInputs = j.TransactionInputs;
            TtransactionOutputs = j.TtransactionOutputs;
        }
        public string GetHashString()
        {
            return (ID.ToString() + Issuer + Reciepient + Amount + TransactionVersion.ToString() + Sequence.ToString()).GetHashString();
        }
        #endregion
        #region signture
        /// <summary>
        /// using inside wallet for signing transaction
        /// </summary>
        /// <param name="PrivateKey">issuer privatekey path</param>
        public void GenerateSignture(string PrivateKey)
        {
            using (var rsa = new RsaSignatureProvider(PrivateKey))
            {
                var data = (ID.ToString() + Issuer + Reciepient + Amount + TransactionVersion.ToString() + Sequence.ToString());
                _signture = rsa.GenerateSignture(data);
                Signture = _signture.Value;
            }
        }
        /// <summary>
        /// for verifying transaction at miner side
        /// </summary>
        public bool IsSignatureVerified
        {
            get
            {
                bool ret = false;
                using (var rsa = new RsaSignatureProvider(Convert.FromBase64String(Issuer)))
                {
                    var data = (ID.ToString() + Issuer + Reciepient + Amount + TransactionVersion.ToString() + Sequence.ToString());
                    ret = rsa.VerifySignature(data, _signture); ;
                }

                return ret;
            }
        }
        #endregion
        #region Process

        #endregion
    }
}
