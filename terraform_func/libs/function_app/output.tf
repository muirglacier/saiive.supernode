output "hostname" {
  value = azurerm_function_app.functions.default_hostname
}
output "id" {
  value = azurerm_function_app.functions.id
}

output "dns_name" {
  value = azurerm_dns_cname_record.function_domain_name.fqdn
}