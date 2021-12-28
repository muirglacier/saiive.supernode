

data "azurerm_key_vault_secret" "azure_vm_public_key" {
  name         = "azure-vm-public-key"
  key_vault_id = var.key_vault_id
}
data "azurerm_key_vault_secret" "azure_vm_private_key" {
  name         = "azure-vm-private-key"
  key_vault_id = var.key_vault_id
}


data "template_file" "cloud_init_azure" {
  template   = file("${path.root}/cloud-init/cloud-init.yml")

  vars = {
    docker_registry      = data.azurerm_key_vault_secret.docker_registry.value
    docker_registry_username = data.azurerm_key_vault_secret.docker_registry_username.value
    docker_registry_password = data.azurerm_key_vault_secret.docker_registry_password.value
    root_directory = "/home/${local.vm_username}/node"
    analytics_id = var.analytics_id
    analytics_key = var.analytics_key
  }
}

module "chain_az_network_nodes" {
  source = "./libs/azure-node"

  node_count = var.azure_node_count
  prefix = "az"
  uptime_prefix = "az"

  disk_size = 1000

  environment = var.environment
  location = var.location
  cloud_init = data.template_file.cloud_init_azure.rendered
  
  dns_zone = var.dns_zone
  dns_zone_resource_group = var.dns_zone_resource_group
  
  resource_group = azurerm_resource_group.rg.name

  username = local.vm_username
  ssh_key = base64decode(data.azurerm_key_vault_secret.azure_vm_private_key.value)
  ssh_pub_key = base64decode(data.azurerm_key_vault_secret.azure_vm_public_key.value)

  public_endpoint = "${local.cname}.${var.dns_zone}"

  docker_user = data.azurerm_key_vault_secret.docker_registry_username.value
  docker_password = data.azurerm_key_vault_secret.docker_registry_password.value
  docker_registry = data.azurerm_key_vault_secret.docker_registry.value

  application_insights_ikey = data.azurerm_key_vault_secret.application_insights_ikey.value
  blockcypher_api_key = data.azurerm_key_vault_secret.blockcypherapikey.value
}
