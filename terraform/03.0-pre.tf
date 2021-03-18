
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

data "azurerm_key_vault_secret" "application_insights_ikey" {
  name         = "${var.environment}-supernode-insights-instrumentation-key"
  key_vault_id = var.key_vault_id
}

locals {
    cname = var.environment == "prod" ? "supernode" :  "${var.environment}-supernode"
    uptime_cname_prefix = var.environment == "prod" ? "" :  "${var.environment}-"
    vm_username = "defi"
}
