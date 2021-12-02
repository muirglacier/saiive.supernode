
resource "random_string" "storage_name" {
    length = 24
    upper = false
    lower = true
    number = true
    special = false
}

# storage for the function app
resource "azurerm_storage_account" "storage" {
    name = random_string.storage_name.result
    resource_group_name = var.resource_group
    location = var.location
    account_tier = "Standard"
    account_replication_type = "LRS"    

    tags = {
        Environment = var.environment
    }
}


# storage container for the function app
resource "azurerm_storage_container" "deployments" {
    name = "function-releases"
    storage_account_name = azurerm_storage_account.storage.name
    container_access_type = "private"
}

# blob for the function app
resource "azurerm_storage_blob" "appcode" {
    name = "${var.environment}-${var.function_app_file}.zip"
    storage_account_name = azurerm_storage_account.storage.name
    storage_container_name = azurerm_storage_container.deployments.name
    type = "Block"
    source = "${var.app_version}-${var.function_app_file}"

}


data "azurerm_storage_account_sas" "sas" {
    connection_string = azurerm_storage_account.storage.primary_connection_string
    https_only = true
    start = "2019-01-01"
    expiry = "2021-12-31"
	
    resource_types {
        object = true
        container = false
        service = false
    }
	
    services {
        blob = true
        queue = false
        table = false
        file = false
    }
	
    permissions {
        read = true
        write = false
        delete = false
        list = false
        add = false
        create = false
        update = false
        process = false
    }
}


resource "azurerm_app_service_plan" "asp" {
    name = "${var.prefix}-${var.environment}"
    resource_group_name = var.resource_group
    location = var.location
    kind = "functionapp"
	reserved = false
    sku {
        tier = var.tier
        size = var.size
    }
}
locals {
    cname = var.environment == "prod" ? var.dns_name :  "${var.environment}-${var.dns_name}"
}

resource "azurerm_function_app" "functions" {
    name = "${var.prefix}-${var.environment}-function"
    location = var.location
    resource_group_name = var.resource_group
    app_service_plan_id = azurerm_app_service_plan.asp.id
    storage_connection_string = azurerm_storage_account.storage.primary_connection_string
    version = "~3"
    https_only = true

    app_settings = {
        https_only = true
        FUNCTIONS_WORKER_RUNTIME = "dotnet"
        FUNCTION_APP_EDIT_MODE = "readonly"
        WEBSITE_ENABLE_SYNC_UPDATE_SITE = "false"
        HASH = base64encode(filesha256("${var.app_version}-${var.function_app_file}"))
        WEBSITE_RUN_FROM_PACKAGE = "https://${azurerm_storage_account.storage.name}.blob.core.windows.net/${azurerm_storage_container.deployments.name}/${azurerm_storage_blob.appcode.name}${data.azurerm_storage_account_sas.sas.sas}"
        WEBSITE_LOAD_USER_PROFILE = 1
        WEBSITE_VNET_ROUTE_ALL = 1

        OpenApi__Title = "saiive.supernode push api"
        OpenApi__Description = "saiive.supernode push api"
        OpenApi__TermsOfService = "https://static.saiive.live/tos.html"

       // APPINSIGHTS_INSTRUMENTATIONKEY = var.environment == "prod" ? azurerm_application_insights.application_insights[0].instrumentation_key : ""

        CosmosDBName = var.cosmos_db_name
        CosmosDBCollection = var.cosmos_table_name
      
    }
    
    connection_string {
        name = "CosmosConnectionString"
        type = "SQLAzure"
        value = var.cosmos_connection_string
    }
   

    site_config {
        always_on = var.always_on
        
        cors {
            allowed_origins = ["*"]
        }
    }

    tags = {
        Environment = var.environment
    }
}


resource "azurerm_dns_cname_record" "function_domain_name" {
  name                = local.cname
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  record              = azurerm_function_app.functions.default_hostname
}

resource "azurerm_dns_txt_record" "function_domain_name_txt" {
  name                = "asuid.${local.cname}"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300

  record {
    value = azurerm_function_app.functions.custom_domain_verification_id
  }
}


resource "azurerm_app_service_custom_hostname_binding" "binding" {

 depends_on = [
     azurerm_dns_cname_record.function_domain_name,
     azurerm_dns_txt_record.function_domain_name_txt
 ]

  hostname            = "${local.cname}.${var.dns_zone}"
  app_service_name    = azurerm_function_app.functions.name
  resource_group_name = var.resource_group
}