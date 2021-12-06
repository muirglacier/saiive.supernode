using Saiive.BlockCypher.Core.Objects;
using Saiive.SuperNode.Model;

namespace Saiive.SuperNode.Bitcoin.Helper
{
    internal static class ModelConverter
    {
        public static TransactionModel ToTransactionModel(this Transaction transaction, string network, string address)
        {
            var details = new TransactionDetailModel();

            foreach (var inp in transaction.Inputs)
            {
                details.Inputs.Add(new TransactionModel
                {
                    Address = inp.Addresses[0],
                    Chain = "BTC",
                    MintTxId = transaction.Hash,
                    MintHeight = (int)transaction.BlockHeight,
                    MintIndex = (int)inp.OutputIndex,
                    Value = (ulong)inp.OutputValue.ValueLong

                });
            }
            foreach (var outs in transaction.Outputs)
            {
                details.Outputs.Add(new TransactionModel
                {
                    Address = outs.Addresses[0],
                    Chain = "BTC",
                    MintTxId = transaction.Hash,
                    SpentTxId = outs.SpentBy,
                    Value = (ulong)outs.Value.ValueLong

                });
            }

            return new TransactionModel
            {
                Address = address,
                Chain = "BTC",
                Id = transaction.Hash,
                IsCustom = false,
                IsCustomTxApplied = true,
                MintHeight = (int)transaction.BlockHeight,
                Network = network,
                Value = (ulong)transaction.Total.ValueLong,
                Details = details
            };
        }
        public static TransactionDetailModel ToTransactionDetailModel(this Transaction transaction, string network, string address)
        {
            var ret = new TransactionDetailModel();

            foreach (var inp in transaction.Inputs)
            {
                ret.Inputs.Add(new TransactionModel
                {
                    Address = inp.Addresses[0],
                    Chain = "BTC",
                    MintTxId = transaction.Hash,
                    MintHeight = (int)transaction.BlockHeight,
                    MintIndex = (int)inp.OutputIndex,
                    Value = (ulong)inp.OutputValue.ValueLong

                });
            }
            foreach (var outs in transaction.Outputs)
            {
                ret.Outputs.Add(new TransactionModel
                {
                    Address = outs.Addresses[0],
                    Chain = "BTC",
                    MintTxId = transaction.Hash,
                    SpentTxId = outs.SpentBy,
                    Value = (ulong)outs.Value.ValueLong

                });
            }


            return ret;
        }

        public static TransactionModel ToTransactionModel(this TxReference tx, string network, string address)
        {
            return new TransactionModel
            {
                Address = address,
                Id = tx.TxHash,
                MintTxId = tx.TxHash,
                MintHeight = tx.BlockHeight,
                Value = (ulong)tx.Value.ValueLong,
                MintIndex = tx.TxOutputN,
                Confirmations = tx.Confirmations,
                SpentTxId = tx.SpentBy
            };
        }

        public static BlockTransactionModel ToBlockTransactionModel(this Transaction tx, string network, string address)
        {
            return new BlockTransactionModel
            {
                Id = tx.Hash,
                Txid = tx.Hash,
                Network = network,
                BlockHeight = tx.BlockHeight,
                BlockHash = tx.BlockHash,
                BlockTime = tx.Received,
                BlockTimeNormalized = tx.Received,
                Locktime = tx.LockTime,
                InputCount = tx.Inputs.Count,
                OutputCount = tx.Outputs.Count,
                Size = tx.VinSz + tx.VoutSz,
                Fee = tx.Fees.ValueLong,
                Value = (ulong)tx.Total.ValueLong,
                Confirmations = tx.Confirmations
            };
        }

        public static BlockModel ToBlockModel(this BlockCypher.Core.Objects.Block block, string network)
        {
            return new BlockModel
            {
                Id = block.Hash,
                Height = block.Height,
                MerkleRoot = block.MrklRoot,
                Network = network,
                Reward = (ulong)block.Fees,
                Nonce = (ulong)block.Nonce,
                Size = block.Size,
                Time = block.Time.ToString(),
                PreviousBlockHash = block.PrevBlock,
                Hash = block.Hash,
                Bits = (ulong)block.Bits
            };
        }
    }
}
