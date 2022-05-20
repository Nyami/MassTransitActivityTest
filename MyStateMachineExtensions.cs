using MassTransit;

namespace ActivityTest;

public static class MyStateMachineExtensions
{
    public static EventActivityBinder<MyTestState, SomeEvent> CopyDataToInstance(this EventActivityBinder<MyTestState, SomeEvent> binder)
    {
        return binder.Then(context =>
        {
            context.Saga.Something = context.Message.MyId.ToString();
        });
    }

    public static EventActivityBinder<MyTestState, SomeEvent> RandomFailure(this EventActivityBinder<MyTestState, SomeEvent> binder)
    {
        return binder.Activity(ctx => ctx.OfType<RandomFailure>());
    }

    public static ExceptionActivityBinder<MyTestState, SomeEvent, Exception> WhenSomethingBadHappened(this ExceptionActivityBinder<MyTestState, SomeEvent, Exception> binder)
    {
        // adding the activity like this doesn't work as expected when used as Exception Activity
        return binder.Activity(ctx => ctx.OfType<SomethingBadHappened>());

        // But this does, however it cant then use a scoped dependency :(
        //return binder.Add(new SomethingBadHappened(null!));
    }
}
