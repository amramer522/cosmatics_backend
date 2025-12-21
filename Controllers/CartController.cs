using Cosmatics.DTOs;
using Cosmatics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Cosmatics.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        return Ok(await _cartService.GetCartAsync(GetUserId()));
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddToCart(int productId, int quantity)
    {
        await _cartService.AddToCartAsync(GetUserId(), productId, quantity);
        return Ok("Item added to cart.");
    }

    [HttpDelete("remove/{productId}")]
    public async Task<IActionResult> RemoveFromCart(int productId)
    {
        await _cartService.RemoveFromCartAsync(GetUserId(), productId);
        return Ok("Item removed from cart.");
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        await _cartService.ClearCartAsync(GetUserId());
        return Ok("Cart cleared.");
    }
}
