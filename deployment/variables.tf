variable "MAIN_LOCATION" {
  type    = string
  default = "uksouth"
}
# variable "KEYVAULTADDRESS" {
#   type = string
#   default = ""
# }
variable "DEPLOY_ENVIRONMENT" {
  default = "DEV"
}
variable "ENV_CONTEXT" {
  type = string  
  default = "Azure"
}
# variable "ENG_OUTGOING_IP" {
#   type= string
# }
# variable "UKHO_MAIN_OUTGOING_IP" {
#   type= string
# }
variable "APPCONFIGURATION" {
  type = string  
  default = ""
}
variable "SERVICENAME" {
  type = string
  default = "SalesCatStub"
}
# variable "WEBSITE_DNS_SERVER" {
#   type = string  
# }
