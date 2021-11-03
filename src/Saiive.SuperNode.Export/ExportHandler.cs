using Saiive.DeFiChain.Sharp.Parser;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model.Export;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Export
{
    public class ExportHandler : IExportHandler
    {
        private readonly ChainProviderCollection _chainProviderCollection;

        public ExportHandler(ChainProviderCollection chainProviderCollection)
        {
            _chainProviderCollection = chainProviderCollection;
        }

        public async Task<bool> Export(string chain, string network, List<string> addresses, DateTime from, DateTime to, string paymentTxId, string mail, ExportType exportType)
        {
            if (!_chainProviderCollection.ContainsKey(chain))
            {
                throw new ArgumentException($"Chain {chain} not supported...");
            }

            if(!await ExportAllowed(chain, network, paymentTxId))
            {
                throw new ArgumentException($"Export not allowed!");
            }

            throw new NotImplementedException();
        }

        public async Task<bool> ExportAllowed(string chain, string network, string paymentTxId)
        {
            if (!string.IsNullOrEmpty(chain))
            {
                throw new ArgumentException($"{nameof(chain)} cannot be null or empty");
            }
            if (!string.IsNullOrEmpty(network))
            {
                throw new ArgumentException($"{nameof(network)} cannot be null or empty");
            }
            if (!string.IsNullOrEmpty(paymentTxId))
            {
                throw new ArgumentException($"{nameof(paymentTxId)} cannot be null or empty");
            }
            if (!_chainProviderCollection.ContainsKey(chain))
            {
                throw new ArgumentException($"Chain {chain} not supported...");
            }

            if(network == "testnet")
            {
                return true;
            }

            var chainProv = _chainProviderCollection.GetInstance(chain);

            var tx = await chainProv.TransactionProvider.GetTransactionById(network, paymentTxId);

            if(tx == null)
            {
                throw new ArgumentException("Tx not found!");
            }

            foreach(var outp in tx.Details.Outputs)
            {
                var script = outp.Script.ToByteArray();
                if (SaiiveScriptParser.IsSaiiveTransaction(script))
                {
                    var scriptType = SaiiveScriptParser.GetScriptType(script);
                    if(scriptType == SaiiveScriptType.Export)
                    {
                        return true;
                    }
                }
                
            }


            return false;
        }
    }
}
