
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<CreateOrderHandler>();
builder.Services.AddScoped<CreateOrderValidator>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<GetOrdersHandler>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/health", () =>
{
    var response = new
    {
        service = "OrderService",
        status = "Healthy",
        timestamp = DateTime.UtcNow,
        
    };
    return response ;
         
})
.WithName("GetHealth");

app.MapPost("/orders",
async (CreateOrderRequest request, CreateOrderValidator validator, CreateOrderHandler handler) =>
{
    var result = validator.Validate(request);
    if (!result.IsValid)
    {
        return Results.BadRequest(result.Errors);
    }
     var response = await handler.Handle(request);
     return Results.Ok(response);
});

app.MapGet("/orders",
async (GetOrdersHandler handler) =>
{
    var response = await handler.Handle();
    return Results.Ok(response);
});

app.Run();
