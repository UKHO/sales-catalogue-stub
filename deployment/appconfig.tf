module "appconfig" {
  source                          = "./modules/AppConfig"
  deploy_environment              = var.DEPLOY_ENVIRONMENT
  servicename                     = var.SERVICENAME
  role                            = local.appconfig-role
  mainlocation                    = var.MAIN_LOCATION
}