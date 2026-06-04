using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechMoveGLMS.Shared.Data;
using TechMoveGLMS.Shared.Models.Entities;
using TechMoveGLMS.Shared.Services;
namespace TechMoveGLMS.Controllers
{
    //(Microsoft,2026)
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;
        public ClientController(ApplicationDbContext context,AuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        
        // GET: Client
        public async Task<IActionResult> Index()
        {
            // Check if user is logged in
            var currentUser = _authService.GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
             // Admins see all clients, Clients see only themselves
            if (_authService.IsAdmin())
            {
                // Admin: Show all clients
                var clients = await _context.Clients.ToListAsync();
                return View(clients);
            }
            else
            {
                // Client: Show only their own client record
                if (currentUser.ClientId != null)
                {
                    var client = await _context.Clients
                        .Where(c => c.Id == currentUser.ClientId)
                        .ToListAsync();
                    return View(client);
                }
                // If client user has no linked ClientId, show empty
                return View(new List<Client>());
            }
        }
                
        
        //  Client/Create
        public IActionResult Create()
        {
             // Only Admin can create clients
            if (!_authService.IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            return View();
        }
        
        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            // Only Admin can create clients
            if (!_authService.IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }
        
        // Client/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            // Only Admin can edit clients
            if (!_authService.IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();
            
            return View(client);
        }
        
        // POST: Client/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            // Only Admin can edit clients
            if (!_authService.IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            
            if (id != client.Id) return NotFound();
            
            if (ModelState.IsValid)
            {
                _context.Update(client);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }
        
        // GET: Client/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            // Only Admin can delete clients
            if (!_authService.IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            if (id == null) return NotFound();
            
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();
            
            return View(client);
        }
        
        // POST: Client/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
             // Only Admin can delete clients
            if (!_authService.IsAdmin())
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            var client = await _context.Clients.FindAsync(id);
            if (client != null)
            {
                _context.Clients.Remove(client);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
        // GET: Client/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            
            var client = await _context.Clients
                .Include(c => c.Contracts)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (client == null) return NotFound();
            
            // Check if user can access this client
            if (!_authService.CanAccessClient(client.Id))
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            
            return View(client);
        }
    }
}
//Microsoft, 2026. ASP.NET Core MVC Overview.[Online] Available at:
//https://learn.microsoft.com/en-us/aspnet/core/mvc/overview?view=aspnetcore-10.0