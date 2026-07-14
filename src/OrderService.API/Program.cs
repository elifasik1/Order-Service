var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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

app.Run();
