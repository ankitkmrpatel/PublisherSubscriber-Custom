using Microsoft.EntityFrameworkCore;
using Tracking.Service.MessageBrocker.Data;
using Tracking.Service.MessageBrocker.Models;

namespace Tracking.Service.MessageBrocker.Endpoints;

public static class TopicEndpoints
{
    internal static void MapTopicEndpoints(this IEndpointRouteBuilder app)
    {
        //Create Topic
        app.MapPost("api/topics", CreateNewTopics)
            .WithName("CreateTopics");

        //Get all Topic
        app.MapGet("api/topics", GetAllTopics)
            .WithName("GetAllTopics");

        //Create Subscription
        app.MapPost("api/topics/{id}/subscriptions", CreateSubscription)
            .WithName("CreateSubscription");

        //Publish Message
        app.MapPost("api/topics/{id}/message", PublishMessage)
            .WithName("PublishMessage");
    }

    private static async Task<IResult> CreateNewTopics(ApplicationDbContext context, Topic topic)
    {
        await context.Topics.AddAsync(topic);
        await context.SaveChangesAsync();

        return Results.Created($"api/topic/{topic.Id}", topic);
    }

    private static async Task<IResult> GetAllTopics(ApplicationDbContext context)
    {
        var topics = await context.Topics.AsNoTracking()
            .ToListAsync();

        return Results.Ok(topics);
    }

    private static async Task<IResult> CreateSubscription(ApplicationDbContext context, int id, Subscription sub)
    {
        var topicExists = await context.Topics.AnyAsync(x => x.Id.Equals(id));
        if (!topicExists)
        {
            return Results.NotFound("Topic not found.");
        }

        sub.TopicId = id;

        await context.Subscriptions.AddAsync(sub);
        await context.SaveChangesAsync();

        return Results.Created($"api/topic/{id}/subscription/{sub.Id}", sub);
    }

    private static async Task<IResult> PublishMessage(ApplicationDbContext context, int id, Message message)
    {
        var topicExists = await context.Topics.AnyAsync(x => x.Id.Equals(id));
        if (!topicExists)
        {
            return Results.NotFound("Topic Not Found.");
        }

        var subs = await context.Subscriptions
            .Where(x => x.TopicId.Equals(id)).AsNoTracking()
            .ToListAsync();

        if (null == subs || !subs.Any())
        {
            return Results.NotFound("No Subscriptions Found For Topic.");
        }

        var allMessage = subs.Select(x => new Message()
        {
            MessageStatus = MessageStatus.New,
            SubscriptionId = x.Id,
            TopicMessage = message.TopicMessage
        });

        await context.Messages.AddRangeAsync(allMessage);
        await context.SaveChangesAsync();

        return Results.Ok("Messages has been publised.");
    }


    //private static Func<ApplicationDbContext, Topic, Task<IResult>> CreateNewTopics()
    //{
    //    return async (ApplicationDbContext context, Topic topic) =>
    //    {
    //        await context.Topics.AddAsync(topic);
    //        await context.SaveChangesAsync();

    //        return Results.Created($"api/topic/{topic.Id}", topic);
    //    };
    //}

    //private static Func<ApplicationDbContext, Task<IResult>> GetAllTopics()
    //{
    //    return async (ApplicationDbContext context) =>
    //    {
    //        var topics = await context.Topics.AsNoTracking()
    //            .ToListAsync();

    //        return Results.Ok(topics);
    //    };
    //}

    //private static Func<ApplicationDbContext, int, Subscription, Task<IResult>> CreateSubscription()
    //{
    //    return async (ApplicationDbContext context, int id, Subscription sub) =>
    //    {
    //        var topicExists = await context.Topics.AnyAsync(x => x.Id.Equals(id));
    //        if (!topicExists)
    //        {
    //            return Results.NotFound("Topic not found.");
    //        }

    //        sub.TopicId = id;

    //        await context.Subscriptions.AddAsync(sub);
    //        await context.SaveChangesAsync();

    //        //return Results.Created($"api/subscription/{sub.Id}", sub);
    //        return Results.Created($"api/topic/{id}/subscription/{sub.Id}", sub);
    //    };
    //}

    //private static Func<ApplicationDbContext, int, Message, Task<IResult>> PublishMessage()
    //{
    //    return async (ApplicationDbContext context, int id, Message message) =>
    //    {
    //        var topicExists = await context.Topics.AnyAsync(x => x.Id.Equals(id));
    //        if (!topicExists)
    //        {
    //            return Results.NotFound("Topic Not Found.");
    //        }

    //        var subs = await context.Subscriptions
    //            .Where(x => x.TopicId.Equals(id)).AsNoTracking()
    //            .ToListAsync();

    //        if (null == subs || !subs.Any())
    //        {
    //            return Results.NotFound("No Subscriptions Found For Topic.");
    //        }

    //        var allMessage = subs.Select(x => new Message()
    //        {
    //            MessageStatus = MessageStatus.New,
    //            SubscriptionId = x.Id,
    //            TopicMessage = message.TopicMessage
    //        });

    //        await context.Messages.AddRangeAsync(allMessage);
    //        await context.SaveChangesAsync();

    //        return Results.Ok("Messages has been publised.");
    //    };
    //}
}
