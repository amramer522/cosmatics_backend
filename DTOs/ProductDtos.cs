using System.ComponentModel.DataAnnotations;

namespace Cosmatics.DTOs;

public record ProductCreateDto([Required] string Name, string Description, [Range(0.01, double.MaxValue)] decimal Price, int Stock, string ImageUrl);

public record ProductUpdateDto(string Name, string Description, decimal Price, int Stock, string ImageUrl);
