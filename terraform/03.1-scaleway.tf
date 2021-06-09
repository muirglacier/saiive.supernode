
data "template_file" "cloud_init_scaleway" {
  template   = file("${path.root}/cloud-init/cloud-init-scw.yml")

  vars = {
    docker_registry      = data.azurerm_key_vault_secret.docker_registry.value
    docker_registry_username = data.azurerm_key_vault_secret.docker_registry_username.value
    docker_registry_password = data.azurerm_key_vault_secret.docker_registry_password.value
    root_directory = "/root/node"
    analytics_id = var.analytics_id
    analytics_key = var.analytics_key
  }
}

module "chain_scw_network_nodes" {
  source = "./libs/scaleway-node"

  server_type = var.scaleway_server_type
  node_count = var.scaleway_node_count
  prefix = "scw"
  uptime_prefix = "scw"

  environment = var.environment
  cloud_init = data.template_file.cloud_init_scaleway.rendered
  
  dns_zone = var.dns_zone
  dns_zone_resource_group = var.dns_zone_resource_group
  
  resource_group = azurerm_resource_group.rg.name

  username = local.vm_username
  ssh_key = base64decode(data.azurerm_key_vault_secret.scalway_private_key.value)

  public_endpoint = "${local.cname}.${var.dns_zone}"

  docker_user     = data.azurerm_key_vault_secret.docker_registry_username.value
  docker_password = data.azurerm_key_vault_secret.docker_registry_password.value
  docker_registry = data.azurerm_key_vault_secret.docker_registry.value

  application_insights_ikey = data.azurerm_key_vault_secret.application_insights_ikey.value
}



module "chain_btc_scw_network_nodes" {
  source = "./libs/scaleway-node"

  server_type = var.scaleway_server_type
  node_count = var.scaleway_btc_node_count
  prefix = "btcscw"
  uptime_prefix = "btcscw"

  node_type = "bitcoin"
  node_prefix = "bitcoin"
  node_chain = "BTC"
  docker_node_config_dir = "/bitcoin/.bitcoin/config"
  docker_node_data_dir = "/bitcoin/.bitcoin/chain"

  environment = var.environment
  cloud_init = data.template_file.cloud_init_scaleway.rendered
  
  dns_zone = var.dns_zone
  dns_zone_resource_group = var.dns_zone_resource_group
  
  resource_group = azurerm_resource_group.rg.name

  username = local.vm_username
  ssh_key = base64decode(data.azurerm_key_vault_secret.scalway_private_key.value)

  public_endpoint = "${local.cname}.${var.dns_zone}"

  docker_user     = data.azurerm_key_vault_secret.docker_registry_username.value
  docker_password = data.azurerm_key_vault_secret.docker_registry_password.value
  docker_registry = data.azurerm_key_vault_secret.docker_registry.value

  application_insights_ikey = data.azurerm_key_vault_secret.application_insights_ikey.value
}
