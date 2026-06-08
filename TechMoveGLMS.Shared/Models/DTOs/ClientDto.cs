// I created this DTO to break JSON circular references and to return only the fields the frontend needs.

namespace TechMoveGLMS.Shared.Models.DTOs;

public class ClientDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string ContactDetails { get; set; } = "";
    public string Region { get; set; } = "";
}

// Microsoft, 2026. Data Transfer Object pattern.[Online] 
// Available at: https://learn.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5