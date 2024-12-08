namespace Domain;

public class OutboxMessage
{
    private OutboxMessage() {}

    public OutboxMessage(string payloadJson)
    {

        MessageId = Guid.NewGuid().ToString();
        Created = DateTime.UtcNow;
        RetryCount = 0;
        Payload = payloadJson;
    }

    public int OutboxMessageId { get; private set; }
    public string MessageId { get; private set; }
    public DateTime Created { get; private set; }
    public int RetryCount { get; private set; }
    public string Payload { get; private set; }
}