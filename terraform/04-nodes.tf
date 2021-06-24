
locals {
    nodes = concat(module.chain_scw_network_nodes.nodes, module.chain_az_network_nodes.nodes, module.chain_btc_scw_network_nodes.nodes)
    uptime_nodes  = concat(module.chain_scw_network_nodes.uptime,  module.chain_az_network_nodes.uptime, module.chain_btc_scw_network_nodes.uptime)

    dfi_node_ips = concat(module.chain_scw_network_nodes.node_ips, module.chain_az_network_nodes.node_ips)
    

    bitcoin_nodes = concat(module.chain_btc_scw_network_nodes.nodes)
    dfi_nodes = concat(module.chain_scw_network_nodes.nodes, module.chain_az_network_nodes.nodes)
}
