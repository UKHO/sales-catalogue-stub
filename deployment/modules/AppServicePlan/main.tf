locals {
  basename = "M-${var.servicename}-${var.role}-${var.deploy_environment}-appserviceplan"
}

resource "azurerm_app_service_plan" "apimain" {
  name                = local.basename
  location            = var.resource_group_location
  resource_group_name = var.resource_group_name
  kind                = var.kind
  reserved            = var.reserved

  sku {
    tier = var.sku_tier
    size = var.sku_size
  }

  lifecycle {
    ignore_changes = [
      tags
    ]
  }
}
