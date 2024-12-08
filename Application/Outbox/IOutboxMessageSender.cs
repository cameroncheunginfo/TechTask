using Domain;

namespace Application.Outbox;

public interface IOutboxMessageSender
{
    Task Send(OutboxMessage outboxMessage);
}