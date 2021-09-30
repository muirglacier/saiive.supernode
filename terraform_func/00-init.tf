terraform {
  backend "azurerm" {
    key = "supernode-light/super-nodes.terraform.tfstate"
  }
}
terraform {
  required_version = ">= 0.13"
}

terraform {
  required_providers {
    

    azurerm = {
      source = "hashicorp/azurerm"
    }

    
    local = {
      source = "hashicorp/local"
    }

    null = {
      source = "hashicorp/null"
    }

    template = {
      source = "hashicorp/template"
    }
  }
}

provider "azurerm" {
  features {}
}

# define the deployment location (az account list-locations --output table)
variable "location" {
  type    = string
  default = "westeurope"
}
variable "location2" {
  type    = string
  default = "northeurope"
}

# define the prefixed name
variable "prefix" {
  default = "defichain-supernode"
}

# define the environemnt name
variable "environment" {
  description = "deployment prefix"
}

variable "environment_tag" {
}


variable "dns_zone" {

}
variable "dns_zone_resource_group" {

}

variable "key_vault_id" {
  default = "/subscriptions/551ab192-148c-445b-ae4f-0d0107e6f5de/resourceGroups/defi-wallet-common/providers/Microsoft.KeyVault/vaults/defichain-wallet"
}


variable "app_version" {
  default = "20200722.2"
}

variable "tier" {
	default = "dynamic"
}

variable "size" {
	default = "Y1"
}

variable "always_on" {
  default = true
}