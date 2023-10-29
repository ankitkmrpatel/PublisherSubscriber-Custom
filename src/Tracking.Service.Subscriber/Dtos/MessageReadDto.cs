namespace Tracking.Service.Subscriber.Dtos;

public class MessageReadDto
{
    public int Id { get; set; }
    public DateTime ExpiredTimeUtc { get; set; }
    public string TopicMessage { get; set; }
    public MessageStatus MessageStatus { get; set; }
}

public enum MessageStatus
{
    New,
    Requeted,
    Send
}
