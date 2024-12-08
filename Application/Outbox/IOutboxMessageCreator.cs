using Domain;

namespace Application.Outbox;

public interface IOutboxMessageCreator
{
    OutboxMessage Create<T>(T domainInput);
}