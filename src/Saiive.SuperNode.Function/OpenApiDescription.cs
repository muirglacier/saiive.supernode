using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Configurations;
using Microsoft.OpenApi.Models;
using System;

namespace Saiive.SuperNode.Function
{
    public class OpenApiConfigurationOptions : DefaultOpenApiConfigurationOptions
    {

        public override OpenApiInfo Info { get; set; } = new OpenApiInfo()
        {
            Version = "1.0.0",
            Title = "Triton DeFi API",
            Description = "Triton DeFi Wallet API for DeFiChain & Bitcoin.",
            TermsOfService = new Uri("https://static.tritonwallet.com/tos.html")
        };

    }
}
