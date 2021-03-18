
locals {
    nodes = concat(module.chain_scaleway_network_nodes.nodes, module.chain_azure_network_nodes.nodes)
}
