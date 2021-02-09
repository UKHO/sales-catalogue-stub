locals {
  basename = "M-${var.servicename}-${var.role}-${var.deploy_environment}-eh"
}

resource "azurerm_eventhub_namespace" "eventhub_namespace" {
  name                = local.basename
  location            = var.resource_group_location
  resource_group_name = var.resource_group_name
  sku                 = var.sku
  capacity            = 2
}

resource "azurerm_eventhub" "eventhub" {
  name                = local.basename
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  resource_group_name = var.resource_group_name
  partition_count     = 2
  message_retention   = 7
}

resource "azurerm_eventhub_authorization_rule" "logstash_rule" {
  name                = "logstash"
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  eventhub_name       = azurerm_eventhub.eventhub.name
  resource_group_name = var.resource_group_name
  listen              = true
  send                = false
  manage              = false
}

resource "azurerm_eventhub_authorization_rule" "api_rule" {
  name                = "RootManageSharedAccessKey"
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  eventhub_name       = azurerm_eventhub.eventhub.name
  resource_group_name = var.resource_group_name
  listen              = false
  send                = true
  manage              = false
}