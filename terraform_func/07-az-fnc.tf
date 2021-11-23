

data "azurerm_key_vault_secret" "blockcypherapikey" {
  name         = "blockcypherapikey"
  key_vault_id = var.key_vault_id
}
data "azurerm_key_vault_secret" "send_grid_key" {
  name         = "SendGridApiKey"
  key_vault_id = var.key_vault_id
}



module "service_bus" {
  source = "./libs/servicebus"

  prefix = var.prefix
  location = var.location
  environment = var.environment
  resource_group = azurerm_resource_group.rg.name

  environment_tag = var.environment
}



module "function_app" {
  source = "./libs/function_app"

  tier = var.tier
  size = var.size
  always_on = var.always_on

  prefix = var.prefix
  location = var.location
  environment = var.environment
  environment_tag = var.environment_tag
  resource_group = azurerm_resource_group.rg.name

  function_app_file = "function.zip"  
  app_version = var.app_version

  dns_zone = var.dns_zone
  dns_zone_resource_group = var.dns_zone_resource_group

  bitcore_url = "https://bitcore.scw-prod-0.saiive.live"
  ocean_url = "https://ocean.defichain.com"
  defichain_api = "https://api.defichain.io"
  coingecko_url = "https://api.coingecko.com/api/v3"

  legacy_bitcoin_url = "https://bitcore.az-prod-0.saiive.live/"
  blockcypher_api = data.azurerm_key_vault_secret.blockcypherapikey.value

  servicebus_connection = module.service_bus.connection
  export_q = module.service_bus.export_q

  sendgrid_api_key = data.azurerm_key_vault_secret.send_grid_key.value
}