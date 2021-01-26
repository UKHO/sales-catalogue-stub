output "this_connection_string" {
  value     = azurerm_app_configuration.appconf.primary_read_key.0.connection_string
  sensitive = true
}
