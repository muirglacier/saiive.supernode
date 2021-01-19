resource "azurerm_traffic_manager_endpoint" "endpoint" {
  name                = var.name
  resource_group_name = var.resource_group
  profile_name        = var.traffic_manager
  target              = var.target
  type                = var.target_type
  weight              = 100
  target_resource_id = var.target_resource_id
}


# resource "azurerm_traffic_manager_endpoint" "endpoint_extern" {
#   count = var.target_resource_id == "" ? 1 : 0
#   name                = var.name
#   resource_group_name = var.resource_group
#   profile_name        = var.traffic_manager
#   target              = var.target
#   type                = var.target_type
#   weight              = 100
# }