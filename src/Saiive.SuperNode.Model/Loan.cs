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

    public class Active
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("weightage")]
        public int Weightage { get; set; }

        [JsonProperty("oracles")]
        public Oracles Oracles { get; set; }
    }

    public class Next
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("weightage")]
        public int Weightage { get; set; }

        [JsonProperty("oracles")]
        public Oracles Oracles { get; set; }
    }

    public class ActivePrice
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("isLive")]
        public bool IsLive { get; set; }

        [JsonProperty("block")]
        public Block Block { get; set; }

        [JsonProperty("active")]
        public Active Active { get; set; }

        [JsonProperty("next")]
        public Next Next { get; set; }

        [JsonProperty("sort")]
        public string Sort { get; set; }
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

        [JsonProperty("activePrice")]
        public ActivePrice ActivePrice { get; set; }
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

        [JsonProperty("activePrice")]
        public ActivePrice ActivePrice { get; set; }
    }

    public class LoanVaultAmount
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("symbolKey")]
        public string SymbolKey { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displaySymbol")]
        public string DisplaySymbol { get; set; }


        [JsonProperty("activePrice")]
        public ActivePrice ActivePrice { get; set; }
    }

    public class LoanAuctionCollateral
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("symbolKey")]
        public string SymbolKey { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displaySymbol")]
        public string DisplaySymbol { get; set; }

        [JsonProperty("activePrice")]
        public ActivePrice ActivePrice { get; set; }
    }

    public class LoanVault
    {
        [JsonProperty("vaultId")]
        public string VaultId { get; set; }

        [JsonProperty("loanScheme")]
        public LoanScheme LoanScheme { get; set; }

        [JsonProperty("ownerAddress")]
        public string OwnerAddress { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("informativeRatio")]
        public string InformativeRatio { get; set; }

        [JsonProperty("collateralRatio")]
        public string CollateralRatio { get; set; }

        [JsonProperty("collateralValue")]
        public string CollateralValue { get; set; }

        [JsonProperty("loanValue")]
        public string LoanValue { get; set; }

        [JsonProperty("interestValue")]
        public string InterestValue { get; set; }

        [JsonProperty("collateralAmounts")]
        public List<LoanVaultAmount> CollateralAmounts { get; set; }

        [JsonProperty("loanAmounts")]
        public List<LoanVaultAmount> LoanAmounts { get; set; }

        [JsonProperty("interestAmounts")]
        public List<LoanVaultAmount> InterestAmounts { get; set; }
    }
    public class HighestBid
    {
        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("amount")]
        public Amount Amount { get; set; }
    }

    public class Batch
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("collaterals")]
        public List<LoanAuctionCollateral> Collaterals { get; set; }

        [JsonProperty("loan")]
        public Loan Loan { get; set; }

        [JsonProperty("highestBid")]
        public HighestBid HighestBid { get; set; }
    }

    public class LoanAuction
    {
        [JsonProperty("vaultId")]
        public string VaultId { get; set; }

        [JsonProperty("loanScheme")]
        public LoanScheme LoanScheme { get; set; }

        [JsonProperty("ownerAddress")]
        public string OwnerAddress { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("batchCount")]
        public int BatchCount { get; set; }

        [JsonProperty("liquidationHeight")]
        public int LiquidationHeight { get; set; }

        [JsonProperty("liquidationPenalty")]
        public int LiquidationPenalty { get; set; }

        [JsonProperty("batches")]
        public List<Batch> Batches { get; set; }
    }

    public class LoanAuctionHistory
    {
        [JsonProperty("winner")]
        public string Winner { get; set; }

        [JsonProperty("blockHeight")] 
        public ulong BlockHeight { get; set; }

        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }

        [JsonProperty("blockTime")]
        public ulong BlockTime { get; set; }

        [JsonProperty("vaultId")]
        public string VaultId { get; set; }

        [JsonProperty("batchIndex")]
        public ulong BatchIndex { get; set; }

        [JsonProperty("auctionBid")]
        public string AuctionBid { get; set; }

        [JsonProperty("auctionWon")]
        public IList<string> AuctionWon { get; set; }
    }
}
