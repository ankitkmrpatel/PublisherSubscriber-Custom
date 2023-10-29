using Microsoft.EntityFrameworkCore;
using Tracking.Service.MessageBrocker.Data;
using Tracking.Service.MessageBrocker.Models;

namespace Tracking.Service.MessageBrocker.Endpoints;

public static class SubscriberEndpoints
{
    internal static void MapSubscriberEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/subscriptions/{id}/messages", GetAllSubsMessages)
            .WithName("GetAllSubscriptionsMessage");

        //Ack Message for Subs
        app.MapPost("api/subscriptions/{id}/messages", AcknowledgeSubscriptionMessages)
            .WithName("AcknowledgeSubscriptionMessages");
    }

    private static async Task<IResult> GetAllSubsMessages(ApplicationDbContext context, int id)
    {
        var subExists = await context.Subscriptions.AnyAsync(x => x.Id.Equals(id));
        if (!subExists)
        {
            return Results.NotFound("Subscription not found.");
        }

        var messages = await context.Messages
            .Where(x => x.SubscriptionId.Equals(id) && !x.MessageStatus.Equals(MessageStatus.Send))
            .ToListAsync();

        if (null == messages || !messages.Any())
        {
            return Results.NotFound("No New Messages.");
        }

        foreach (var message in messages)
        {
            message.MessageStatus = MessageStatus.Requeted;
        }

        await context.SaveChangesAsync();

        return Results.Ok(messages);
    }

    private static async Task<IResult> AcknowledgeSubscriptionMessages(ApplicationDbContext context, int id, int[] confs)
    {
        if (null == confs || confs.Length == 0)
        {
            return Results.BadRequest("Message Id Not Provided.");
        }

        var subExists = await context.Subscriptions.AnyAsync(x => x.Id.Equals(id));
        if (!subExists)
        {
            return Results.NotFound("Subscription not found.");
        }

        var messageFound = 0;
        foreach (var msgId in confs)
        {
            var message = await context.Messages
                .FirstAsync(x => x.Id.Equals(msgId));

            if (null != message)
            {
                message.MessageStatus = MessageStatus.Send;
                await context.SaveChangesAsync();

                messageFound++;
            }
        }

        return Results.Ok($"Acknowledged Messages {messageFound}/{confs.Length}");
    }
}
