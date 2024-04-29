using MassTransit;

namespace SagaService;

public class DefinitionBuyTicketSagaState : SagaDefinition<BuyTicketSagaState>
{
    protected override void ConfigureSaga(
        IReceiveEndpointConfigurator endpointConfigurator, 
        ISagaConfigurator<BuyTicketSagaState> sagaConfigurator)
    {
        endpointConfigurator.UseMessageRetry(
            r => r.Intervals(
                TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10)));

        endpointConfigurator.UseInMemoryOutbox();
    }
}