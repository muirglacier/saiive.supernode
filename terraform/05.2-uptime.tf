
data "uptimerobot_account" "account" {}

resource "uptimerobot_status_page" "defichain_status_page" {
  friendly_name  = "${var.prefix} DeFiChain Wallet Supernode"
  custom_domain  = "${local.uptime_cname_prefix}stats.defichain-wallet.com"
  monitors       = concat(module.chain_scaleway_network_nodes.uptime,  module.chain_azure_network_nodes.uptime)
}


resource "azurerm_dns_cname_record" "stats_uptimerobot" {
  name                = "${local.uptime_cname_prefix}stats"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  record              = "stats.uptimerobot.com"
}