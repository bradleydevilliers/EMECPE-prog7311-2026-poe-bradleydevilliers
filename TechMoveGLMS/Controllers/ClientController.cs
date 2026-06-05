using Microsoft.AspNetCore.Mvc;
using TechMoveGLMS.Services.ApiClients;
using TechMoveGLMS.Shared.Models.DTOs;
using TechMoveGLMS.Shared.Models.Entities;

namespace TechMoveGLMS.Controllers;

public class ClientController : Controller
{
    private readonly ClientApiService _clientApi;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClientController(ClientApiService clientApi, IHttpContextAccessor httpContextAccessor)
    {
        _clientApi = clientApi;
        _httpContextAccessor = httpContextAccessor;
    }

    private bool IsAdmin() => _httpContextAccessor.HttpContext?.Session.GetString("UserRole") == "Admin";
    private int? CurrentClientId() => _httpContextAccessor.HttpContext?.Session.GetInt32("ClientId");

    public async Task<IActionResult> Index()
    {
        var allClients = await _clientApi.GetAllAsync();
        if (!IsAdmin())
        {
            var myId = CurrentClientId();
            if (myId.HasValue)
                allClients = allClients.Where(c => c.Id == myId.Value).ToList();
            else
                allClients = new List<ClientDto>();
        }
        return View(allClients);
    }

    public IActionResult Create()
    {
        if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Client client)
    {
        if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");
        if (!ModelState.IsValid) return View(client);
        await _clientApi.CreateAsync(client);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");
        var client = await _clientApi.GetByIdAsync(id);
        if (client == null) return NotFound();
        return View(client);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, Client client)
    {
        if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");
        if (id != client.Id) return NotFound();
        if (!ModelState.IsValid) return View(client);
        await _clientApi.UpdateAsync(client);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");
        var client = await _clientApi.GetByIdAsync(id);
        if (client == null) return NotFound();
        return View(client);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!IsAdmin()) return RedirectToAction("AccessDenied", "Account");
        await _clientApi.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var client = await _clientApi.GetByIdAsync(id);
        if (client == null) return NotFound();

        if (!IsAdmin() && CurrentClientId() != id)
            return RedirectToAction("AccessDenied", "Account");

        return View(client);
    }
}
