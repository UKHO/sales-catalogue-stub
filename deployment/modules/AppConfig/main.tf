locals {
  basename = "${var.servicename}-${var.role}-${var.deploy_environment}"
}

resource "azurerm_resource_group" "main" {
  name = "M-${local.basename}-RG"
  location = var.mainlocation
  # tags = var.tags

  # lifecycle {
  #   ignore_changes = [
  #     tags["COST_CENTRE"],
  #   ]
  # }
}

# resource "azurerm_app_configuration" "appconf" {
#   lifecycle {
#      prevent_destroy = true
#      ignore_changes = [
#       tags["COST_CENTRE"],
#     ]
#   }
#   name                = local.basename
#   location            = azurerm_resource_group.main.location
#   resource_group_name = azurerm_resource_group.main.name

#   sku        = "standard"

#   tags = var.tags
# }