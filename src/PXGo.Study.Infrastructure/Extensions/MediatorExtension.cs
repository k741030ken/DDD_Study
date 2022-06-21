using MediatR;
using PXGo.Study.Domain.SeedWork;

namespace PXGo.Study.Infrastructure.Extensions;

/// <summary>
/// Mediator 擴充
/// </summary>
static class MediatorExtension
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator, DBContext ctx)
    {
        var domainEntities = ctx.ChangeTracker.Entries<Entity>().Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());
        var domainEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents).ToList();
        domainEntities.ToList().ForEach(entity => entity.Entity.ClearDomainEvents());
        try
        {
            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent);
        }
        catch (Exception ex)
        {
            var a = ex.ToString();
        }
    }
}
