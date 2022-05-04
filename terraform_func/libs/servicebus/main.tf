resource "azurerm_servicebus_namespace" "servicebus" {
  name                  = "${var.environment}-${var.prefix}-servicebus"
  resource_group_name   = var.resource_group
  location              = var.location
  sku                   = "Standard"

  tags = {
    Environment = var.environment_tag
  }
}

resource "azurerm_servicebus_queue" "export_q" {
  name                = "export-queue"
  resource_group_name = var.resource_group
  namespace_name      = azurerm_servicebus_namespace.servicebus.name

  enable_partitioning                   = true
  max_delivery_count                    = 5
  dead_lettering_on_message_expiration  = true
}