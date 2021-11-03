using Saiive.SuperNode.Model.Export;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saiive.SuperNode.Abstaction
{
  
    public interface IExportHandler
    {

        Task<bool> ExportAllowed(string chain, string network, string paymentTxId);
        Task<bool> Export(string chain, string network, List<string> addresses, DateTime from, DateTime to, string paymentTxId, string mail, ExportType exportType);
    }
}
