variable "servicename" {
  type = string
}
variable "key_vault_address" {
  type = string
}
variable "deploy_environment" {
  type = string
}
variable "env_context" {
  type = string
}
variable "role" {
  type = string
}
variable "appconfiguration" {
  type = string
}
variable "website_dns_server" {
  type = string
}
variable "resource_group_name" {
  type = string
}
variable "resource_group_location" {
  type = string
}
variable "resource_group_id" {
  type = string
}
variable "app_service_plan_id" {
  type = string
}
variable "https_only" {
  type = bool
}
variable "dotnet_framework_version" {
  type = string
}
variable "always_on" {
  type = bool
}
variable "run_from_package" {
  type = string
}
variable "identity_type" {
  type = string
}

variable "eng_outgoing_ip" {
  type = string
}

variable "ukho_main_outgoing_ip" {
  type = string
}

variable "key_vault_id" {
  type = string
}

variable "spoke_vnet_name" {
  type = string
}

variable "spoke_subnet_name" {
  type = string
}

variable "spoke_rg" {
  type = string
}

variable "kv_access_policy_group_object_id" {
  type = string
}
