output "nodes" {
    value = scaleway_instance_server.supernode.*.name
}


output "node_ips" {
    value = scaleway_instance_server.supernode.*.public_ip
}


output "uptime" {
    value = flatten(module.uptime_robot.*.out)
}