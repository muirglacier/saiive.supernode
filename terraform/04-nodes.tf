
locals {
    nodes = concat(module.chain_scw_network_nodes.nodes, module.chain_az_network_nodes.nodes, module.chain_btc_scw_network_nodes.nodes)
    uptime_nodes  = concat(module.chain_scw_network_nodes.uptime,  module.chain_az_network_nodes.uptime, module.chain_btc_scw_network_nodes.uptime)
}
