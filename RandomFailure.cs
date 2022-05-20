using MassTransit;
using Microsoft.Extensions.Logging;

namespace ActivityTest;

public class RandomFailure : IStateMachineActivity<MyTestState, SomeEvent>
{
    private readonly ILogger<RandomFailure> logger;

    public RandomFailure(ILogger<RandomFailure> logger)
    {
        this.logger = logger;
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<MyTestState, SomeEvent> context, IBehavior<MyTestState, SomeEvent> next)
    {
        if (new Random().Next(1, 3) == 1)
        {
            logger.LogWarning("Sorry, not sorry");
            throw new Exception("Randomness!");
        }
        logger.LogInformation("All good..");
        await next.Execute(context);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<MyTestState, SomeEvent, TException> context, IBehavior<MyTestState, SomeEvent> next) where TException : Exception
    {
        return next.Faulted(context);
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("RandomFailure");
    }
}
