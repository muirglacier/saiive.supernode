
# locals {
#   seed_name = "seed"
# }
# locals {
#     seed_name_comp = var.environment == "prod" ? local.seed_name :  "${var.environment}-${local.seed_name}"
# }

# resource "azurerm_dns_a_record" "seed" {
#   name                = local.seed_name_comp
#   zone_name           = var.dns_zone
#   resource_group_name = var.dns_zone_resource_group
#   ttl                 = 300
#   records             = local.dfi_node_ips
# }
