namespace WebApplication2.Controllers;

public record ApiModelBuyTickets
{
    public required string UserId { get; set; }
    public required string ClassId { get; set; }
}