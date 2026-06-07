using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoveGLMS.Services.ApiClients;
using TechMoveGLMS.Shared.Models.ViewModels;
namespace TechMoveGLMS.Controllers;

public class ServiceRequestController : Controller
{
    private readonly ServiceRequestApiService _requestApi;
      private readonly ContractApiService _contractApi;
    public ServiceRequestController(ServiceRequestApiService requestApi, ContractApiService contractApi)
    {
        _requestApi = requestApi;
               _contractApi = contractApi;
        }

    public async Task<IActionResult> Index()
    {
        var requests = await _requestApi.GetAllAsync();
        return View(requests);
    }

        public async Task<IActionResult> Create()
    {
        var allContracts = await _contractApi.GetAllAsync();
        var activeContracts = allContracts.Where(c => c.Status == "Active" && c.EndDate >= DateTime.Now).ToList();
        var viewModel = new ServiceRequestViewModel
        {
            Contracts = activeContracts.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.ClientName} - {c.ServiceLevel} (Expires: {c.EndDate:dd/MM/yyyy})"
            }).ToList(),
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
