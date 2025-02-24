
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
    source = "${var.function_app_file}"

}


data "azurerm_storage_account_sas" "sas" {
    connection_string = azurerm_storage_account.storage.primary_connection_string
    https_only = true
    start = "2019-01-01"
    expiry = "2025-12-31"
	
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
        tag = false
        filter = false
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
    storage_account_name       = azurerm_storage_account.storage.name
    storage_account_access_key = azurerm_storage_account.storage.primary_access_key
    version = "~3"
    https_only = true

    app_settings = {
        https_only = true
        FUNCTIONS_WORKER_RUNTIME = "dotnet"
        FUNCTION_APP_EDIT_MODE = "readonly"
        WEBSITE_ENABLE_SYNC_UPDATE_SITE = "false"
        HASH = base64encode(filesha256("${var.function_app_file}"))
        WEBSITE_RUN_FROM_PACKAGE = "https://${azurerm_storage_account.storage.name}.blob.core.windows.net/${azurerm_storage_container.deployments.name}/${azurerm_storage_blob.appcode.name}${data.azurerm_storage_account_sas.sas.sas}"
        WEBSITE_LOAD_USER_PROFILE = 1
        WEBSITE_VNET_ROUTE_ALL = 1

        OpenApi__Title = "tritonwallet.supernode"
        OpenApi__Description = "saiive.supernode API"
        OpenApi__TermsOfService = "https://static.tritonwallet.com/tos.html"

       // APPINSIGHTS_INSTRUMENTATIONKEY = var.environment == "prod" ? azurerm_application_insights.application_insights[0].instrumentation_key : ""

        LEGACY_API_URL = var.bitcore_url
        OCEAN_URL=  var.ocean_url
        DEFI_CHAIN_API_URL=  var.defichain_api
        COINGECKO_API_URL=  var.coingecko_url
        BLOCKCYHPER_API_KEY=  var.blockcypher_api
        LEGACY_BITCORE_URL = var.legacy_bitcoin_url

        EXPORT_QUEUE = var.export_q
        SENDGRID_API_KEY = var.sendgrid_api_key
    }

    connection_string {
        name = "ServiceBusConnection" 
        type = "Custom"
        value = var.servicebus_connection
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
