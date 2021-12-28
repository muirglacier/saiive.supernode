output "connection" {
  value = azurerm_servicebus_namespace.servicebus.default_primary_connection_string 
}

output "export_q" {
  value = azurerm_servicebus_queue.export_q.name
}