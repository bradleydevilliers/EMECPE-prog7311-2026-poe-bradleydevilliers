using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Shared.Data;
using TechMoveGLMS.Shared.Models.Entities;


// I created this controller to handle CRUD operations for clients. All endpoints require a valid JWT token.

namespace TechMoveGLMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ClientsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var clients = await _context.Clients
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.ContactDetails,
                c.Region
            })
            .ToListAsync();
        return Ok(clients);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var client = await _context.Clients
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.Name,
                c.ContactDetails,
                c.Region,
                Contracts = c.Contracts.Select(co => new
                {
                    co.Id,
                    co.ServiceLevel,
                    co.StartDate,
                    co.EndDate,
                    co.Status
                })
            })
            .FirstOrDefaultAsync();

        if (client == null) return NotFound();
        return Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<Client>> Create(Client client)
    {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Client client)
    {
        if (id != client.Id) return BadRequest();
        _context.Entry(client).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var client = await _context.Clients.FindAsync(id);
        if (client == null) return NotFound();
        _context.Clients.Remove(client);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

// Microsoft, 2026. ASP.NET Core Web API controllers.[Online] 
// Available at: https://learn.microsoft.com/en-us/aspnet/core/web-api/