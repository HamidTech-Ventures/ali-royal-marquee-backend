using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AliRoyalMarquee.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace AliRoyalMarquee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public class TestController : ControllerBase
{
    private readonly IAppDbContext _context;

    public TestController(IAppDbContext context)
    {
        _context = context;
    }

    [HttpGet("customers")]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await _context.Customers.Select(c => new { c.Id, c.Name }).ToListAsync();
        return Ok(customers);
    }
}
