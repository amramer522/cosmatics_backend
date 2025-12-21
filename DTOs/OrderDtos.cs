namespace Cosmatics.DTOs;

public record CreateOrderDto(string PaymentMethod); 

public record OrderDto(int OrderId, DateTime OrderDate, string Status, decimal TotalAmount);
