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


variable "tier" {
	default = "dynamic"
}

variable "size" {
	default = "Y1"
}

variable "dns_name" {
	default = "supernode"
}
variable "dns_zone" {
  
}
variable "dns_zone_resource_group" {

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

variable "always_on" {
	default = true
}

variable "servicebus_connection" {
	
}

variable "export_q" {

}

variable "sendgrid_api_key" {
	
}