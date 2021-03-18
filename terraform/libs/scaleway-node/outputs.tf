output "nodes" {
    value = scaleway_instance_server.supernode.*.name
}

output "uptime" {
    value = concat(uptimerobot_monitor.dfi_mainnet.*.id, uptimerobot_monitor.dfi_testnet.*.id)
}