locals {
    cname = var.environment == "prod" ? var.name :  "${var.environment}-${var.name}"
}

resource "azurerm_dns_cname_record" "frontdoor_cname" {
  name                = local.cname
  zone_name           = var.dns_zone
  resource_group_name = var.dns_zone_resource_group
  ttl                 = 300
  record              = "${var.prefix}-${var.environment}.azurefd.net"
}

resource "azurerm_frontdoor" "frontdoor" {
  name                                         ="${var.prefix}-${var.environment}"
  location                                     = "global"
  resource_group_name                          = var.resource_group
  enforce_backend_pools_certificate_name_check = true


  routing_rule {
    name               = "${var.prefix}-${var.environment}-http-https"
    accepted_protocols = ["Http"]
    patterns_to_match  = ["/*"]
    frontend_endpoints = ["${var.prefix}-${var.environment}-frontend"]
    
    redirect_configuration {
      redirect_protocol = "HttpsOnly"
      redirect_type       = "Found"
    }
  }

  routing_rule {
    name               = "${var.prefix}-${var.environment}-be-swagger"
    accepted_protocols = ["Https"]
    patterns_to_match  = ["/swagger/*"]
    frontend_endpoints = ["${var.prefix}-${var.environment}-frontend"]
    
    forwarding_configuration {
      forwarding_protocol = "HttpsOnly"
      backend_pool_name   = "${var.prefix}-${var.environment}-backend"
    }
  }

  routing_rule {
    name               = "${var.prefix}-${var.environment}-be-dfi-mainnet-route"
    accepted_protocols = ["Https"]
    patterns_to_match  = ["/api/v1//mainnet/DFI/*"]
    frontend_endpoints = ["${var.prefix}-${var.environment}-frontend"]
    
    forwarding_configuration {
      forwarding_protocol = "HttpsOnly"
      backend_pool_name   = "${var.prefix}-${var.environment}-backend-dfi-mainnet"
    }
  }

  routing_rule {
    name               = "${var.prefix}-${var.environment}-be-dfi-testnet-route"
    accepted_protocols = ["Https"]
    patterns_to_match  = ["/api/v1/testnet/DFI/*"]
    frontend_endpoints = ["${var.prefix}-${var.environment}-frontend"]
    
    forwarding_configuration {
      forwarding_protocol = "HttpsOnly"
      backend_pool_name   = "${var.prefix}-${var.environment}-backend-dfi-testnet"
    }
  }

  routing_rule {
    name               = "${var.prefix}-${var.environment}-be-default"
    accepted_protocols = ["Https"]
    patterns_to_match  = ["/*"]
    frontend_endpoints = ["${var.prefix}-${var.environment}-frontend"]
    
    forwarding_configuration {
      forwarding_protocol = "HttpsOnly"
      backend_pool_name   = "${var.prefix}-${var.environment}-backend"
    }
  }

  backend_pool_load_balancing {
    name = "${var.prefix}-${var.environment}-lb"
  }

  backend_pool_health_probe {
    name = "${var.prefix}-${var.environment}-health"
    path = "/v1/api/health"
    protocol = "Https"
  }

  backend_pool_health_probe {
    name = "${var.prefix}-${var.environment}-health-dfi-mainnet"
    path = "/v1/api/mainnet/DFI/health"
    protocol = "Https"
  }

  backend_pool_health_probe {
    name = "${var.prefix}-${var.environment}-health-dfi-testnet"
    path = "/v1/api/testnet/DFI/health"
    protocol = "Https"
  }

  backend_pool {
    name = "${var.prefix}-${var.environment}-backend"
    
    dynamic "backend" {
      for_each = var.scaleway_nodes
      content {
        address     = "${backend.value}.${var.dns_zone}"
        host_header = "${backend.value}.${var.dns_zone}"
        http_port   = 80
        https_port  = 443
      }
    } 
    load_balancing_name =  "${var.prefix}-${var.environment}-lb"
    health_probe_name   =  "${var.prefix}-${var.environment}-health"
  }
  
  backend_pool {
    name = "${var.prefix}-${var.environment}-backend-dfi-mainnet"
    
    dynamic "backend" {
      for_each = var.scaleway_nodes
      content {
        address     = "${backend.value}.${var.dns_zone}"
        host_header = "${backend.value}.${var.dns_zone}"
        http_port   = 80
        https_port  = 443
      }
    } 
    load_balancing_name =  "${var.prefix}-${var.environment}-lb"
    health_probe_name   =  "${var.prefix}-${var.environment}-health-dfi-mainnet"
  }

  backend_pool {
    name = "${var.prefix}-${var.environment}-backend-dfi-testnet"
    
    dynamic "backend" {
      for_each = var.scaleway_nodes
      content {
        address     = "${backend.value}.${var.dns_zone}"
        host_header = "${backend.value}.${var.dns_zone}"
        http_port   = 80
        https_port  = 443
      }
    } 
    load_balancing_name =  "${var.prefix}-${var.environment}-lb"
    health_probe_name   =  "${var.prefix}-${var.environment}-health-dfi-testnet"
  }

  frontend_endpoint {
    name                              = "${var.prefix}-${var.environment}-frontend"
    host_name                         = "${local.cname}.${var.dns_zone}"
    custom_https_provisioning_enabled = true
    custom_https_configuration {
      certificate_source                         = "FrontDoor"
    }
  }

  frontend_endpoint {
    name                              = "default"
    host_name                         = "${var.prefix}-${var.environment}.azurefd.net"
    custom_https_provisioning_enabled = false
  }
}
