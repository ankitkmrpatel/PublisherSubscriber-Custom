// See https://aka.ms/new-console-template for more information
using System.Net.Http.Json;
using Tracking.Service.Subscriber.Dtos;

Console.WriteLine("Press ESC to Stop");
do
{
    HttpClient client = new();
    Console.WriteLine("Listing...");

    while (!Console.KeyAvailable)
    {
        var ackIds = await GetMessagesAsync(client);
        Thread.Sleep(2000);

        if (ackIds != null && ackIds.Count > 0)
        {
            await AcknowledgeMessageAsync(client, ackIds);
        }
    }
}
while (Console.ReadKey(true).Key == ConsoleKey.Escape);


static async Task<List<int>> GetMessagesAsync(HttpClient client)
{
    List<int> ackIds = new();
    List<MessageReadDto>? newMessages;

    try
    {
        //newMessages = await client.GetFromJsonAsync<List<MessageReadDto>>("https://localhost:44369/api/subscriptions/1/messages");
        var response = await client.GetAsync("https://localhost:44369/api/subscriptions/1/messages");
        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }
        else
        {
            newMessages = await response.Content.ReadFromJsonAsync<List<MessageReadDto>>();
            if (null != newMessages && newMessages.Any())
            {
                foreach (var message in newMessages)
                {
                    Console.WriteLine($"Message Recived : {message.Id} - {message.TopicMessage} - {message.MessageStatus}");
                    ackIds.Add(message.Id);
                }
            }
        }
    }
    catch (HttpRequestException ex)
    {

    }

    return ackIds;
}

static async Task AcknowledgeMessageAsync(HttpClient client, List<int> ackIds)
{
    try
    {
        var response = await client.PostAsJsonAsync("https://localhost:44369/api/subscriptions/1/messages", ackIds);
        var returnMessage = await response.Content.ReadAsStringAsync();

        Console.WriteLine(returnMessage);
    }
    catch (HttpRequestException ex)
    {

    }
}