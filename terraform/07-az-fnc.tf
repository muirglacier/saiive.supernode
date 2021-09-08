
module "function_app" {
  source = "./libs/function_app"

  tier = "Standard"
  size = "S1"

  prefix = var.prefix
  location = var.location
  environment = var.environment
  environment_tag = var.environment_tag
  resource_group = azurerm_resource_group.rg.name

  function_app_file = "function.zip"  
  app_version = var.app_version

  bitcore_url = "https://bitcore.az-prod-0.saiive.live"
  ocean_url = "https://ocean.defichain.com"
  defichain_api = "https://api.defichain.io"
  coingecko_url = "https://api.coingecko.com/api/v3"
  blockcypher_api = data.azurerm_key_vault_secret.blockcypherapikey.value
}
