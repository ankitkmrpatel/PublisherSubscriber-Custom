using System.ComponentModel.DataAnnotations;

namespace Tracking.Service.MessageBrocker.Models;

public class Topic
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
}
