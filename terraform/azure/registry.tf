resource "azurerm_container_registry" "main" {
  name                = "inventoryapiacr"
  resource_group_name = azurerm_resource_group.main.name
  location            = azurerm_resource_group.main.location
  sku                 = "Basic"

  tags = {
    Project = "InventoryApi"
  }
}