output "nodes" {
    value = azurerm_linux_virtual_machine.supernode.*.name
}
output "uptime" {
    value = concat(uptimerobot_monitor.dfi_mainnet.*.id, uptimerobot_monitor.dfi_testnet.*.id)
}