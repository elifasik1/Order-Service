
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Features.Auth.Login;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.Services;
using OrderService.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OrderService.Application.Features.Auth.Register;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OrderService.API.Extensions;
using OrderService.Application.Features.Orders.GetMyOrders;
using OrderService.API.Middleware;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using AutoMapper;
using OrderService.Application.Mappings;
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddScoped<CreateOrderHandler>();
builder.Services.AddScoped<CreateOrderValidator>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<OrderMappingProfile>();
});
builder.Services.AddScoped<GetOrdersHandler>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});
builder.Services.AddScoped<UpdateOrderHandler>();
builder.Services.AddScoped<UpdateOrderValidator>();
builder.Services.AddScoped<DeleteOrderHandler>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<RefreshHandler>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<MyOrdersHandler>();
builder.Services.AddScoped<GetPagedOrdersHandler>();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings not found.");

var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("UserOrAdmin", policy =>
        policy.RequireRole("Admin", "User"));

    options.AddPolicy("CustomerOnly", policy =>
        policy.RequireRole("Customer"));
});
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<RegisterHandler>();
builder.Services.AddScoped<RegisterValidator>();
var app = builder.Build();
app.UseMiddleware<GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
        
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.MapHealthChecks("/health")
    .WithName("HealthCheck");

app.MapPost("/orders",
    async (
        HttpContext context,
        CreateOrderRequest request,
        CreateOrderValidator validator,
        CreateOrderHandler handler) =>
    {
        var result = validator.Validate(request);

        if (!result.IsValid)
            return Results.BadRequest(result.Errors);

        var userId = context.User.GetUserId();

        var response = await handler.Handle(userId, request);

        if (!response.IsSuccess)
        {
            return Results.BadRequest(response);
        }

        return Results.Ok(response);
    })
    .RequireAuthorization("UserOrAdmin");

app.MapDelete("/orders/{id:guid}",
    async (
        Guid id,
        DeleteOrderHandler handler) =>
    {
        var response = await handler.Handle(id);

        if (!response.IsSuccess)
        {
            return Results.BadRequest(response);
        }

        return Results.Ok(response);
    })
    .RequireAuthorization("AdminOnly");


app.MapPut("/orders/{id:guid}",
    async (
        Guid id,
        UpdateOrderRequest request,
        UpdateOrderValidator validator,
        UpdateOrderHandler handler) =>
    {
        var result = validator.Validate(request);

        if (!result.IsValid)
            return Results.BadRequest(result.Errors);

        var response = await handler.Handle(id, request);

        if (!response.IsSuccess)
        {
            return Results.BadRequest(response);
        }

        return Results.Ok(response);
    })
    .RequireAuthorization("AdminOnly");

app.MapGet("/orders",
async (
    [AsParameters] GetPagedOrdersRequest request,
    GetPagedOrdersHandler handler) =>
{
    var response = await handler.Handle(request);

    return Results.Ok(response);
})
.RequireAuthorization();


app.MapPost("/auth/login",
async (LoginRequest request, LoginHandler handler) =>
{
    var response = await handler.Handle(request);
    return Results.Ok(response);
});



app.MapPost("/auth/register",
async (RegisterRequest request, RegisterValidator validator, RegisterHandler handler) =>
{
    var result = validator.Validate(request);

    if (!result.IsValid)
        return Results.BadRequest(result.Errors);

    var response = await handler.Handle(request);

    return Results.Ok(response);
});


app.MapPost("/auth/refresh",
async (RefreshRequest request, RefreshHandler handler) =>
{
    var response = await handler.Handle(request);

    return Results.Ok(response);
});



app.MapGet("/me", (HttpContext context) =>
{
    return Results.Ok(new
    {
        UserId = context.User.GetUserId(),
        Email = context.User.GetEmail(),
        Role = context.User.GetRole(),
        Name = context.User.GetFullName()
    });
})
.RequireAuthorization();
app.MapGet("/orders/my",
    async (
        HttpContext context,
        MyOrdersHandler handler) =>
    {
        var userId = context.User.GetUserId();

        var orders = await handler.Handle(userId);

        return Results.Ok(orders);
    })
    .RequireAuthorization("UserOrAdmin");



app.Run();
