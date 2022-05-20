using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;

namespace ActivityTest;

public class MyStateDbContext : SagaDbContext
{
    public MyStateDbContext(DbContextOptions options)
        : base(options)
    {

    }

    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get { yield return new MyTestStateMap(); }
    }
}
