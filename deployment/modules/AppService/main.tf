locals {
  basename = "M-${var.servicename}-${var.role}-${var.deploy_environment}-appservice"
}

data "azurerm_virtual_network" "vnet" {
  name                = "${var.spoke_vnet_name}${var.deploy_environment}-vnet"
  resource_group_name = var.spoke_rg
}

data "azurerm_subnet" "subnet" {
  name                = var.spoke_subnet_name
  virtual_network_name = data.azurerm_virtual_network.vnet.name
  resource_group_name = var.spoke_rg
}

resource "azurerm_app_service" "main" {
  name                = local.basename
  location            = var.resource_group_location
  resource_group_name = var.resource_group_name
  app_service_plan_id = var.app_service_plan_id
  https_only          = var.https_only

  site_config {
    dotnet_framework_version = var.dotnet_framework_version
    always_on                = var.always_on
    dynamic "ip_restriction" {
      for_each = [data.azurerm_subnet.subnet]
      content {
        virtual_network_subnet_id = ip_restriction.value.id
      }
    }
    dynamic "ip_restriction" {
      for_each = ["${var.eng_outgoing_ip}/32", "${var.ukho_main_outgoing_ip}/32"] 
      content {
        ip_address  = ip_restriction.value
      }
    }
  }

  app_settings = {
    "ENVIRONMENT"                                   = var.env_context
    "SCS_KEY_VAULT_ADDRESS"                         = var.key_vault_address
    "SCS_AZURE_APP_CONFIGURATION_CONNECTION_STRING" = var.appconfiguration
    "WEBSITE_RUN_FROM_PACKAGE"                      = var.run_from_package
    "WEBSITE_DNS_SERVER"                            = var.website_dns_server
  }

  identity {
    type = var.identity_type
  }

  lifecycle {
    ignore_changes = [
      app_settings["WEBSITE_RUN_FROM_PACKAGE"],
      tags
    ]
  }
}

resource "azurerm_key_vault_access_policy" "kvpolicy" {
  key_vault_id = var.key_vault_id
  tenant_id = azurerm_app_service.main.identity.0.tenant_id
  object_id = azurerm_app_service.main.identity.0.principal_id
  secret_permissions = [
    "get",
    "list",
  ]
}

resource "azurerm_app_service_virtual_network_swift_connection" "main" {
  app_service_id = azurerm_app_service.main.id
  subnet_id      = data.azurerm_subnet.subnet.id
}
