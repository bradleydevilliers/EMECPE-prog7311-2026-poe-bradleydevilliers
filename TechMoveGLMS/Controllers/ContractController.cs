using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoveGLMS.Services.ApiClients;
using TechMoveGLMS.Shared.Models.ViewModels;
using TechMoveGLMS.Shared.Models.DTOs;
using TechMoveGLMS.Shared.Data;
using Microsoft.EntityFrameworkCore;

namespace TechMoveGLMS.Controllers;

public class ContractController : Controller
{
    private readonly ContractApiService _contractApi;
    private readonly ApplicationDbContext _context;

    public ContractController(ContractApiService contractApi, ApplicationDbContext context)
    {
        _contractApi = contractApi;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var contracts = await _contractApi.GetAllAsync();
        return View(contracts);
    }

    public IActionResult Create()
    {
        var viewModel = new ContractViewModel
        {
            Clients = _context.Clients.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList() ?? new(),
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
    public async Task<IActionResult> Create(ContractViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        await _contractApi.CreateAsync(model.ClientId, model.ServiceLevel, model.StartDate, model.EndDate, model.SignedAgreement);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var contract = await _contractApi.GetByIdAsync(id);
        if (contract == null) return NotFound();
        var model = new ContractViewModel
        {
            Id = contract.Id,
            ClientId = contract.ClientId,
            ServiceLevel = contract.ServiceLevel,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, ContractViewModel model, string status)
    {
        await _contractApi.UpdateStatusAsync(id, status);
        return RedirectToAction(nameof(Index));
    }
}
