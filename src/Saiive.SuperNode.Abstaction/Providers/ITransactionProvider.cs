using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction.Providers
{
    public interface ITransactionProvider
    {
        Task<TransactionModel> GetTransactionById(string network, string txId, bool onlyConfirmed=false);
        Task<IList<TransactionModel>> GetTransactionsByBlock(string network, string block);
        Task<IList<BlockTransactionModel>> GetTransactionsByBlockHeight(string network, int height, bool includeDetails);

        Task<string> SendRawTransaction(string network, TransactionRequest request);

        Task<object> DecodeRawTransaction(string network, TransactionRequest rawTx);
        Task<object> DecodeRawTransactionFromTxId(string network, string txId);
    }
}
