using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Services.ApiClients;
using TechMoveGLMS.Shared.Models.ViewModels;
using TechMoveGLMS.Shared.Data;

namespace TechMoveGLMS.Controllers;

public class ServiceRequestController : Controller
{
    private readonly ServiceRequestApiService _requestApi;
    private readonly ApplicationDbContext _context;

    public ServiceRequestController(ServiceRequestApiService requestApi, ApplicationDbContext context)
    {
        _requestApi = requestApi;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var requests = await _requestApi.GetAllAsync();
        return View(requests);
    }

    public IActionResult Create()
    {
        var viewModel = new ServiceRequestViewModel
        {
            Contracts = _context.Contracts
                .Include(c => c.Client)
                .Where(c => c.Status == "Active" && c.EndDate >= DateTime.Now)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Client.Name} - {c.ServiceLevel} (Expires: {c.EndDate:dd/MM/yyyy})"
                }).ToList() ?? new(),
            ServiceLevels = new List<SelectListItem>
            {
                new() { Value = "Basic", Text = "Basic" },
                new() { Value = "Premium", Text = "Premium" },
                new() { Value = "Enterprise", Text = "Enterprise" }
            }
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ServiceRequestViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await _requestApi.CreateAsync(model.ContractId, model.Description, model.ServiceLevel, model.Distance, model.IsPriority);
        return RedirectToAction(nameof(Index));
    }
}
