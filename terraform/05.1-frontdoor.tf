
# module "frontdoor" {
#   depends_on = [
#     local.nodes
#   ]
  
#   source = "./libs/frontdoor"

#   name = "supernode"
#   prefix = var.prefix
#   environment = var.environment
#   resource_group = azurerm_resource_group.rg.name
#   location = var.location

#   dns_zone = var.dns_zone
#   dns_zone_resource_group = var.dns_zone_resource_group

#   nodes = local.nodes
#   bitcoin_nodes = local.bitcoin_nodes
#   dfi_nodes = local.dfi_nodes
# }


# resource "azurerm_monitor_diagnostic_setting" "frontdoor_log_storage" {

#   name               = "${var.prefix}-${var.environment}"
#   target_resource_id = module.frontdoor.id

#   log_analytics_workspace_id = azurerm_log_analytics_workspace.analytics.id 

#   log {
#     category = "FrontdoorAccessLog"

#     retention_policy {
#       enabled = true
#       days    = 60
#     }
#   }

#   metric {
#     category = "AllMetrics"

#     retention_policy {
#       enabled = true
#       days    = 60
#     }
#   }
# }