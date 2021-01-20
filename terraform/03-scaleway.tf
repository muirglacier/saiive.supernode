
data "azurerm_key_vault_secret" "docker_registry" {
  name         = "defichain-container-registry"
  key_vault_id = var.key_vault_id
}

data "azurerm_key_vault_secret" "docker_registry_username" {
  name         = "defichain-container-registry-username"
  key_vault_id = var.key_vault_id
}

data "azurerm_key_vault_secret" "docker_registry_password" {
  name         = "defichain-container-registry-password"
  key_vault_id = var.key_vault_id
}

data "azurerm_key_vault_secret" "scalway_private_key" {
  name         = "scaleway-vm-private-key"
  key_vault_id = var.key_vault_id
}

data "template_file" "cloud_init" {
  template   = file("${path.root}/cloud-init/cloud-init.yml")

  vars = {
    docker_registry      = data.azurerm_key_vault_secret.docker_registry.value
    docker_registry_username = data.azurerm_key_vault_secret.docker_registry_username.value
    docker_registry_password = data.azurerm_key_vault_secret.docker_registry_password.value
  }
}

locals {
    cname = var.environment == "prod" ? "supernode" :  "${var.environment}-supernode"
}


module "chain_scaleway_network_nodes" {
  source = "./libs/scaleway-node"

  node_count = 2
  prefix = var.prefix
  environment = var.environment
  cloud_init = data.template_file.cloud_init.rendered
  
  dns_zone = var.dns_zone
  dns_zone_resource_group = var.dns_zone_resource_group
  
  resource_group = azurerm_resource_group.rg.name
  network="testnet"

  ssh_key = base64decode(data.azurerm_key_vault_secret.scalway_private_key.value)

  public_endpoint = "${local.cname}.${var.dns_zone}"
}
