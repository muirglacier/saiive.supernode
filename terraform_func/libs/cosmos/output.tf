output "connection_strings" {
  value = azurerm_cosmosdb_account.db.connection_strings
}

output "endpoint" {
  value = azurerm_cosmosdb_account.db.endpoint
}

output "primary_master_key" {
  value = azurerm_cosmosdb_account.db.primary_master_key
}

output "name" {
  value = azurerm_cosmosdb_sql_database.sql_db.name
}

output "table" {
  value = local.comsos_collection_name
}