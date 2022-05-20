using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ActivityTest;

public class MyTestStateMap : SagaClassMap<MyTestState>
{
    protected override void Configure(EntityTypeBuilder<MyTestState> entity, ModelBuilder model)
    {
        entity.ToTable("MyTestState");
        entity.Property(x => x.CurrentState).HasMaxLength(64);
        entity.Property(x => x.RowVersion).IsRowVersion();
    }
}