locals {
  basename = lower("m${var.servicename}${var.role}${var.deploy_environment}ac")
}
resource "azurerm_app_configuration" "appconf" {
  name                = local.basename
  location            = var.resource_group_location
  resource_group_name = var.resource_group_name
  sku                 = var.sku

   lifecycle {
     prevent_destroy = true
     ignore_changes = [
      tags
    ]
  }
}
