using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Shared.Data;
using TechMoveGLMS.Shared.Models.Entities;
using TechMoveGLMS.Shared.Services.Contracts;
using TechMoveGLMS.Shared.Services.Notifications;

namespace TechMoveGLMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ContractsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IContractFactory _contractFactory;
    private readonly NotificationService _notifications;
    private readonly IWebHostEnvironment _env;

    public ContractsController(ApplicationDbContext context, IContractFactory contractFactory,
        NotificationService notifications, IWebHostEnvironment env)
    {
        _context = context;
        _contractFactory = contractFactory;
        _notifications = notifications;
        _env = env;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll([FromQuery] string? status, [FromQuery] int? clientId)
    {
        var query = _context.Contracts.Include(c => c.Client).AsQueryable();
        if (!string.IsNullOrEmpty(status))
            query = query.Where(c => c.Status == status);
        if (clientId.HasValue)
            query = query.Where(c => c.ClientId == clientId);

        var contracts = await query.Select(c => new
        {
            c.Id,
            c.ClientId,
            ClientName = c.Client!.Name,
            c.StartDate,
            c.EndDate,
            c.ServiceLevel,
            c.Status,
            c.SignedAgreementPath
        }).ToListAsync();

        return Ok(contracts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var contract = await _context.Contracts
            .Include(c => c.Client)
            .Where(c => c.Id == id)
            .Select(c => new
            {
                c.Id,
                c.ClientId,
                ClientName = c.Client!.Name,
                c.StartDate,
                c.EndDate,
                c.ServiceLevel,
                c.Status,
                c.SignedAgreementPath
            })
            .FirstOrDefaultAsync();

        if (contract == null) return NotFound();
        return Ok(contract);
    }

    [HttpPost]
    public async Task<ActionResult<Contract>> Create(
        [FromForm] int clientId,
        [FromForm] string serviceLevel,
        [FromForm] DateTime startDate,
        [FromForm] DateTime endDate,
        IFormFile? signedAgreement)
    {
        var contract = _contractFactory.CreateContract(serviceLevel, clientId, startDate, endDate);
        if (signedAgreement != null && signedAgreement.Length > 0)
        {
            var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = Guid.NewGuid() + "_" + signedAgreement.FileName;
            var filePath = Path.Combine(uploadsFolder, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await signedAgreement.CopyToAsync(stream);
            contract.SignedAgreementPath = "/uploads/" + fileName;
        }
        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();
        await _notifications.NotifyAllAsync("New Contract", $"Contract {contract.Id} created");
        return CreatedAtAction(nameof(GetById), new { id = contract.Id }, contract);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null) return NotFound();
        contract.Status = status;
        await _context.SaveChangesAsync();
        await _notifications.NotifyAllAsync("Status Updated", $"Contract {id} status = {status}");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var contract = await _context.Contracts.FindAsync(id);
        if (contract == null) return NotFound();
        _context.Contracts.Remove(contract);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
