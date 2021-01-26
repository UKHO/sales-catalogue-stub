locals {
  basename = lower("m${var.servicename}${var.role}${var.deploy_environment}kv")
}

resource "azurerm_key_vault" "keyvault" {
  name                = local.basename
  location            = var.resource_group_location
  resource_group_name = var.resource_group_name
  sku_name            = var.sku_name
  tenant_id           = var.tenant_id

  lifecycle {
    ignore_changes = [
      tags
    ]
  }
}
