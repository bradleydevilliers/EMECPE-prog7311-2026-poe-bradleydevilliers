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
