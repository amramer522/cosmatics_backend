using Cosmatics.Data;
using Cosmatics.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cosmatics.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly IRepository<CountryCode> _repo;

    public CountriesController(IRepository<CountryCode> repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CountryCode country)
    {
        var existing = await _repo.FindAsync(c => c.Code == country.Code || c.Name == country.Name);
        if (existing.Any())
        {
            return BadRequest(new { message = "Country code or name already exists." });
        }

        await _repo.AddAsync(country);
        return Ok(country);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var country = await _repo.GetByIdAsync(id);
        if (country == null)
        {
            return NotFound(new { message = "Country code not found." });
        }

        await _repo.DeleteAsync(country);
        return Ok(new { message = "Country code deleted successfully." });
    }
}
