
data "uptimerobot_account" "account" {}

locals {
  uptime_robot_name = "DeFiChain Wallet Supernode"
}
locals {
    env_name = var.environment == "prod" ? local.uptime_robot_name :  "${var.environment}-${local.uptime_robot_name}"
}

resource "uptimerobot_status_page" "defichain_status_page" {
  friendly_name  = local.env_name
  custom_domain  = "${local.uptime_cname_prefix}status.defichain-wallet.com"
  monitors       = concat(local.uptime_nodes)
}


resource "azurerm_dns_cname_record" "status_uptimerobot" {
  name                = "${local.uptime_cname_prefix}status"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  record              = "status.uptimerobot.com"
}