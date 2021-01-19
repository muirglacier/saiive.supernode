
variable "name" {
}
variable "environment" {
}

variable "cloud_init" {
}

variable "prefix" {
}


variable "server_arch" {
  default = "x86_64"
}

variable "server_image" {
  default = "Debian Buster"
}

variable "server_type" {
  default = "DEV1-S"
}

variable "ssh_key_file" {

}

variable "username" {
  default = "root"
}

variable "dns_zone" {
  
}
variable "dns_zone_resource_group" {

}

variable "traffic_manager" {
  
}
variable "resource_group" {
	
}

variable "node_count" {
  
}

variable "network" {
  
}
