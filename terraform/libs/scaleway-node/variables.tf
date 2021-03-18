
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