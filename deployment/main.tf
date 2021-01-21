terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=2.41.0"
    }
  }
  required_version = ">= 0.14"
  backend "azurerm" {
    container_name                = "tfstate"
    key                           = "terraform.tfstate"    
  }
}
# A local value assigns a name to an expression, allowing it to be used multiple times within a module without repeating it.

locals {
  nsb-role                        ="nsb"
  api-role                        ="api"
  web-role                        ="web"
  appconfig-role                  ="appconfig"
}

