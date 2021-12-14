

# Create a resource group
resource "azurerm_resource_group" "rg" {
  name     = "${var.prefix}-${var.environment}"
  location = "West Europe"

  tags = {
    Environment = var.environment_tag
  }
}

resource "azurerm_resource_group" "rg_us" {
  name     = "${var.prefix}-${var.environment}-us"
  location = "Central US"

  tags = {
    Environment = var.environment_tag
  }
}
