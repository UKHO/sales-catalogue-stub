output "location" {
  value     = azurerm_resource_group.main.location
  sensitive = true
}

output "name" {
  value     = azurerm_resource_group.main.name
  sensitive = true
}

output "id" {
  value     = azurerm_resource_group.main.id
  sensitive = true
}
