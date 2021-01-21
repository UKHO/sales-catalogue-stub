terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=2.41.0"
    }
  }
  required_version = ">= 0.14"
  backend "azurerm" {
    container_name = "tfstate"
    key            = "terraform.tfstate"
  }
}

data "azurerm_client_config" "current" {}
# A local value assigns a name to an expression, allowing it to be used multiple times within a module without repeating it.
locals {
  api_role       = "api"
  appconfig_role = "appconfig"
  keyvault_role  = "keyvault"
  tenant_id      = data.azurerm_client_config.current.tenant_id

}

