data "scaleway_image" "image" {
  architecture = var.server_arch
  name         = var.server_image
  most_recent = true
}

locals {
    node_name = "${var.prefix}-scw-${var.environment}"
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
  
  inbound_rule {
    action = "accept"
    port   = "18555"
  }

  inbound_rule {
    action = "accept"
    port   = "8555"
  }
}

resource "scaleway_instance_server" "supernode" {
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
    user = "root"
    private_key = var.ssh_key
  }

   provisioner "remote-exec" {
    inline = [
      "mkdir ~/node",
      "mkdir ~/node/mainnet",
      "mkdir ~/node/testnet"
    ]
  }

  provisioner "file" {
    content = element(data.template_file.docker_compose.*.rendered, count.index)
    destination = "~/node/docker-compose.yml"
  }

  provisioner "file" {
    content = element(data.template_file.bitcore_mainnnet.*.rendered, count.index)
    destination = "~/node/mainnet/bitcore.mainnet.config.json"
  }
  
  provisioner "file" {
    content = element(data.template_file.bitcore_all.*.rendered, count.index)
    destination = "~/node/bitcore.all.config.json"
  }

  provisioner "file" {
    content = element(data.template_file.bitcore_testnet.*.rendered, count.index)
    destination = "~/node/testnet/bitcore.testnet.config.json"
  }
  provisioner "file" {
    content = element(data.template_file.defi_mainnnet.*.rendered, count.index)
    destination = "~/node/mainnet/defi.mainnet.conf"
  }
  provisioner "file" {
    content = element(data.template_file.defi_testnet.*.rendered, count.index)
    destination = "~/node/testnet/defi.testnet.conf"
  }

  provisioner "remote-exec" {
    inline = [
      "tail -f /var/log/cloud-init-output.log &",
      "while [ ! -f /var/lib/cloud/instance/boot-finished ]; do sleep 10; done;",
    ]
  }
}

resource "null_resource" "docker" {
  count = var.node_count
  triggers = {
    always_run = timestamp()
  }

  connection {
    host = element(scaleway_instance_server.supernode.*.public_ip, count.index)
    user = "root"
    private_key = var.ssh_key
  }
  
  provisioner "file" {
    content = element(data.template_file.docker_compose.*.rendered, count.index)
    destination = "~/node/docker-compose.yml"
  }

  provisioner "file" {
    content = element(data.template_file.bitcore_mainnnet.*.rendered, count.index)
    destination = "~/node/mainnet/bitcore.mainnet.config.json"
  }
  
  provisioner "file" {
    content = element(data.template_file.bitcore_all.*.rendered, count.index)
    destination = "~/node/bitcore.all.config.json"
  }

  provisioner "file" {
    content = element(data.template_file.bitcore_testnet.*.rendered, count.index)
    destination = "~/node/testnet/bitcore.testnet.config.json"
  }
  provisioner "file" {
    content = element(data.template_file.defi_mainnnet.*.rendered, count.index)
    destination = "~/node/mainnet/defi.mainnet.conf"
  }
  provisioner "file" {
    content = element(data.template_file.defi_testnet.*.rendered, count.index)
    destination = "~/node/testnet/defi.testnet.conf"
  }
  provisioner "remote-exec" {
    inline = [
      "docker login ${var.docker_registry} --username ${var.docker_user} --password ${var.docker_password} &", 
      "docker-compose -f ~/node/docker-compose.yml pull &",
      "docker-compose -f ~/node/docker-compose.yml up -d ",
    ]
  }
}

resource "azurerm_dns_a_record" "custom_domain_cname" {
  count = var.node_count
  name                = element(scaleway_instance_server.supernode.*.name, count.index)
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  records             = [element(scaleway_instance_server.supernode.*.public_ip, count.index)]
}
resource "azurerm_dns_a_record" "traefik_custom_domain_cname" {
  count = var.node_count
  name                = "traefik.${element(scaleway_instance_server.supernode.*.name, count.index)}"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  records             = [element(scaleway_instance_server.supernode.*.public_ip, count.index)]
}

data "uptimerobot_account" "account" {}

data "uptimerobot_alert_contact" "default_alert_contact" {
  friendly_name = data.uptimerobot_account.account.email
}

resource "uptimerobot_monitor" "dfi_mainnet" {

  count = var.node_count
  friendly_name = "${element(scaleway_instance_server.supernode.*.name, count.index)}-mainnet"
  type          = "http"
  url           = "https://${element(scaleway_instance_server.supernode.*.name, count.index)}.${var.dns_zone}/api/v1/mainnet/DFI/health"
  
  interval      = 60

  alert_contact {
    id = data.uptimerobot_alert_contact.default_alert_contact.id
  }
}

resource "uptimerobot_monitor" "dfi_testnet" {
  count = var.node_count
  friendly_name = "${element(scaleway_instance_server.supernode.*.name, count.index)}-testnet"
  type          = "http"
  url           = "https://${element(scaleway_instance_server.supernode.*.name, count.index)}.${var.dns_zone}/api/v1/testnet/DFI/health"
  
  interval      = 60

  alert_contact {
    id = data.uptimerobot_alert_contact.default_alert_contact.id
  }
}

resource "uptimerobot_monitor" "vm" {
  count = var.node_count
  friendly_name = element(scaleway_instance_server.supernode.*.name, count.index)
  type          = "http"
  url           = "https://${element(scaleway_instance_server.supernode.*.name, count.index)}.${var.dns_zone}/api/v1/health"
  
  interval      = 60

  alert_contact {
    id = data.uptimerobot_alert_contact.default_alert_contact.id
  }
}