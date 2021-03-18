

module "chain_scaleway_network_nodes" {
  source = "./libs/scaleway-node"

  server_type = var.scaleway_server_type
  node_count = var.scaleway_node_count
  prefix = var.prefix
  environment = var.environment
  cloud_init = data.template_file.cloud_init.rendered
  
  dns_zone = var.dns_zone
  dns_zone_resource_group = var.dns_zone_resource_group
  
  resource_group = azurerm_resource_group.rg.name

  ssh_key = base64decode(data.azurerm_key_vault_secret.scalway_private_key.value)

  public_endpoint = "${local.cname}.${var.dns_zone}"

  docker_user = data.azurerm_key_vault_secret.docker_registry_username.value
  docker_password = data.azurerm_key_vault_secret.docker_registry_password.value

  application_insights_ikey = data.azurerm_key_vault_secret.application_insights_ikey.value
}
