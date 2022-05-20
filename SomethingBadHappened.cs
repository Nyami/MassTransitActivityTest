using MassTransit;
using Microsoft.Extensions.Logging;

namespace ActivityTest;

public class SomethingBadHappened : IStateMachineActivity<MyTestState, SomeEvent>
{
    private readonly ILogger<SomethingBadHappened> logger;

    public SomethingBadHappened(ILogger<SomethingBadHappened> logger)
    {
        this.logger = logger;
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<MyTestState, SomeEvent> context, IBehavior<MyTestState, SomeEvent> next)
    {
        logger?.LogError("A bad thing happened");
        context.Saga.Something = "Wibble";
        await next.Execute(context);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<MyTestState, SomeEvent, TException> context, IBehavior<MyTestState, SomeEvent> next) where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("SomethingBadHappened");
    }
}