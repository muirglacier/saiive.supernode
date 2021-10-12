
data "uptimerobot_account" "account" {}

locals {
   uptime_cname_prefix = var.environment == "prod" ? "" :  "${var.environment}-"
}

locals {
  uptime_robot_name = "saiive.live"
}
locals {
  
    env_name = var.environment == "prod" ? local.uptime_robot_name :  "${var.environment}-${local.uptime_robot_name}"
}

resource "azurerm_dns_cname_record" "status_uptimerobot" {
  name                = "${local.uptime_cname_prefix}status"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  record              = "status.uptimerobot.com"
}

data "uptimerobot_alert_contact" "default_alert_contact" {
  friendly_name = data.uptimerobot_account.account.email
}



resource "uptimerobot_monitor" "dfi_mainnet" {
  friendly_name = "DFI"
  type          = "http"
  url           = "https://${local.uptime_cname_prefix}/api/v1/mainnet/DFI/health"
  
  interval      = 60

  alert_contact {
    id = data.uptimerobot_alert_contact.default_alert_contact.id
  }
}

resource "uptimerobot_monitor" "dfi_testnet" {
  friendly_name = "DFI Testnet"
  type          = "http"
  url           = "https://${local.uptime_cname_prefix}/api/v1/testnet/DFI/health"
  
  interval      = 60

  alert_contact {
    id = data.uptimerobot_alert_contact.default_alert_contact.id
  }
}



resource "uptimerobot_monitor" "btc_mainnet" {
  friendly_name = "BTC"
  type          = "http"
  url           = "https://${local.uptime_cname_prefix}/api/v1/mainnet/BTC/health"
  
  interval      = 60

  alert_contact {
    id = data.uptimerobot_alert_contact.default_alert_contact.id
  }
}

resource "uptimerobot_monitor" "btc_testnet" {
  friendly_name = "BTC Testnet"
  type          = "http"
  url           = "https://${local.uptime_cname_prefix}/api/v1/testnet/BTC/health"
  
  interval      = 60

  alert_contact {
    id = data.uptimerobot_alert_contact.default_alert_contact.id
  }
}

resource "uptimerobot_monitor" "supernode" {
  friendly_name = "Supernode"
  type          = "http"
  url           = "https://${local.uptime_cname_prefix}/api/v1/health"
  
  interval      = 60

  alert_contact {
    id = data.uptimerobot_alert_contact.default_alert_contact.id
  }
}



resource "uptimerobot_status_page" "defichain_status_page" {
  friendly_name  = local.env_name
  custom_domain  = "${local.uptime_cname_prefix}status.saiive.live"
  monitors       = [  uptimerobot_monitor.dfi_mainnet.id,
                      uptimerobot_monitor.dfi_testnet.id,
                      uptimerobot_monitor.btc_mainnet.id,
                      uptimerobot_monitor.btc_testnet.id,
                      uptimerobot_monitor.supernode.id
                   ]
}

