

resource "random_string" "wallet_user" {
  count = var.node_count
  length = 16
  special = false
}

resource "random_password" "wallet_password" {
  count = var.node_count
  length = 16
  special = false
}

data "template_file" "env" {
  count = var.node_count
  template   = file("${path.root}/templates/env.tpl")

  vars = {
    network      = var.network
  }
}

data "template_file" "docker_compose" {
  count = var.node_count
  template   = file("${path.root}/templates/docker-compose.tpl")

  vars = {
    public_url      = "${local.node_name}-${count.index}.${var.dns_zone}"
    repo_user       = var.docker_user
    repo_pass       = var.docker_password
  }
}

data "template_file" "bitcore_mainnnet" {
  count = var.node_count
  template   = file("${path.root}/templates/bitcore.mainnet.config.tpl")

  vars = {
    wallet_user          = element(random_string.wallet_user.*.result, count.index)
    wallet_password      = element(random_password.wallet_password.*.result, count.index)
  }
}
data "template_file" "bitcore_testnet" {
  count = var.node_count
  template   = file("${path.root}/templates/bitcore.testnet.config.tpl")

  vars = {
    wallet_user          = element(random_string.wallet_user.*.result, count.index)
    wallet_password      = element(random_password.wallet_password.*.result, count.index)
  }
}
data "template_file" "defi_mainnnet" {
  count = var.node_count
  template   = file("${path.root}/templates/defi.mainnet.tpl")

  vars = {
    wallet_user          = element(random_string.wallet_user.*.result, count.index)
    wallet_password      = element(random_password.wallet_password.*.result, count.index)
  }
}
data "template_file" "defi_testnet" {
  count = var.node_count
  template   = file("${path.root}/templates/defi.testnet.tpl")

  vars = {
    wallet_user          = element(random_string.wallet_user.*.result, count.index)
    wallet_password      = element(random_password.wallet_password.*.result, count.index)
  }
}