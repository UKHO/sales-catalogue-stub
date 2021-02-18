locals {
  basename = lower("m${var.servicename}${var.role}${var.deploy_environment}")
}

resource "azurerm_storage_account" "logstashStorage" {
  name                      = local.basename
  resource_group_name       = var.resource_group_name
  location                  = var.resource_group_location
  account_kind              = var.account_kind
  account_tier              = var.account_tier
  account_replication_type  = var.account_replication_type
  access_tier               = var.access_tier
  enable_https_traffic_only = var.enable_https_traffic_only

  lifecycle {
    ignore_changes = [
      tags
    ]
  }
}