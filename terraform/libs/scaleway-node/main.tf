
locals {
    node_name = "${var.prefix}-${var.environment}"
    short_name = "${var.uptime_prefix}-${var.environment}"
}

resource "scaleway_instance_ip" "node_ip_address" {
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

  inbound_rule {
    action = "accept"
    port   = "0"
    protocol = "ICMP"
  }
}

resource "scaleway_instance_volume" "main" {
    count = var.node_count
    type       = "b_ssd"
    name       = "${local.node_name}-${count.index}-vol-mainnet"
    size_in_gb = 400
}
resource "scaleway_instance_volume" "test" {
    count = var.node_count
    type       = "b_ssd"
    name       = "${local.node_name}-${count.index}-vol-testnet"
    size_in_gb = 800
}

resource "scaleway_instance_server" "supernode" {
  count = var.node_count
  name = "${local.node_name}-${count.index}"
  depends_on = [scaleway_instance_ip.node_ip_address]

  image               = var.server_image
  type                = var.server_type
  enable_dynamic_ip   = true
  ip_id = element(scaleway_instance_ip.node_ip_address.*.id, count.index)
  security_group_id = scaleway_instance_security_group.node.id

  cloud_init  = var.cloud_init

  root_volume {
    delete_on_termination = false
    size_in_gb = 120
  }

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

  
  additional_volume_ids = [ 
    element(scaleway_instance_volume.main.*.id, count.index),
    element(scaleway_instance_volume.test.*.id, count.index) 
  ]

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
  name                = "api.${element(scaleway_instance_server.supernode.*.name, count.index)}"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  records             = [element(scaleway_instance_server.supernode.*.public_ip, count.index)]
}

resource "azurerm_dns_a_record" "ping_omain_cname" {
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
resource "azurerm_dns_a_record" "explorer_custom_domain_cname" {
  count = var.node_count
  name                = "mainnet.${element(scaleway_instance_server.supernode.*.name, count.index)}"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  records             = [element(scaleway_instance_server.supernode.*.public_ip, count.index)]
}
resource "azurerm_dns_a_record" "explorer_testnet_custom_domain_cname" {
  count = var.node_count
  name                = "testnet.${element(scaleway_instance_server.supernode.*.name, count.index)}"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  records             = [element(scaleway_instance_server.supernode.*.public_ip, count.index)]
}
resource "azurerm_dns_a_record" "bitcore_testnet_custom_domain_cname" {
  count = var.node_count
  name                = "bitcore.${element(scaleway_instance_server.supernode.*.name, count.index)}"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  records             = [element(scaleway_instance_server.supernode.*.public_ip, count.index)]
}

module "uptime_robot" {
  source = "../uptime"

  count     = var.node_count
  dns_zone  = var.dns_zone
  node_name = element(scaleway_instance_server.supernode.*.name, count.index)
  node_name_short = local.short_name
  index = count.index

}