resource "azurerm_traffic_manager_profile" "traffic_manager" {
  name                = "${var.prefix}-${var.environment}"
  resource_group_name = var.resource_group

  traffic_routing_method = "Weighted"

  dns_config {
    relative_name = "${var.environment}-${var.prefix}"
    ttl           = 100
  }

  monitor_config {
    protocol                     = "https"
    port                         = 443
    path                         = "/v1/api/health"
    interval_in_seconds          = 30
    timeout_in_seconds           = 9
    tolerated_number_of_failures = 3
  }

  tags = {
    environment = var.environment
  }
}


locals {
    cname = var.environment == "prod" ? var.name :  "${var.environment}-${var.name}"
}


resource "azurerm_dns_cname_record" "traffic_manager_cname" {
  name                = local.cname
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  target_resource_id  = azurerm_traffic_manager_profile.traffic_manager.id
}