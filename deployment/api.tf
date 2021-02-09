
module "key_vault_rg" {
  source             = "./modules/ResourceGroup"
  deploy_environment = var.DEPLOY_ENVIRONMENT
  servicename        = var.SERVICENAME
  role               = local.keyvault_role
  mainlocation       = var.MAIN_LOCATION
}

module "key_vault" {
  source                  = "./modules/KeyVault"
  deploy_environment      = var.DEPLOY_ENVIRONMENT
  servicename             = var.SERVICENAME
  role                    = local.api_role
  tenant_id               = local.tenant_id
  resource_group_location = module.key_vault_rg.location
  resource_group_name     = module.key_vault_rg.name
  sku_name                = "standard"
}

module "app_config_rg" {
  source             = "./modules/ResourceGroup"
  deploy_environment = var.DEPLOY_ENVIRONMENT
  servicename        = var.SERVICENAME
  role               = local.appconfig_role
  mainlocation       = var.MAIN_LOCATION
}

module "app_config" {
  source                  = "./modules/AppConfig"
  deploy_environment      = var.DEPLOY_ENVIRONMENT
  servicename             = var.SERVICENAME
  role                    = local.api_role
  resource_group_location = module.app_config_rg.location
  resource_group_name     = module.app_config_rg.name
  sku                     = "standard"
}

module "api_rg" {
  source             = "./modules/ResourceGroup"
  deploy_environment = var.DEPLOY_ENVIRONMENT
  servicename        = var.SERVICENAME
  role               = local.api_role
  mainlocation       = var.MAIN_LOCATION
}

module "app_service_plan" {
  source                  = "./modules/AppServicePlan"
  deploy_environment      = var.DEPLOY_ENVIRONMENT
  servicename             = var.SERVICENAME
  role                    = local.api_role
  sku_tier                = "Standard"
  sku_size                = "S1"
  kind                    = "app"
  reserved                = false
  resource_group_name     = module.api_rg.name
  resource_group_location = module.api_rg.location

}

module "app_service" {
  source                   = "./modules/AppService"
  deploy_environment       = var.DEPLOY_ENVIRONMENT
  appconfiguration         = module.app_config.this_connection_string
  key_vault_address        = module.key_vault.vault_uri
  key_vault_id             = module.key_vault.id
  servicename              = var.SERVICENAME
  role                     = local.api_role
  env_context              = var.ENV_CONTEXT
  website_dns_server       = var.WEBSITE_DNS_SERVER
  resource_group_name      = module.api_rg.name
  resource_group_location  = module.api_rg.location
  resource_group_id        = module.api_rg.id
  app_service_plan_id      = module.app_service_plan.this_id
  https_only               = "true"
  dotnet_framework_version = "v4.0"
  always_on                = "true"
  run_from_package         = "0"
  identity_type            = "SystemAssigned"
  eng_outgoing_ip          = var.API_ENG_OUTGOING_IP
  ukho_main_outgoing_ip    = var.API_UKHO_MAIN_OUTGOING_IP
  spoke_vnet_name          = var.SPOKE_VNET_NAME
  spoke_subnet_name        = var.SPOKE_SUBNET_NAME
  spoke_rg                 = var.SPOKE_RG
}

module "event_hub_rg" {
  source             = "./modules/ResourceGroup"
  deploy_environment = var.DEPLOY_ENVIRONMENT
  servicename        = var.SERVICENAME
  role               = local.event_hub_role
  mainlocation       = var.MAIN_LOCATION
}

module "event_hub" {
  source                  = "./modules/EventHub"
  deploy_environment      = var.DEPLOY_ENVIRONMENT
  servicename             = var.SERVICENAME
  role                    = local.event_hub_role
  resource_group_name     = module.event_hub_rg.name
  resource_group_location = module.event_hub_rg.location
  sku                     = "Standard"
}
module "storage_account" {
  source                  = "./modules/StorageAccount"
  deploy_environment      = var.DEPLOY_ENVIRONMENT
  servicename             = var.SERVICENAME
  role                    = local.event_hub_role
  resource_group_name     = module.event_hub_rg.name
  resource_group_location = module.event_hub_rg.location
}
