using Cosmatics.Models;
using Cosmatics.DTOs;
using Cosmatics.Data;

namespace Cosmatics.Services;

public class CartService : ICartService
{
    private readonly IRepository<CartItem> _cartRepo;
    private readonly IRepository<Product> _productRepo;

    public CartService(IRepository<CartItem> cartRepo, IRepository<Product> productRepo)
    {
        _cartRepo = cartRepo;
        _productRepo = productRepo;
    }

    public async Task AddToCartAsync(int userId, int productId, int quantity)
    {
        var cartItems = await _cartRepo.FindAsync(c => c.UserId == userId && c.ProductId == productId);
        var existingItem = cartItems.FirstOrDefault();

        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
            await _cartRepo.UpdateAsync(existingItem);
        }
        else
        {
            var cartItem = new CartItem
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity
            };
            await _cartRepo.AddAsync(cartItem);
        }
    }

    public async Task RemoveFromCartAsync(int userId, int productId)
    {
        var cartItems = await _cartRepo.FindAsync(c => c.UserId == userId && c.ProductId == productId);
        var existingItem = cartItems.FirstOrDefault();

        if (existingItem != null)
        {
            await _cartRepo.DeleteAsync(existingItem);
        }
    }

    public async Task<CartDto> GetCartAsync(int userId)
    {
        var cartItems = await _cartRepo.FindAsync(c => c.UserId == userId);
        var cartItemDtos = new List<CartItemDto>();
        decimal total = 0;

        foreach (var item in cartItems)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                cartItemDtos.Add(new CartItemDto(product.Id, product.Name, item.Quantity, product.Price));
                total += item.Quantity * product.Price;
            }
        }

        return new CartDto(cartItemDtos, total);
    }

    public async Task ClearCartAsync(int userId)
    {
        var cartItems = await _cartRepo.FindAsync(c => c.UserId == userId);
        foreach (var item in cartItems)
        {
            await _cartRepo.DeleteAsync(item);
        }
    }
}
