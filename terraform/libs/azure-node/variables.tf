
variable "environment" {
}

variable "location" {
}

variable "cloud_init" {
  
}

variable "prefix" {
}

variable "server_image" {
  default = "UbuntuServer"
}
variable "server_version" {
  default = "18.04-LTS"
}

variable "server_type" {
  default = "Standard_DS1_v2"
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

variable "application_insights_ikey" {
  
}