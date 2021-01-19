data "scaleway_image" "image" {
  architecture = var.server_arch
  name         = var.server_image
  most_recent = true
}

locals {
    environment = "${var.environment}-"
}

resource "scaleway_instance_ip" "node_ip" {
  count = var.node_count
}


data "template_file" "env" {
  template   = file("${path.root}/templates/env.tpl")

  vars = {
    network      = var.network
  }
}

resource "local_file" "env" {
  depends_on = [data.template_file.env]

  content  = data.template_file.env.rendered
  filename = "${path.root}/config/.env"
}


resource "scaleway_instance_server" "node" {
  count = var.node_count
  name = "${var.prefix}-${var.name}-${count.index}"
  depends_on = [scaleway_instance_ip.node_ip]

  image               = data.scaleway_image.image.id
  type                = var.server_type
  enable_dynamic_ip   = true
  ip_id = element(scaleway_instance_ip.node_ip.*.id, count.index)
  # initialization sequence
  cloud_init = var.cloud_init

  connection {
    host = self.public_ip
    user = var.username
    private_key = file(var.ssh_key_file)
  }

   provisioner "remote-exec" {
    inline = [
      "mkdir /opt/node"
    ]
  }

  provisioner "file" {
    source      = "${path.root}/config/"
    destination = "/opt/node"
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
  name = "${var.prefix}-${var.name}-${count.index}"
  resource_group_name = var.resource_group
  profile_name        = var.traffic_manager
  target              = element(scaleway_instance_server.node.*.public_ip, count.index)
  type                = "externalEndpoints"
  weight              = 100
}

