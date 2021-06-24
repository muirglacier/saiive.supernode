terraform {
  backend "azurerm" {
    key = "defi/super-nodes.terraform.tfstate"
  }
}
terraform {
  required_version = ">= 0.13"
}


terraform {
  required_providers {
    uptimerobot = {
      source  = "louy/uptimerobot"
      version = "0.5.1"
    }

    azurerm = {
      source = "hashicorp/azurerm"
    }

    scaleway = {
      source = "scaleway/scaleway"
      version = "1.17.2"
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

provider "scaleway" {
  zone   = "nl-ams-1"
  region = "nl-ams"
}

provider "uptimerobot" {
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

variable "scaleway_node_count" {
  default = 2
  type = number
}
variable "scaleway_btc_node_count" {
  default = 0
  type = number
}
variable "azure_node_count" {
  default = 2
  type = number
}
variable "scaleway_server_type" {
  default = "DEV1-S"
}

variable "analytics_key" {
  
}
variable "analytics_id" {
  
}