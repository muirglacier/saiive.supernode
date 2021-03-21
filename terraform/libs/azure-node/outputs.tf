output "nodes" {
    value = azurerm_linux_virtual_machine.supernode.*.name
}

output "uptime" {
    value = flatten(module.uptime_robot.*.out)
}