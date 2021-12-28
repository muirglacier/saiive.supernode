output "name" {
  value = azurerm_traffic_manager_profile.traffic_manager.name
}

output "cname" {
  value = azurerm_dns_cname_record.traffic_manager_cname.name
}

output "public_endpoint" {
  value = "${azurerm_dns_cname_record.traffic_manager_cname.name}.${var.dns_zone}"
}