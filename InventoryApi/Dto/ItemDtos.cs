namespace InventoryApi.Dtos;

public class CreateItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
    public int OwnerId { get; set; }
}

public class UpdateItemDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
    public int OwnerId { get; set; }
}

public class ReturnItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
    public int OwnerId { get; set; }
}