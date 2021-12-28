using System.Collections.Generic;
using Newtonsoft.Json;

namespace Saiive.SuperNode.Model
{
    public class TransactionDetailModel
    {
        public TransactionDetailModel()
        {
            Inputs = new List<TransactionModel>();
            Outputs = new List<TransactionModel>();
        }

        [JsonProperty("inputs")]
        public List<TransactionModel> Inputs { get; set; }
        
        [JsonProperty("outputs")]
        public List<TransactionModel> Outputs { get; set; }

    }
}
