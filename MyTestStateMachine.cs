using MassTransit;
using Microsoft.Extensions.Logging;

namespace ActivityTest;

public class MyTestStateMachine : MassTransitStateMachine<MyTestState>
{
    public MyTestStateMachine(ILogger<MyTestStateMachine> logger)
    {
        InstanceState(x => x.CurrentState);

        Initially(
            When(OnSomeEvent)
                .Then(context => logger.LogInformation(FormattableString.Invariant($"message {context.Message} received by Saga")))
                .CopyDataToInstance()
                .RandomFailure()
                .TransitionTo(NextStage)
                .Catch<Exception>(xab =>
                    xab
                        .Then(context => logger.LogWarning(FormattableString.Invariant($"Exception in Saga")))
                        .WhenSomethingBadHappened()
                        .Finalize()
                    ));

        Event(() => OnSomeEvent, e =>
        {
            e.CorrelateById(i => i.CorrelationId, x => x.Message.MyId);
            e.SelectId(_ => NewId.NextGuid());
        });

        SetCompletedWhenFinalized();
    }

    public State NextStage { get; } = null!;

    public Event<SomeEvent> OnSomeEvent { get; } = null!;
}
