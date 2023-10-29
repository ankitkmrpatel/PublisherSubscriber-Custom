using System.ComponentModel.DataAnnotations;

namespace Tracking.Service.MessageBrocker.Models;

public class Message
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime ExpiredTimeUtc { get; set; } = DateTime.UtcNow.AddDays(1);

    [Required]
    public string TopicMessage { get; set; }

    [Required]
    public MessageStatus MessageStatus { get; set; }

    [Required]
    public int SubscriptionId { get; set; }
}

public enum MessageStatus
{
    New,
    Requeted,
    Send
}
