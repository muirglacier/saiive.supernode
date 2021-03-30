
locals {
    node_name = "${var.prefix}-${var.environment}"
    short_name = "${var.uptime_prefix}-${var.environment}"
}

resource "azurerm_virtual_network" "main" {
  name                = "${local.node_name}-network"
  address_space       = ["10.0.0.0/16"]
  location            = var.location
  resource_group_name = var.resource_group
}

resource "azurerm_subnet" "internal" {
  name                 = local.node_name
  resource_group_name  = var.resource_group
  virtual_network_name = azurerm_virtual_network.main.name
  address_prefixes     = ["10.0.2.0/24"]
}

resource "azurerm_network_security_group" "sg" {
    name                = "${local.node_name}-security-group"
    location            = var.location
    resource_group_name = var.resource_group

    security_rule {
        name                       = "SSH"
        priority                   = 1001
        direction                  = "Inbound"
        access                     = "Allow"
        protocol                   = "Tcp"
        source_port_range          = "*"
        destination_port_range     = "22"
        source_address_prefix      = "*"
        destination_address_prefix = "*"
    }

    security_rule {
        name                       = "DFI-Mainnet"
        priority                   = 100
        direction                  = "Inbound"
        access                     = "Allow"
        protocol                   = "Tcp"
        source_port_range          = "*"
        destination_port_range     = "8555"
        source_address_prefix      = "*"
        destination_address_prefix = "*"
    }
    security_rule {
        name                       = "DFI-Testnet"
        priority                   = 200
        direction                  = "Inbound"
        access                     = "Allow"
        protocol                   = "Tcp"
        source_port_range          = "*"
        destination_port_range     = "18555"
        source_address_prefix      = "*"
        destination_address_prefix = "*"
    }

    security_rule {
        name                       = "HTTPS"
        priority                   = 300
        direction                  = "Inbound"
        access                     = "Allow"
        protocol                   = "Tcp"
        source_port_range          = "*"
        destination_port_range     = "443"
        source_address_prefix      = "*"
        destination_address_prefix = "*"
    }
    security_rule {
        name                       = "HTTP"
        priority                   = 301
        direction                  = "Inbound"
        access                     = "Allow"
        protocol                   = "Tcp"
        source_port_range          = "*"
        destination_port_range     = "80"
        source_address_prefix      = "*"
        destination_address_prefix = "*"
    }

    security_rule {
        name                       = "PING"
        priority                   = 1000
        direction                  = "Inbound"
        access                     = "Allow"
        protocol                   = "ICMP"
        source_port_range          = "*"
        destination_port_range     = "*"
        source_address_prefix      = "*"
        destination_address_prefix = "*"
    }

    tags = {
        environment = var.environment
    }
}


resource "azurerm_public_ip" "node_public_ip" {
    count = var.node_count
    name                         = "${local.node_name}-${count.index}-public-ip"
    location                     = var.location
    resource_group_name          = var.resource_group
    allocation_method            = "Static"

    tags = {
        environment = var.environment
    }
}

resource "azurerm_network_interface" "nic" {
    count = var.node_count
    name                        = "${local.node_name}-${count.index}-nic"
    location                    = var.location
    resource_group_name         = var.resource_group

    ip_configuration {
        name                          = "${local.node_name}-${count.index}-nic-config"
        subnet_id                     = azurerm_subnet.internal.id
        private_ip_address_allocation = "Dynamic"
        public_ip_address_id          = element(azurerm_public_ip.node_public_ip.*.id, count.index)
    }

    tags = {
        environment = var.environment
    }
}

resource "azurerm_network_interface_security_group_association" "sg_link" {
    count = var.node_count
    network_interface_id      = element(azurerm_network_interface.nic.*.id, count.index)
    network_security_group_id = azurerm_network_security_group.sg.id
}

resource "azurerm_linux_virtual_machine" "supernode" {
    count = var.node_count
    name                  = "${local.node_name}-${count.index}"
    location              = var.location
    resource_group_name   = var.resource_group
    network_interface_ids = [element(azurerm_network_interface.nic.*.id, count.index)]
    size                  = var.server_type

    os_disk {
        name              = "${local.node_name}-${count.index}-disk"
        caching           = "ReadWrite"
        storage_account_type = "Standard_LRS"
        disk_size_gb = var.disk_size
    }

    source_image_reference {
        publisher = "Canonical"
        offer     = var.server_image
        sku       = var.server_version
        version   = "latest"
    }

    computer_name  = "${local.node_name}-${count.index}"
    admin_username = var.username
    disable_password_authentication = true

    admin_ssh_key {
        username       = var.username
        public_key     = var.ssh_pub_key
    }

    custom_data = base64encode(var.cloud_init)


    connection {
        host = self.public_ip_address
        user = var.username
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

    tags = {
        environment = var.environment
    }
}

resource "null_resource" "docker" {
  count = var.node_count
  triggers = {
    always_run = timestamp()
  }

  connection {
    host = element(azurerm_linux_virtual_machine.supernode.*.public_ip_address, count.index)
    user = var.username
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
      "sudo docker login ${var.docker_registry} --username ${var.docker_user} --password ${var.docker_password} &", 
      "sudo docker-compose -f ~/node/docker-compose.yml pull &",
      "sudo docker-compose -f ~/node/docker-compose.yml up -d ",
    ]
  }

}


resource "azurerm_dns_a_record" "custom_domain_cname" {
  count = var.node_count
  name                = "api.${element(azurerm_linux_virtual_machine.supernode.*.name, count.index)}"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  records             = [element(azurerm_linux_virtual_machine.supernode.*.public_ip_address, count.index)]
}

resource "azurerm_dns_a_record" "ping_domain_cname" {
  count = var.node_count
  name                = element(azurerm_linux_virtual_machine.supernode.*.name, count.index)
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  records             = [element(azurerm_linux_virtual_machine.supernode.*.public_ip_address, count.index)]
}
resource "azurerm_dns_a_record" "traefik_custom_domain_cname" {
  count = var.node_count
  name                = "traefik.${element(azurerm_linux_virtual_machine.supernode.*.name, count.index)}"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  records             = [element(azurerm_linux_virtual_machine.supernode.*.public_ip_address, count.index)]
}
resource "azurerm_dns_a_record" "explorer_custom_domain_cname" {
  count = var.node_count
  name                = "mainnet.${element(azurerm_linux_virtual_machine.supernode.*.name, count.index)}"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  records             = [element(azurerm_linux_virtual_machine.supernode.*.public_ip_address, count.index)]
}
resource "azurerm_dns_a_record" "explorer_testnet_custom_domain_cname" {
  count = var.node_count
  name                = "testnet.${element(azurerm_linux_virtual_machine.supernode.*.name, count.index)}"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  records             = [element(azurerm_linux_virtual_machine.supernode.*.public_ip_address, count.index)]
}
resource "azurerm_dns_a_record" "bitcore_testnet_custom_domain_cname" {
  count = var.node_count
  name                = "bitcore.${element(azurerm_linux_virtual_machine.supernode.*.name, count.index)}"
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  records             = [element(azurerm_linux_virtual_machine.supernode.*.public_ip_address, count.index)]
}

module "uptime_robot" {
  source = "../uptime"

  count     = var.node_count
  dns_zone  = var.dns_zone
  node_name = element(azurerm_linux_virtual_machine.supernode.*.name, count.index)  
  node_name_short = local.short_name
  index = count.index

}