using Newtonsoft.Json;
using System.Collections.Generic;

namespace Saiive.SuperNode.Model
{

    public class BitcoreVaultAddress
    {
        [JsonProperty("vaultId")]
        public string VaultId { get; set; }

        [JsonProperty("ownerAddress")]
        public string OwnerAddress { get; set; }

        [JsonProperty("loanSchemeId")]
        public string LoanSchemeId { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }
    }
    public class BitcoreVault
    {
        [JsonProperty("vaultId")]
        public string VaultId { get; set; }

        [JsonProperty("loanSchemeId")]
        public string LoanSchemeId { get; set; }

        [JsonProperty("ownerAddress")]
        public string OwnerAddress { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("collateralAmounts")]
        public List<string> CollateralAmounts { get; set; }

        [JsonProperty("loanAmounts")]
        public List<string> LoanAmounts { get; set; }

        [JsonProperty("interestAmounts")]
        public List<string> InterestAmounts { get; set; }

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
    }
}
