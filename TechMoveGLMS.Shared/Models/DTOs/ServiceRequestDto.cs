namespace TechMoveGLMS.Shared.Models.DTOs;

public class ServiceRequestDto
{
    public int Id { get; set; }
    public int ContractId { get; set; }
    public string Description { get; set; } = "";
    public string Status { get; set; } = "";
    public string ServiceLevel { get; set; } = "";
    public decimal LocalCost { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ClientName { get; set; } = "";
    public string ContractServiceLevel { get; set; } = "";
}
