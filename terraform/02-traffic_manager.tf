
module "traffic_manager" {
  source = "./libs/traffic_manager"

  name = "supernode"
  prefix = var.prefix
  environment = var.environment
  resource_group = azurerm_resource_group.rg.name

  dns_zone = var.dns_zone
  dns_zone_resource_group = var.dns_zone_resource_group
}
