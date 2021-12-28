resource "random_integer" "ri" {
  min = 10000
  max = 99999
}

resource "azurerm_cosmosdb_account" "db" {
  name                = "${var.environment}-${var.prefix}-${random_integer.ri.result}"
  location            = var.location
  resource_group_name = var.resource_group
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"

  

  consistency_policy {
    consistency_level       = "BoundedStaleness"
    max_interval_in_seconds = 10
    max_staleness_prefix    = 200
  }

  capabilities {
    name = "EnableServerless"
  }

  geo_location {
    prefix            = "${var.environment}-${var.prefix}-${random_integer.ri.result}-geo"
    location          = var.location
    failover_priority = 0
  }
}

resource "azurerm_cosmosdb_sql_database" "sql_db" {
  name                = "${var.environment}-${var.prefix}"
  resource_group_name = azurerm_cosmosdb_account.db.resource_group_name
  account_name        = azurerm_cosmosdb_account.db.name
}

locals {
  comsos_collection_name = "${var.environment}-${var.prefix}-collection"
}

resource "azurerm_template_deployment" "sql_collection_arm" {
  name = "sql_cosmos_collection_arm_${var.environment}"

  resource_group_name   = var.resource_group
  deployment_mode       = "Incremental"
  template_body         = file("arm/cosmsos_db_collection.json")

  
  parameters = {
    cosmos = azurerm_cosmosdb_account.db.name
    cosmos_db = azurerm_cosmosdb_sql_database.sql_db.name
    cosmos_collection = local.comsos_collection_name
    cosmos_collection_partition_key = "/partitionKey"
  }
}