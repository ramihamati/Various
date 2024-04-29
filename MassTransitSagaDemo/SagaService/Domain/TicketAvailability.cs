namespace TicketsService.Domain;

public class TicketAvailability
{
    public required string ClassId { get; set; }
    
    public required List<int> TicketCount { get; set; }
}