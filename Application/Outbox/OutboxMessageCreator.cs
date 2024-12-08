using Domain;
using System.Text.Json;

namespace Application.Outbox;

public class OutboxMessageCreator : IOutboxMessageCreator
{
    public OutboxMessage Create<T>(T domainInput)
    {
        // this is a placeholder for real code which would help us with our messages queues
        var json = "{ \"example\": \"of a real outbox message that would communicate with other services\"}";

        return new OutboxMessage(json);
    }
}