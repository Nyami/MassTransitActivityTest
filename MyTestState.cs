using MassTransit;

namespace ActivityTest;

public class MyTestState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }

    public string CurrentState { get; set; } = null!;

    public string Something { get; set; } = "Default";

    public byte[] RowVersion { get; set; } = null!;
}
