// This DTO flattens the navigation properties (ClientName instead of Client.Name) to avoid serialisation cycles.

namespace TechMoveGLMS.Shared.Models.DTOs;

public class ContractDto
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string ClientName { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ServiceLevel { get; set; } = "";
    public string Status { get; set; } = "";
    public string? SignedAgreementPath { get; set; }
}

// Microsoft, 2026. Data Transfer Object pattern.[Online] 
// Available at: https://learn.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5