using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.SuperNode.Model
{
    public class Count
    {
        [JsonProperty("blocks")]
        public int Blocks { get; set; }

        [JsonProperty("prices")]
        public int Prices { get; set; }

        [JsonProperty("tokens")]
        public int Tokens { get; set; }

        [JsonProperty("masternodes")]
        public int Masternodes { get; set; }

        [JsonProperty("collateralTokens")]
        public int CollateralTokens { get; set; }

        [JsonProperty("loanTokens")]
        public int LoanTokens { get; set; }

        [JsonProperty("openAuctions")]
        public int OpenAuctions { get; set; }

        [JsonProperty("openVaults")]
        public int OpenVaults { get; set; }

        [JsonProperty("schemes")]
        public int Schemes { get; set; }
    }

    public class Burned
    {
        [JsonProperty("address")]
        public double Address { get; set; }

        [JsonProperty("emission")]
        public double Emission { get; set; }

        [JsonProperty("fee")]
        public int Fee { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }
    }

    public class Tvl
    {
        [JsonProperty("dex")]
        public double Dex { get; set; }

        [JsonProperty("masternodes")]
        public double Masternodes { get; set; }

        [JsonProperty("loan")]
        public double Loan { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }
    }

    public class DFIPrice
    {
        [JsonProperty("usd")]
        public double Usd { get; set; }

        [JsonProperty("usdt")]
        public double Usdt { get; set; }
    }

    public class Locked
    {
        [JsonProperty("weeks")]
        public int Weeks { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("tvl")]
        public double Tvl { get; set; }
    }

    public class Masternodes
    {
        [JsonProperty("locked")]
        public List<Locked> Locked { get; set; }
    }

    public class Value
    {
        [JsonProperty("collateral")]
        public double Collateral { get; set; }

        [JsonProperty("loan")]
        public double Loan { get; set; }
    }

    public class StatsLoan
    {
        [JsonProperty("count")]
        public Count Count { get; set; }

        [JsonProperty("value")]
        public Value Value { get; set; }
    }

    public class Emission
    {
        [JsonProperty("masternode")]
        public double Masternode { get; set; }

        [JsonProperty("dex")]
        public double Dex { get; set; }

        [JsonProperty("community")]
        public double Community { get; set; }

        [JsonProperty("anchor")]
        public double Anchor { get; set; }

        [JsonProperty("burned")]
        public double Burned { get; set; }

        [JsonProperty("total")]
        public double Total { get; set; }
    }

    public class Net
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("subversion")]
        public string Subversion { get; set; }

        [JsonProperty("protocolversion")]
        public int Protocolversion { get; set; }
    }

    public class Blockchain
    {
        [JsonProperty("difficulty")]
        public double Difficulty { get; set; }
    }

    public class StatsModel
    {
        [JsonProperty("count")]
        public Count Count { get; set; }

        [JsonProperty("burned")]
        public Burned Burned { get; set; }

        [JsonProperty("tvl")]
        public Tvl Tvl { get; set; }

        [JsonProperty("price")]
        public DFIPrice Price { get; set; }

        [JsonProperty("masternodes")]
        public Masternodes Masternodes { get; set; }

        [JsonProperty("loan")]
        public StatsLoan Loan { get; set; }

        [JsonProperty("emission")]
        public Emission Emission { get; set; }

        [JsonProperty("net")]
        public Net Net { get; set; }

        [JsonProperty("blockchain")]
        public Blockchain Blockchain { get; set; }
    }


}
