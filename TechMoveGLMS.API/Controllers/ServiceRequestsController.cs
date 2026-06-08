using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Shared.Data;
using TechMoveGLMS.Shared.Models.Entities;
using TechMoveGLMS.Shared.Services;
using TechMoveGLMS.Shared.Services.Notifications;
using TechMoveGLMS.Shared.Services.Pricing;


// I used the Strategy pattern (PricingContext) to calculate costs and the Observer pattern (NotificationService) to log events.
// The currency conversion is done via an external API call (ExchangeRate-API).

namespace TechMoveGLMS.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ServiceRequestsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly CurrencyService _currencyService;
    private readonly PricingContext _pricingContext;
    private readonly NotificationService _notifications;

    public ServiceRequestsController(ApplicationDbContext context, CurrencyService currencyService,
        PricingContext pricingContext, NotificationService notifications)
    {
        _context = context;
        _currencyService = currencyService;
        _pricingContext = pricingContext;
        _notifications = notifications;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var requests = await _context.ServiceRequests
            .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
            .Select(s => new
            {
                s.Id,
                s.ContractId,
                s.Description,
                s.Status,
                s.ServiceLevel,
                s.LocalCost,
                s.CreatedAt,
                ClientName = s.Contract!.Client!.Name,
                ContractServiceLevel = s.Contract.ServiceLevel
            })
            .ToListAsync();

        return Ok(requests);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var request = await _context.ServiceRequests
            .Include(s => s.Contract)
                .ThenInclude(c => c.Client)
            .Where(s => s.Id == id)
            .Select(s => new
            {
                s.Id,
                s.ContractId,
                s.Description,
                s.Status,
                s.ServiceLevel,
                s.LocalCost,
                s.CreatedAt,
                ClientName = s.Contract!.Client!.Name,
                ContractServiceLevel = s.Contract.ServiceLevel
            })
            .FirstOrDefaultAsync();

        if (request == null) return NotFound();
        return Ok(request);
    }

        [HttpPost]
    public async Task<ActionResult<object>> Create(
        [FromForm] int contractId,
        [FromForm] string description,
        [FromForm] string serviceLevel,
        [FromForm] decimal distance,
        [FromForm] bool isPriority)
    {
        var contract = await _context.Contracts.FindAsync(contractId);
        if (contract == null) return BadRequest("Contract does not exist");
        if (contract.Status != "Active" || contract.EndDate < DateTime.Now)
            return BadRequest("Contract is not active or expired");

        decimal baseCostUsd = serviceLevel.ToLower() switch
        {
            "premium" => 250.00m,
            "enterprise" => 500.00m,
            _ => 100.00m
        };
        _pricingContext.SetStrategyByServiceLevel(serviceLevel);
        var finalCostUsd = _pricingContext.ExecuteStrategy(baseCostUsd, serviceLevel, distance, isPriority);
        var localCostZar = await _currencyService.ConvertUsdToZarAsync(finalCostUsd);

        var serviceRequest = new ServiceRequest
        {
            ContractId = contractId,
            Description = description,
            ServiceLevel = serviceLevel,
            Status = "Draft",
            LocalCost = localCostZar,
            CreatedAt = DateTime.Now
        };
        _context.ServiceRequests.Add(serviceRequest);
        await _context.SaveChangesAsync();
        await _notifications.NotifyNewServiceRequestAsync($"Request #{serviceRequest.Id} - {serviceLevel} - R{localCostZar:F2}");

        return CreatedAtAction(nameof(GetById), new { id = serviceRequest.Id }, new
        {
            serviceRequest.Id,
            serviceRequest.ContractId,
            serviceRequest.Description,
            serviceRequest.Status,
            serviceRequest.ServiceLevel,
            serviceRequest.LocalCost,
            serviceRequest.CreatedAt,
            ClientName = contract.Client?.Name ?? "",
            ContractServiceLevel = contract.ServiceLevel
        });
    }[HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var request = await _context.ServiceRequests.FindAsync(id);
        if (request == null) return NotFound();
        request.Status = status;
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
// Microsoft, 2026. ASP.NET Core Web API – form binding and routing.[Online] 
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads
// Refactoring Guru, 2026. Strategy pattern.[Online]
// Available at: https://refactoring.guru/design-patterns/strategy