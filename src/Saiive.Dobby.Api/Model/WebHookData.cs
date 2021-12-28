using Newtonsoft.Json;

namespace Saiive.Dobby.Api.Model
{
    public class WebHookModel
    {
        [JsonProperty("type")]
        public string? Type { get; set; }

        [JsonProperty("data")]
        public WebHookData? Data { get; set; }
    }

    public class WebHookData
    {
        [JsonProperty("vaultId")]
        public string? VaultId { get; set; }

        [JsonProperty("ratio")]
        public int Ratio { get; set; }

        [JsonProperty("currentRatio")]
        public int CurrentRatio { get; set; }

        [JsonProperty("collateralAmount")]
        public double CollateralAmount { get; set; }

        [JsonProperty("loanValue")]
        public double LoanValue { get; set; }

        [JsonProperty("difference")]
        public double Difference { get; set; }

    }
}
