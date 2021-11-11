using CsvHelper;
using Saiive.DeFiChain.Sharp.Parser;
using Saiive.SuperNode.Abstaction;
using Saiive.SuperNode.Model;
using Saiive.SuperNode.Model.Export;
using Saiive.SuperNode.Model.Requests;
using SendGrid.Helpers.Mail;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Export
{
    public class ExportInfo
    {
        public DateTime Date { get; set; }
        public long BlockHeight { get; set; }

        public string? OperationType { get; set; }
        public ulong Amount { get; set; }

        public string? TransactionId { get; set; }
    }

    public class ExportHandler : IExportHandler
    {
        private readonly ChainProviderCollection _chainProviderCollection;
        private readonly MailHandler _mailHandler;
        private readonly string PaymentAddressTestnet = "tXmZ6X4xvZdUdXVhUKJbzkcN2MNuwVSEWv";
        private readonly string PaymentAddressMainnet = "dResgN7szqZ6rysYbbj2tUmqjcGHD4LmKs";

        public ExportHandler(ChainProviderCollection chainProviderCollection, MailHandler mailHandler)
        {
            _chainProviderCollection = chainProviderCollection;
            _mailHandler = mailHandler;
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
            var chainProv = _chainProviderCollection.GetInstance(chain);


            var transactions = await chainProv.AddressProvider.GetTransactions(network, new AddressesBodyRequest
            {
                Addresses = addresses
            });

            var toMaxDateTime = to;

            if(network != "testnet")
            {
                var tx = await chainProv.TransactionProvider.GetTransactionById(network, paymentTxId);

                toMaxDateTime = tx.BlockTime;
            }

            transactions = transactions.Where(a => a.BlockTime >= from && a.BlockTime <= to).ToList();

            var txDetails = new List<TransactionModel>();

            var exportInfo = new List<ExportInfo>();

            foreach(var tx in transactions)
            {
                try
                {
                    var txDetail = await chainProv.TransactionProvider.GetTransactionById(network, String.IsNullOrEmpty(tx.MintTxId) ? tx.SpentTxId : tx.MintTxId);

                    txDetails.Add(txDetail);

                    var isFromInput = txDetail.Details.Inputs.Any(a => addresses.Contains(a.Address));
                    var isFromOutput = txDetail.Details.Outputs.Any(a => addresses.Contains(a.Address));

                    if (isFromInput || isFromOutput) {

                        foreach (var outp in txDetail.Details.Outputs)
                        {
                            if (!String.IsNullOrEmpty(outp.Script) && outp.Script[0..2] == "6a")
                            {
                                var script = outp.Script.ToByteArray()[2..];
                                if (DefiScriptParser.IsDeFiTransaction(script))
                                {
                                    var defiTx = DefiScriptParser.Parse(script);
                                    outp.ParesedTransactionType = defiTx;

                                    exportInfo.Add(new ExportInfo
                                    {
                                        Date = txDetail.BlockTime,
                                        BlockHeight = txDetail.BlockHeight,
                                        OperationType = defiTx.TxType.ToString(),
                                        Amount = 0,
                                        TransactionId = txDetail.SpentTxId
                                    });
                                }
                            }
                            else if(!addresses.Contains(outp.Address))
                            {
                                continue;
                            }
                            else if(isFromInput && !isFromOutput)
                            {
                                exportInfo.Add(new ExportInfo
                                {
                                    Date = txDetail.BlockTime,
                                    BlockHeight = txDetail.BlockHeight,
                                    OperationType = "Move funds",
                                    Amount = 0,
                                    TransactionId = txDetail.SpentTxId
                                });
                            }
                            else if(!isFromInput && isFromOutput)
                            {
                                exportInfo.Add(new ExportInfo
                                {
                                    Date = txDetail.BlockTime,
                                    BlockHeight = txDetail.BlockHeight,
                                    OperationType = "Received funds",
                                    Amount = 0,
                                    TransactionId = txDetail.MintTxId
                                });
                            }
                        }
                    }
                }
                catch(Exception ex)
                {

                }
            }

            var aggregatedData = await chainProv.AddressProvider.GetAggregatedAddresses(network, new AddressesBodyRequest { Addresses = addresses });
            var balance = await chainProv.AddressProvider.GetTotalBalance(network, new AddressesBodyRequest() { Addresses = addresses });

            var export = ConvertToCsv(exportInfo, "Export.csv");
            var txDetailsWriter = ConvertToCsv(txDetails, "Transactions.csv");
            var aggregatedDataWriter = ConvertToCsv(aggregatedData, "AggregatedAddressData.csv");
            var balanceWriter = ConvertToCsv(balance, "Balances.csv");

            try
            {
                await _mailHandler.Send(mail, export, txDetailsWriter, aggregatedDataWriter, balanceWriter);
            }
            catch(Exception e)
            {
                return false;
            }
            return true;
        }

        public Attachment ConvertToCsv<T>(T any, string name) where T : IEnumerable
        {
            using var textWriter = new StringWriter();
            using var csv = new CsvWriter(textWriter, CultureInfo.InvariantCulture);
            csv.WriteRecords(any);


            return new Attachment()
            {
                Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(textWriter.ToString())),
                Filename = name,
                Type = "application/csv"

            };

        }

        public async Task<bool> ExportAllowed(string chain, string network, string paymentTxId)
        {
            if (string.IsNullOrEmpty(chain))
            {
                throw new ArgumentException($"{nameof(chain)} cannot be null or empty");
            }
            if (string.IsNullOrEmpty(network))
            {
                throw new ArgumentException($"{nameof(network)} cannot be null or empty");
            }
            if (string.IsNullOrEmpty(paymentTxId))
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

            var paymentAddress = network == "testnet" ? PaymentAddressTestnet : PaymentAddressMainnet;
            var hasPaymentOutput = tx.Details.Outputs.Any(a => a.Address == paymentAddress);

            if(!hasPaymentOutput)
            {
                return false;
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
