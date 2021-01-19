data "scaleway_image" "image" {
  architecture = var.server_arch
  name         = var.server_image
  most_recent = true
}

locals {
    node_name = "${var.prefix}-${var.environment}"
}

resource "scaleway_instance_ip" "node_ip" {
  count = var.node_count
}

resource "scaleway_instance_security_group" "node" {
  inbound_default_policy  = "drop"
  outbound_default_policy = "accept"

  inbound_rule {
    action = "accept"
    port   = "22"
  }

  inbound_rule {
    action = "accept"
    port   = "80"
  }

  inbound_rule {
    action = "accept"
    port   = "443"
  }
}

resource "scaleway_instance_server" "node" {
  count = var.node_count
  name = "${local.node_name}-${count.index}"
  depends_on = [scaleway_instance_ip.node_ip]

  image               = data.scaleway_image.image.id
  type                = var.server_type
  enable_dynamic_ip   = true
  ip_id = element(scaleway_instance_ip.node_ip.*.id, count.index)
  security_group_id = scaleway_instance_security_group.node.id
  # initialization sequence
  cloud_init = var.cloud_init

  connection {
    host = self.public_ip
    user = var.username
    private_key = var.ssh_key
  }

   provisioner "remote-exec" {
    inline = [
      "mkdir /opt/node"
    ]
  }
  
  provisioner "file" {
    content = element(data.template_file.env.*.rendered, count.index)
    destination = "/opt/node/.env"
  }

  provisioner "file" {
    content = element(data.template_file.docker_compose.*.rendered, count.index)
    destination = "/opt/node/docker-compose.yml"
  }

  provisioner "file" {
    content = element(data.template_file.bitcore_mainnnet.*.rendered, count.index)
    destination = "/opt/node/bitcore.mainnet.config.json"
  }

  provisioner "file" {
    content = element(data.template_file.bitcore_testnet.*.rendered, count.index)
    destination = "/opt/node/bitcore.testnet.config.json"
  }
  provisioner "file" {
    content = element(data.template_file.defi_mainnnet.*.rendered, count.index)
    destination = "/opt/node/defi.mainnet.conf"
  }
  provisioner "file" {
    content = element(data.template_file.defi_testnet.*.rendered, count.index)
    destination = "/opt/node/defi.testnet.conf"
  }

  provisioner "remote-exec" {
    inline = [
      "tail -f /var/log/cloud-init-output.log &",
      "while [ ! -f /var/lib/cloud/instance/boot-finished ]; do sleep 10; done;",
    ]
  }
}

resource "azurerm_dns_a_record" "custom_domain_cname" {
  count = var.node_count
  name                = element(scaleway_instance_server.node.*.name, count.index)
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  records             = [element(scaleway_instance_server.node.*.public_ip, count.index)]
}
resource "azurerm_dns_a_record" "traefik_custom_domain_cname" {
  count = var.node_count
  name                = "traefik.${element(scaleway_instance_server.node.*.name, count.index)}"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  records             = [element(scaleway_instance_server.node.*.public_ip, count.index)]
}

data "uptimerobot_account" "account" {}

data "uptimerobot_alert_contact" "default_alert_contact" {
  friendly_name = data.uptimerobot_account.account.email
}

resource "uptimerobot_monitor" "main" {
  count = var.node_count
  friendly_name = element(scaleway_instance_server.node.*.name, count.index)
  type          = "http"
  url           = "https://${element(scaleway_instance_server.node.*.name, count.index)}.${var.dns_zone}/v1/api/health"
  # pro allows 60 seconds
  interval      = 300
  sub_type      = "https"

  alert_contact {
    id = data.uptimerobot_alert_contact.default_alert_contact.id
  }
}

resource "azurerm_traffic_manager_endpoint" "endpoint" {
  count = var.node_count
  name = "${var.prefix}-${count.index}"
  resource_group_name = var.resource_group
  profile_name        = var.traffic_manager
  target              = element(scaleway_instance_server.node.*.public_ip, count.index)
  type                = "externalEndpoints"
  weight              = 100
}

