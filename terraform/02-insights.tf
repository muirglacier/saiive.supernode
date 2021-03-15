resource "azurerm_application_insights" "supernode" {
  name                = "${var.environment}-supernode"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
  retention_in_days   = 120
}

resource "azurerm_key_vault_secret" "supernode_insights_key" {
  name         = "${var.environment}-supernode-insights-instrumentation-key"
  value        = azurerm_application_insights.supernode.instrumentation_key
  key_vault_id = var.key_vault_id
}