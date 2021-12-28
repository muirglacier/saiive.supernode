# define the prefixed name
variable "prefix" {
	description = "deployment prefix"
}

variable "location" {
	
}

# define the environemnt name
variable "environment" {
	description = "deployment prefix"
}

variable "environment_tag" {

}

variable "resource_group" {
	
}

variable "function_app_file" {

}

variable "app_version" {
	
}

variable "tier" {
	default = "dynamic"
}

variable "size" {
	default = "Y1"
}

variable "dns_name" {
	default = "supernode-push"
}
variable "dns_zone" {
  
}
variable "dns_zone_resource_group" {

}


variable "always_on" {
	default = true
}

variable "cosmos_connection_string" {
	
}

variable "cosmos_db_name" {
	
}

variable "cosmos_table_name" {
	
}

variable "bitcore_url" {
	
}
variable "ocean_url" {
	
}
variable "defichain_api" {
	
}
variable "coingecko_url" {
	
}

variable "blockcypher_api" {
	
}

variable "legacy_bitcoin_url" {

}

variable "dobby_url" {
	
}
variable "webhook_url" {
	
}

variable "notification_hub_key" {
	
}
variable "legacy_api_url" {

}