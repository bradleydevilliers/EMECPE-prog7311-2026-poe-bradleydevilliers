using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoveGLMS.Services.ApiClients;
using TechMoveGLMS.Shared.Models.ViewModels;
using TechMoveGLMS.Shared.Models.DTOs;
using Microsoft.EntityFrameworkCore;


// I replaced the DbContext with ContractApiService and ClientApiService.
// The Create action populates dropdowns by calling the API (GetAllClients), not the database.

namespace TechMoveGLMS.Controllers;

public class ContractController : Controller
{
    private readonly ContractApiService _contractApi;
      private readonly ClientApiService _clientApi;
    public ContractController(ContractApiService contractApi, ClientApiService clientApi)
    {
        _contractApi = contractApi;
              _clientApi = clientApi;     
        }

    public async Task<IActionResult> Index()
    {
        var contracts = await _contractApi.GetAllAsync();
        return View(contracts);
    }

        public async Task<IActionResult> Create()
    {
        var clients = await _clientApi.GetAllAsync();
        var viewModel = new ContractViewModel
        {
            Clients = clients.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name }).ToList(),
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

// Microsoft, 2026. ASP.NET Core MVC – model binding and API integration.[Online] 
// Available at: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/model-binding