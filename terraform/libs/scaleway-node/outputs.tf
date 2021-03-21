output "nodes" {
    value = scaleway_instance_server.supernode.*.name
}

output "uptime" {
    value = flatten(module.uptime_robot.*.out)
}