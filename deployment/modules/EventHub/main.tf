resource "azurerm_eventhub_namespace" "eventhub_namespace" {
  name                = "M-${var.servicename}-${var.role}-${var.deploy_environment}-ns"
  location            = var.resource_group_location
  resource_group_name = var.resource_group_name
  sku                 = var.sku
  capacity            = 2
  
  lifecycle {
    ignore_changes = [
      tags
    ]
  }
}

resource "azurerm_eventhub" "eventhub" {
  name                = "M-${var.servicename}-${var.role}-${var.deploy_environment}-eh"
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  resource_group_name = var.resource_group_name
  partition_count     = 2
  message_retention   = 7
}

resource "azurerm_eventhub_consumer_group" "logstash_consumer_group" {
  name                = "logstash"
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  eventhub_name       = azurerm_eventhub.eventhub.name
  resource_group_name = var.resource_group_name
}

resource "azurerm_eventhub_consumer_group" "logging_application_consumer_group" {
  name                = "loggingApplication"
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  eventhub_name       = azurerm_eventhub.eventhub.name
  resource_group_name = var.resource_group_name
}

resource "azurerm_eventhub_authorization_rule" "logstash_rule" {
  name                = "logstashAccessKey"
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  eventhub_name       = azurerm_eventhub.eventhub.name
  resource_group_name = var.resource_group_name
  listen              = true
  send                = false
  manage              = false
}

resource "azurerm_eventhub_authorization_rule" "api_rule" {
  name                = "logAccessKey"
  namespace_name      = azurerm_eventhub_namespace.eventhub_namespace.name
  eventhub_name       = azurerm_eventhub.eventhub.name
  resource_group_name = var.resource_group_name
  listen              = false
  send                = true
  manage              = false
}