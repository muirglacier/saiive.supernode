

module "frontdoor" {
  source = "./libs/frontdoor"

  name = "supernode"
  prefix = var.prefix
  environment = var.environment
  resource_group = azurerm_resource_group.rg.name
  location = var.location

  dns_zone = var.dns_zone
  dns_zone_resource_group = var.dns_zone_resource_group

  scaleway_nodes = module.chain_scaleway_network_nodes.nodes
}
