
variable "environment" {
}

variable "cloud_init" {
}

variable "prefix" {
}

variable "node_type" {
  default = "dfi"
}
variable "node_chain" {
  default = "DFI"
}
variable "node_prefix" {
  default = "defi"
}
variable "docker_node_config_dir" {
  default = "/data"
}
variable "docker_node_data_dir" {
  default = "/data"
}

variable "uptime_prefix" {
}


variable "server_arch" {
  default = "x86_64"
}

variable "server_image" {
  default = "ubuntu_focal"
}

variable "server_type" {
  default = "DEV1-S"
}

variable "ssh_key" {

}

variable "username" {

}

variable "dns_zone" {
  
}
variable "dns_zone_resource_group" {

}

variable "resource_group" {
	
}

variable "node_count" {
  
}

variable "public_endpoint" {
  
}


variable "docker_user" {
  
}
variable "docker_password" {
  
}
variable "docker_registry" {
  
}

variable "application_insights_ikey" {
  
}