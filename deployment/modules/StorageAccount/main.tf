locals {
  basename = lower("m${var.servicename}${var.role}${var.deploy_environment}")
}

resource "azurerm_storage_account" "logstashStorage" {
  name                      = local.basename
  resource_group_name       = var.resource_group_name
  location                  = var.resource_group_location
  account_kind              = "StorageV2"
  account_tier              = "Standard"
  account_replication_type  = "LRS"
  access_tier               = "Hot"
  enable_https_traffic_only = true
}