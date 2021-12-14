resource "azurerm_traffic_manager_profile" "tm" {
  name                =  "${var.prefix}-${var.environment}-tm"
  resource_group_name = azurerm_resource_group.rg.name

  traffic_routing_method = "Geographic"

  dns_config {
    relative_name =  "${var.prefix}-${var.environment}-tm"
    ttl           = 100
  }

  monitor_config {
    protocol                     = "https"
    port                         = 443
    path                         = "/"
    interval_in_seconds          = 30
    timeout_in_seconds           = 9
    tolerated_number_of_failures = 3
  }

}

resource "azurerm_traffic_manager_endpoint" "endpoint_default" {
  name                =  "${var.prefix}-${var.environment}-endpoint-default"
  resource_group_name = azurerm_resource_group.rg.name
  profile_name        = azurerm_traffic_manager_profile.tm.name
  target_resource_id   = module.function_app.id
  type                = "azureEndpoints"
  weight              = 100

  geo_mappings = ["WORLD"]
}

resource "azurerm_traffic_manager_endpoint" "endpoint_default_us" {
  name                =  "${var.prefix}-${var.environment}-endpoint-default_us"
  resource_group_name = azurerm_resource_group.rg.name
  profile_name        = azurerm_traffic_manager_profile.tm.name
  target_resource_id   = module.function_app_us.id
  type                = "azureEndpoints"
  weight              = 100

  geo_mappings = ["US"]
}