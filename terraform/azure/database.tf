resource "azurerm_postgresql_flexible_server" "main" {
  name                = "inventoryapi-db"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location

  sku_name   = "B_Standard_B1ms"
  storage_mb = 32768
  version    = "16"

  administrator_login    = var.db_username
  administrator_password = var.db_password

  tags = {
    Project = "InventoryApi"
  }
}