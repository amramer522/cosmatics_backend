namespace Cosmatics.DTOs;

public record CartItemDto(int ProductId, string ProductName, int Quantity, decimal Price);

public record CartDto(List<CartItemDto> Items, decimal Total);
