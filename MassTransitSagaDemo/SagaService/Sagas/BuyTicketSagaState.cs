using MassTransit;
using Messages;

namespace SagaService;

[ChannelName("saga:state:buy-tickets")]
public class BuyTicketSagaState : SagaStateMachineInstance, ISagaVersion
{
    // state function
    public string CurrentState { get; set; }
    public int Version { get; set; } = 1;

    // state data
    public Guid CorrelationId { get; set; }
    public string UserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastChanged { get; set; }
    public string CourseId { get; set; }
    public string FailReason { get; set; }
    public bool Reserved { get; set; }
    public string BasketId { get; set; }
}

public class BuyTicketSaga
    : MassTransitStateMachine<BuyTicketSagaState>
{
    public Event<ISagaCommandBuyTicket> CommandBuyTicket { get; set; }
    public Event<ITicketsMessageTicketReserved> EventTicketReserved { get; set; }
    public Event<ITicketsMessageTicketNotReserved> EventTicketNotReserved { get; set; }
    public Event<IBasketMessageBasketCreated> EventBasketCreated { get; set; }
    public Event<IBasketMessageBasketNotCreated> EventBasketNotCreated { get; set; }

    public State StateReserveInProgress { get; set; }
    public State StateBasketCreationInProgress { get; set; }
    public State StateOrderCreationInProgress { get; set; }
    public State StateCompleted { get; set; }

    public BuyTicketSaga()
    {
        Event(() => CommandBuyTicket, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => EventTicketReserved, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => EventTicketNotReserved, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => EventBasketCreated, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => EventBasketNotCreated, x => x.CorrelateById(m => m.Message.CorrelationId));

        InstanceState(x => x.CurrentState);

        Initially(
            When(CommandBuyTicket)
                .Then(context =>
                {
                    context.Saga.LastChanged = DateTimeOffset.UtcNow;
                    context.Saga.CreatedAt = DateTimeOffset.UtcNow;
                    context.Saga.UserId = context.Message.UserId;
                    context.Saga.CorrelationId = context.Message.CorrelationId;
                    context.Saga.CourseId = context.Message.CourseId;
                })
                .PublishAsync(context => context.Init<ITicketsCommandReserveTicket>(
                    CraeteTicketsCommandReserveTicket(context)
                ))
                .TransitionTo(StateReserveInProgress));

        During(StateReserveInProgress,
            Ignore(CommandBuyTicket),
            When(EventTicketNotReserved)
                .Then(async context =>
                {
                    context.Saga.LastChanged = DateTimeOffset.UtcNow;
                    context.Saga.FailReason = context.Message.Reason;
                })
                .TransitionTo(StateCompleted)
                .Finalize(),
            When(EventTicketReserved)
                .Then(context =>
                {
                    context.Saga.Reserved = true;
                    context.Saga.LastChanged = DateTimeOffset.UtcNow;
                })
                .ThenAsync(async context =>
                {
                    await context.Publish<IBasketCommandCreateBasket>(CreateBasketCommandCreateBasket(context));
                }).TransitionTo(StateBasketCreationInProgress));

        During(StateBasketCreationInProgress,
            Ignore(CommandBuyTicket),
            When(EventBasketNotCreated)
                .Then(async context =>
                {
                    context.Saga.LastChanged = DateTimeOffset.UtcNow;
                    context.Saga.FailReason = context.Message.Reason;
                })
                .TransitionTo(StateCompleted)
                .Finalize(),
            When(EventBasketCreated)
                .Then(context =>
                {
                    context.Saga.BasketId = context.Message.BasketId;
                    context.Saga.LastChanged = DateTimeOffset.UtcNow;
                })
                .TransitionTo(StateOrderCreationInProgress));

        SetCompletedWhenFinalized();
    }

    private static IBasketCommandCreateBasket CreateBasketCommandCreateBasket(BehaviorContext<BuyTicketSagaState, ITicketsMessageTicketReserved> context)
    {
        return (IBasketCommandCreateBasket)new BasketCommandCreateBasket(
            context.Saga.CorrelationId,
            context.Saga.UserId);
    }

    private static ITicketsCommandReserveTicket CraeteTicketsCommandReserveTicket(BehaviorContext<BuyTicketSagaState, ISagaCommandBuyTicket> context)
    {
        return new TicketsCommandReserveTicket(
            correlationId: context.Message.CorrelationId,
            userId: context.Message.UserId,
            courseId: context.Message.CourseId);
    }
}