output "nodes" {
    value = scaleway_instance_server.node.*.name
}