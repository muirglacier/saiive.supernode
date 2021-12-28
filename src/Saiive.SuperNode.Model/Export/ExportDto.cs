using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Saiive.SuperNode.Model.Export
{
    public enum ExportType
    {
        Text,
        Csv
    }

    public class ExportDto
    {
        [JsonProperty("chain")]
        public string Chain { get; set; }

        [JsonProperty("network")]
        public string Network { get; set; }

        [JsonProperty("addresses")]
        public List<string> Addresses { get; set; }

        [JsonProperty("from")]
        public DateTime From { get; set; }

        [JsonProperty("to")]
        public DateTime To { get; set; }

        [JsonProperty("paymentTxId")]
        public string PaymentTxId { get; set; }

        [JsonProperty("mail")]
        public string Mail { get; set; }

        [JsonProperty("exportType")]
        public ExportType ExportType { get; set; }
    }
}
