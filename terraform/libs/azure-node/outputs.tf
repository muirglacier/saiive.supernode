output "nodes" {
    value = azurerm_linux_virtual_machine.supernode.*.name
}

output "node_ips" {
    value = azurerm_linux_virtual_machine.supernode.*.public_ip_address
}


output "uptime" {
    value = flatten(module.uptime_robot.*.out)
}