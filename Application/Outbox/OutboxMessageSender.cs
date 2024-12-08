using Domain;

namespace Application.Outbox;

public class OutboxMessageSender : IOutboxMessageSender
{
    public Task Send(OutboxMessage outboxMessage)
    {
        // this is a placeholder for real code which would help us with our messages queues
        return Task.CompletedTask;
    }
}