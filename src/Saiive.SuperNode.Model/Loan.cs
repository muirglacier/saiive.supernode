using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Saiive.SuperNode.Model
{
    public class Loan
    {
    }

    public class LoanScheme
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("minColRatio")]
        public string MinColRatio { get; set; }

        [JsonProperty("interestRate")]
        public string InterestRate { get; set; }
    }

    public class Creation
    {
        [JsonProperty("tx")]
        public string Tx { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Destruction
    {
        [JsonProperty("tx")]
        public string Tx { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class Token
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("symbolKey")]
        public string SymbolKey { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("decimal")]
        public int Decimal { get; set; }

        [JsonProperty("limit")]
        public string Limit { get; set; }

        [JsonProperty("mintable")]
        public bool Mintable { get; set; }

        [JsonProperty("tradeable")]
        public bool Tradeable { get; set; }

        [JsonProperty("isDAT")]
        public bool IsDAT { get; set; }

        [JsonProperty("isLPS")]
        public bool IsLPS { get; set; }

        [JsonProperty("finalized")]
        public bool Finalized { get; set; }

        [JsonProperty("minted")]
        public string Minted { get; set; }

        [JsonProperty("creation")]
        public Creation Creation { get; set; }

        [JsonProperty("destruction")]
        public Destruction Destruction { get; set; }

        [JsonProperty("collateralAddress")]
        public string CollateralAddress { get; set; }

        [JsonProperty("displaySymbol")]
        public string DisplaySymbol { get; set; }
    }

    public class LoanCollateral
    {
        [JsonProperty("tokenId")]
        public string TokenId { get; set; }

        [JsonProperty("token")]
        public Token Token { get; set; }

        [JsonProperty("factor")]
        public string Factor { get; set; }

        [JsonProperty("priceFeedId")]
        public string PriceFeedId { get; set; }

        [JsonProperty("activateAfterBlock")]
        public int ActivateAfterBlock { get; set; }
    }

    public class LoanToken
    {
        [JsonProperty("tokenId")]
        public string TokenId { get; set; }

        [JsonProperty("token")]
        public Token Token { get; set; }

        [JsonProperty("interest")]
        public string Interest { get; set; }

        [JsonProperty("fixedIntervalPriceId")]
        public string FixedIntervalPriceId { get; set; }
    }
}
