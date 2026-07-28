namespace InventoryApi.Dtos;

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateCategoryDto
{
    public string Name { get; set; } = string.Empty;
}

public class ReturnCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}