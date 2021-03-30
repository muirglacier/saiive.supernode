
data "uptimerobot_account" "account" {}

data "uptimerobot_alert_contact" "default_alert_contact" {
  friendly_name = data.uptimerobot_account.account.email
}

resource "uptimerobot_monitor" "dfi_mainnet" {
  friendly_name = "${var.node_name_short}-${var.index}-mainnet"
  type          = "http"
  url           = "https://api.${var.node_name}.${var.dns_zone}/api/v1/mainnet/DFI/health"
  
  interval      = 60

  alert_contact {
    id = data.uptimerobot_alert_contact.default_alert_contact.id
  }
}

resource "uptimerobot_monitor" "dfi_testnet" {
  friendly_name = "${var.node_name_short}-${var.index}-testnet"
  type          = "http"
  url           = "https://api.${var.node_name}.${var.dns_zone}/api/v1/testnet/DFI/health"
  
  interval      = 60

  alert_contact {
    id = data.uptimerobot_alert_contact.default_alert_contact.id
  }
}

resource "uptimerobot_monitor" "vm" {
  friendly_name = "${var.node_name_short}-${var.index}-supernode"
  type          = "http"
  url           = "https://api.${var.node_name}.${var.dns_zone}/api/v1/health"
  
  interval      = 60

  alert_contact {
    id = data.uptimerobot_alert_contact.default_alert_contact.id
  }
}

resource "uptimerobot_monitor" "ping" {
  friendly_name = "${var.node_name_short}-${var.index}"
  type          = "ping"
  url           = "${var.node_name}.${var.dns_zone}"
  
  interval      = 60

  alert_contact {
    id = data.uptimerobot_alert_contact.default_alert_contact.id
  }
}

output "out" {
    value = concat(uptimerobot_monitor.dfi_mainnet.*.id, uptimerobot_monitor.dfi_testnet.*.id, uptimerobot_monitor.vm.*.id, uptimerobot_monitor.ping.*.id)
}