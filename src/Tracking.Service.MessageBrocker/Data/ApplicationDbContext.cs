using Microsoft.EntityFrameworkCore;
using Tracking.Service.MessageBrocker.Models;

namespace Tracking.Service.MessageBrocker.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
}
