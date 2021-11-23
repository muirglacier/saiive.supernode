
variable "environment" {
}

variable "location" {
}

variable "cloud_init" {
  
}

variable "prefix" {
}
variable "node_type" {
  default = "dfi"
}
variable "node_prefix" {
  default = "defi"
}
variable "node_chain" {
  default = "DFI"
}

variable "docker_node_config_dir" {
  default = "/data"
}
variable "docker_node_data_dir" {
  default = "/data"
}

variable "docker_db_dir" {
  default = "/data/db"
}

variable "uptime_prefix" {
}

variable "server_image" {
  default = "UbuntuServer"
}
variable "server_version" {
  default = "18.04-LTS"
}

variable "server_type" {
  default = "Standard_B2s"
}
variable "disk_size" {
  default = 200
}

variable "ssh_key" {

}

variable "ssh_pub_key" {

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

variable "blockcypher_api_key" {
  
}