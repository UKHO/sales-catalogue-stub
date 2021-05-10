variable "MAIN_LOCATION" {
  type = string
}
variable "DEPLOY_ENVIRONMENT" {
  type = string
}
variable "ENV_CONTEXT" {
  type = string
}
variable "SERVICENAME" {
  type = string
}

variable "WEBSITE_DNS_SERVER" {
  type = string
}
variable "API_ENG_OUTGOING_IP" {
  type = string
}
variable "API_UKHO_MAIN_OUTGOING_IP" {
  type = string
}

variable "SPOKE_VNET_NAME" {
  type = string
}

variable "SPOKE_SUBNET_NAME" {
  type = string
}

variable "SPOKE_RG" {
  type = string
}

variable "KV_ACCESS_POLICY_GROUP_OBJECT_ID" {
  type = string
}

variable "KV_ACCESS_POLICY_PIPELINE_OBJECT_ID" {
  type = string
}

variable "API-MASTEK-IP-1" {
  type = string
}

variable "API-MASTEK-IP-2" {
  type = string
}

variable "API-MASTEK-IP-3" {
  type = string
}

variable "API-MASTEK-IP-JUMP-BOX" {
  type = string
}
