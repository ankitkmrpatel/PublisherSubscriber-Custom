using Microsoft.EntityFrameworkCore;
using Tracking.Service.MessageBrocker.Data;
using Tracking.Service.MessageBrocker.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(option => 
    option.UseSqlite("Data Source=MessageBrocker.db"));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapTopicEndpoints();
app.MapSubscriberEndpoints();

app.Run();