using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.Dobby.Api.Model
{
    public class LoanScheme
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("minCollateral")]
        public int MinCollateral { get; set; }
    }

    public class CollateralAmount
    {
        [JsonProperty("raw")]
        public string? Raw { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("token")]
        public string? Token { get; set; }

        [JsonProperty("valueUsd")]
        public double ValueUsd { get; set; }
    }

    public class LoanAmount
    {
        [JsonProperty("raw")]
        public string? Raw { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("token")]
        public string? Token { get; set; }

        [JsonProperty("valueUsd")]
        public double ValueUsd { get; set; }
    }

    public class InterestAmount
    {
        [JsonProperty("raw")]
        public string? Raw { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("token")]
        public string? Token { get; set; }

        [JsonProperty("valueUsd")]
        public double ValueUsd { get; set; }
    }

    public class Vault
    {
        [JsonProperty("vaultId")]
        public string? VaultId { get; set; }

        [JsonProperty("ownerAddress")]
        public string? OwnerAddress { get; set; }

        [JsonProperty("loanScheme")]
        public LoanScheme? LoanScheme { get; set; }

        [JsonProperty("state")]
        public string? State { get; set; }

        [JsonProperty("collateralAmounts")]
        public List<CollateralAmount>? CollateralAmounts { get; set; }

        [JsonProperty("loanAmounts")]
        public List<LoanAmount>? LoanAmounts { get; set; }

        [JsonProperty("interestAmounts")]
        public List<InterestAmount>? InterestAmounts { get; set; }

        [JsonProperty("collateralValue")]
        public double CollateralValue { get; set; }

        [JsonProperty("loanValue")]
        public double LoanValue { get; set; }

        [JsonProperty("interestValue")]
        public double InterestValue { get; set; }

        [JsonProperty("informativeRatio")]
        public double InformativeRatio { get; set; }

        [JsonProperty("collateralRatio")]
        public int CollateralRatio { get; set; }

        [JsonProperty("liquidationHeight")]
        public int LiquidationHeight { get; set; }

        [JsonProperty("batchCount")]
        public int BatchCount { get; set; }

        [JsonProperty("liquidationPenalty")]
        public int LiquidationPenalty { get; set; }

        [JsonProperty("batches")]
        public List<object>? Batches { get; set; }
    }

    public class GetUserResponse
    {
        [JsonProperty("userId")]
        public string? UserId { get; set; }

        [JsonProperty("language")]
        public string? Language { get; set; }

        [JsonProperty("theme")]
        public string? Theme { get; set; }

        [JsonProperty("vaults")]
        public List<Vault>? Vaults { get; set; }
    }
}
