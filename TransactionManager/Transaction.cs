﻿using System;
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
        public DateTime MinedTime { get; set; }
        public string ContainerBlockHash { get; set; }
        public uint BlockNumber { get; set; }
        public bool IsBlockReward { get; set; }
        public List<TransactionInput> TransactionInputs { get; private set; }
        public List<TransactionOutput> TransactionOutputs { get; private set; }
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
                foreach (var item in TransactionOutputs)
                {
                    retValue += item.Amount;
                }
                return retValue;
            }
        }
        #region ctor
        /// <summary>
        /// json constructor
        /// tip:never can be used!
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="TransactionName"></param>
        /// <param name="TransactionVersion"></param>
        /// <param name="Issuer"></param>
        /// <param name="Reciepient"></param>
        /// <param name="Amount"></param>
        /// <param name="Sequence"></param>
        /// <param name="Signture"></param>
        /// <param name="IssuanceTime"></param>
        /// <param name="TransactionInputs"></param>
        /// <param name="TtransactionOutputs"></param>
        /// <param name="IsBlockReward"></param>
        [JsonConstructor]
        public Transaction(Guid ID, string TransactionName, byte TransactionVersion, string Issuer, string Reciepient, double Amount, uint Sequence, string Signture,
    DateTime IssuanceTime, List<TransactionInput> TransactionInputs, List<TransactionOutput> TtransactionOutputs, bool IsBlockReward)
    : this(TransactionName, TransactionVersion, Issuer, Reciepient, Amount, Sequence, TransactionInputs)
        {
            this.ID = ID;
            this.Signture = Signture;
            this.TransactionOutputs = TtransactionOutputs;
            this.IssuanceTime = IssuanceTime;
            this.IsBlockReward = IsBlockReward;
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
            TransactionOutputs = new List<TransactionOutput>();
            IssuanceTime = DateTime.UtcNow;
            IsBlockReward = false;
        }
        /// <summary>
        /// Genesis Creator
        /// </summary>
        public Transaction(byte version, string issuer, string reciepient, double amount, uint seq, string HashString) :
            this("Genesis", version, issuer, reciepient, amount, seq, null)
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
            TransactionOutputs = j.TransactionOutputs;
            IsBlockReward = j.IsBlockReward;
        }
        /// <summary>
        /// uses in miners for creatging block reward
        /// </summary>
        /// <param name="version">block version</param>
        /// <param name="reciepient">sender and reciepient which both are the same</param>
        /// <param name="seq">block number</param>
        public Transaction(byte version, string reciepient, uint seq)
            : this("Block Reward", version, reciepient, reciepient, Utilities.BlockRewardForThisVersion, seq, null)
        {
            IsBlockReward = true;
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
        /// <summary>
        /// Processing Transaction In Miner
        /// </summary>
        /// <param name="checkGenesis">A function That returns whether a transaction is Genesis or not.</param>
        /// <param name="chechBlockReward">A function that indicate if Transaction is a Block Reward.</param>
        /// <param name="checkForAvailibilityInUTXOs">check if transaction inputs are valide in network.</param>
        /// <param name="cleaningUTXOs">clearing old inputs and adding new ones in network UTXOs and update UTXOs.</param>
        /// <returns></returns>
        public bool Process(
            Func<Transaction, bool> checkGenesis,
            Func<Transaction, bool> chechBlockReward,
            Func<List<TransactionInput>, bool> checkForAvailibilityInUTXOs,
            Func<List<TransactionInput>, List<TransactionOutput>, bool> cleaningUTXOs
            )
        {
            if (!IsSignatureVerified) return false;
            //check if Transaction Reward Or Genesis
            if (TransactionInputs == null)
            {
                if (!string.IsNullOrEmpty(TransactionHash)) return checkGenesis(this);
                return chechBlockReward(this);
            }
            if (!checkForAvailibilityInUTXOs(TransactionInputs))
            {
                return false;
            }
            var Change = InputsBalance - OutputsBalance;
            TransactionHash = GetHashString();
            TransactionOutputs.Add(new TransactionOutput(Reciepient, Amount, TransactionHash));
            TransactionOutputs.Add(new TransactionOutput(Issuer, Change, TransactionHash));
            return cleaningUTXOs(TransactionInputs, TransactionOutputs);
        }
        public bool FinishingTransacion(Func<string, string, bool> finalize)
        {
            return finalize(TransactionHash, ContainerBlockHash);
        }
        #endregion
    }
}
