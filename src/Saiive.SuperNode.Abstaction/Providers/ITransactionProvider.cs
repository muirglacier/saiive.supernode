using Saiive.SuperNode.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction.Providers
{
    public interface ITransactionProvider
    {
        Task<TransactionModel> GetTransactionById(string network, string txId);
        Task<IList<TransactionModel>> GetTransactionsByBlock(string network, string block);
        Task<IList<BlockTransactionModel>> GetTransactionsByBlockHeight(string network, int height, bool includeDetails);

        Task<string> SendRawTransaction(string network, TransactionRequest request);
    }
}
