output "nodes" {
    value = scaleway_instance_server.supernode.*.name
}