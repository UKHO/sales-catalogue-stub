locals {
  basename = "M-${var.servicename}-${var.role}-${var.deploy_environment}-RG"
}

resource "azurerm_resource_group" "main" {
  name     = local.basename
  location = var.mainlocation
  
  lifecycle {
    ignore_changes = [
      tags
    ]
  }
}
