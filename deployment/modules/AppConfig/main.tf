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
      tags["COST_CENTRE"],
    ]
  }
}

resource "azurerm_management_lock" "appconflock" {
  name       = "${local.basename}lock"
  scope      = azurerm_app_configuration.appconf.id
  lock_level = "CanNotDelete"
  notes      = "Locked because we do not want to lose configuration values"
}
