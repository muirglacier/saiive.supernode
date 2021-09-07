

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

data "template_file" "docker_compose" {
  count = var.node_count
  template   = file("${path.root}/templates/${var.node_type}/docker-compose.tpl")

  vars = {
    public_url                  = "${local.node_name}-${count.index}.${var.dns_zone}"
    repo_user                   = var.docker_user
    repo_pass                   = var.docker_password
    application_insights_ikey   = var.application_insights_ikey
    blockcypher_api_key         = var.blockcypher_api_key
    
    machine_name                = local.node_name
    volume_testnet              = "node_data_testnet"
    volume_mainnet              = "node_data_mainnet"
    volume_db                   = "db_data"
    volume_type                 = "volume"

    data_dir                    = var.docker_node_data_dir
    config_dir                  = var.docker_node_config_dir
    db_dir                      = var.docker_db_dir
    node_prefix                 = var.node_prefix
  }
}


data "template_file" "bitcore_all" {
  count = var.node_count
  template   = file("${path.root}/templates/${var.node_type}/bitcore.all.config.tpl")

  vars = {
    wallet_user          = element(random_string.wallet_user.*.result, count.index)
    wallet_password      = element(random_password.wallet_password.*.result, count.index)
  }
}

data "template_file" "node_mainnnet" {
  count = var.node_count
  template   = file("${path.root}/templates/${var.node_type}/node.mainnet.tpl")

  vars = {
    wallet_user          = element(random_string.wallet_user.*.result, count.index)
    wallet_password      = element(random_password.wallet_password.*.result, count.index)
    chain_dir            = var.docker_node_data_dir
  }
}
data "template_file" "node_testnet" {
  count = var.node_count
  template   = file("${path.root}/templates/${var.node_type}/node.testnet.tpl")

  vars = {
    wallet_user          = element(random_string.wallet_user.*.result, count.index)
    wallet_password      = element(random_password.wallet_password.*.result, count.index)
    chain_dir            = var.docker_node_data_dir
  }
}