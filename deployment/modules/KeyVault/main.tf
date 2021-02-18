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
     prevent_destroy = true
     ignore_changes = [
      tags
    ]
  }
}

resource "azurerm_key_vault_access_policy" "kvpolicygroup" {
  key_vault_id       = azurerm_key_vault.keyvault.id
  tenant_id          = azurerm_key_vault.keyvault.tenant_id
  object_id          = var.kv_access_policy_group_object_id
  secret_permissions = [
    "get",
    "list",
    "set",
  ]
}

resource "azurerm_key_vault_access_policy" "kvpolicypipeline" {
  key_vault_id       = azurerm_key_vault.keyvault.id
  tenant_id          = azurerm_key_vault.keyvault.tenant_id
  object_id          = var.kv_access_policy_pipeline_object_id
  secret_permissions = [
    "get",
    "list",
  ]
}
