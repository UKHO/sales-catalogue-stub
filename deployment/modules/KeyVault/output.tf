output "vault_uri" {
  value     = azurerm_key_vault.keyvault.vault_uri
  sensitive = true
}

output "id" {
  value     = azurerm_key_vault.keyvault.id
  sensitive = true
}
