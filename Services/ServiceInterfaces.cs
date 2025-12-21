using Cosmatics.Models;
using Cosmatics.DTOs;

namespace Cosmatics.Services;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterDto dto);
    Task<(string? Token, User? User, string? Error)> LoginAsync(LoginDto dto);
    Task<bool> VerifyOtpAsync(VerifyOtpDto dto);
    Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto);
    Task<bool> ResetPasswordAsync(ResetPasswordDto dto);
    Task<bool> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    Task<User?> GetUserByIdAsync(int id);
}

public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task<Product> CreateProductAsync(ProductCreateDto dto);
    Task UpdateProductAsync(int id, ProductUpdateDto dto);
    Task DeleteProductAsync(int id);
}

public interface ICartService
{
    Task AddToCartAsync(int userId, int productId, int quantity);
    Task RemoveFromCartAsync(int userId, int productId);
    Task<CartDto> GetCartAsync(int userId);
    Task ClearCartAsync(int userId);
}

public interface IOrderService
{
    Task<Order> PlaceOrderAsync(int userId, CreateOrderDto dto);
    Task<IEnumerable<Order>> GetUserOrdersAsync(int userId);
    Task<Order?> GetOrderByIdAsync(int orderId);
}

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(CreateCategoryDto dto);
    Task UpdateCategoryAsync(int id, UpdateCategoryDto dto);
    Task DeleteCategoryAsync(int id);
}
