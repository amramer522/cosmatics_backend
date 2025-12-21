using Cosmatics.Data;
using Cosmatics.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cosmatics.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class NotificationsController : ControllerBase
{
    private readonly AppDbContext _context;

    public NotificationsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var notifications = await _context.Notifications.ToListAsync();
        return Ok(notifications.OrderByDescending(n => n.CreatedAt));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var notif = await _context.Notifications.FindAsync(id);
        if (notif == null) return NotFound();

        _context.Notifications.Remove(notif);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Notification deleted" });
    }

    [HttpDelete("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteAll()
    {
        var all = await _context.Notifications.ToListAsync();
        _context.Notifications.RemoveRange(all);
        await _context.SaveChangesAsync();
        return Ok(new { message = "All Notifications deleted." });
    }
}
