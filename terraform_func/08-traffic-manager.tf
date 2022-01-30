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
    path                         = "/api/v1/health"
    interval_in_seconds          = 30
    timeout_in_seconds           = 9
    tolerated_number_of_failures = 3
    expected_status_code_ranges  = ["200-204"]
  }

}

resource "azurerm_traffic_manager_endpoint" "endpoint_default" {
  name                =  "${var.prefix}-${var.environment}-endpoint-default"
  resource_group_name = azurerm_resource_group.rg.name
  profile_name        = azurerm_traffic_manager_profile.tm.name
  target              = substr(module.function_app.dns_name, 0, length(module.function_app.dns_name)-1)
  type                = "externalEndpoints"
  weight              = 100

  geo_mappings = ["WORLD"]
}

resource "azurerm_traffic_manager_endpoint" "endpoint_default_us" {
  name                =  "${var.prefix}-${var.environment}-endpoint-default_us"
  resource_group_name = azurerm_resource_group.rg.name
  profile_name        = azurerm_traffic_manager_profile.tm.name
  target              = substr(module.function_app_us.dns_name, 0, length(module.function_app_us.dns_name)-1)
  type                = "externalEndpoints"
  weight              = 100

  geo_mappings = ["US"]
}
locals {
    cname = var.environment == "prod" ? "supernode" :  "${var.environment}-supernode"
}
resource "azurerm_traffic_manager_endpoint" "endpoint_default_us_fallback" {
  name                =  "${var.prefix}-${var.environment}-endpoint-default_us_world"
  resource_group_name = azurerm_resource_group.rg.name
  profile_name        = azurerm_traffic_manager_profile.tm.name
  target              = substr(module.function_app.dns_name, 0, length(module.function_app.dns_name)-1)
  type                = "externalEndpoints"
  weight              = 1

  geo_mappings = ["US"]
}

resource "azurerm_dns_cname_record" "function_domain_name" {
  name                = local.cname
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  record              = azurerm_traffic_manager_profile.tm.fqdn
}
