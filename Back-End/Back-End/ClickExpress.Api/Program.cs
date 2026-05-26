using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;
using ClickExpress.DataAccess;
using ClickExpress.DataAccess.Context;
using ClickExpress.BusinessLogic.Interfaces;
using ClickExpress.BusinessLogic.Functions.User;
using ClickExpress.BusinessLogic.Functions.Product;
using ClickExpress.BusinessLogic.Functions.Order;
using ClickExpress.BusinessLogic.Functions.Cart;
using ClickExpress.BusinessLogic.Functions.Vehicle;
using ClickExpress.BusinessLogic.Functions.Driver;
using ClickExpress.BusinessLogic.Functions.Review;
using ClickExpress.BusinessLogic.Functions.Lead;
using ClickExpress.BusinessLogic.Functions.JobApplication;
using ClickExpress.BusinessLogic.Functions.News;
using ClickExpress.BusinessLogic.Functions.SavedLoad;
using ClickExpress.BusinessLogic.Functions.Notification;
using ClickExpress.Api.Middleware;
using ClickExpress.Api.Hubs;
using ClickExpress.BusinessLogic.Helpers;

var builder = WebApplication.CreateBuilder(args);

DbSession.ConnectionStrings = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

builder.Services.AddScoped<IUserActions, UserFlow>();
builder.Services.AddScoped<IProductActions, ProductFlow>();
builder.Services.AddScoped<IOrderActions, OrderFlow>();
builder.Services.AddScoped<ICartActions, CartFlow>();
builder.Services.AddScoped<IVehicleActions, VehicleFlow>();
builder.Services.AddScoped<IDriverActions, DriverFlow>();
builder.Services.AddScoped<IReviewActions, ReviewFlow>();
builder.Services.AddScoped<ILeadActions, LeadFlow>();
builder.Services.AddScoped<IJobApplicationActions, JobApplicationFlow>();
builder.Services.AddScoped<INewsActions, NewsFlow>();
builder.Services.AddScoped<ISavedLoadActions, SavedLoadFlow>();
builder.Services.AddScoped<INotificationActions, NotificationFlow>();
builder.Services.AddSingleton<IEmailService, SmtpEmailService>();
builder.Services.AddSignalR();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
        // SignalR sends the token via query string because WebSockets don't support headers
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(token) &&
                    context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    });

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
              .AllowCredentials(); // required for SignalR WebSocket handshake
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

using (var db = new UserContext())
{
    db.Database.EnsureCreated();
}

using (var db = new OrderContext())
{
    db.Database.ExecuteSqlRaw(@"
        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Notifications' AND xtype='U')
        CREATE TABLE Notifications (
            Id INT IDENTITY(1,1) PRIMARY KEY,
            UserId INT NOT NULL,
            Title NVARCHAR(200) NOT NULL,
            Body NVARCHAR(1000) NOT NULL,
            Type NVARCHAR(50) NOT NULL DEFAULT 'info',
            IsRead BIT NOT NULL DEFAULT 0,
            CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
        );
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Reviews' AND COLUMN_NAME='Role')
            ALTER TABLE Reviews ADD Role NVARCHAR(100) NULL;
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Reviews' AND COLUMN_NAME='Location')
            ALTER TABLE Reviews ADD Location NVARCHAR(100) NULL;
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Products' AND COLUMN_NAME='ViewCount')
            ALTER TABLE Products ADD ViewCount INT NOT NULL DEFAULT 0;
    ");
}

using (var db = new UserContext())
{

    if (!db.Users.Any())
    {
        db.Users.AddRange(
            new ClickExpress.Domain.Entities.User.UserData
            {
                Username = "admin",
                Email = "admin@clickexpress.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ClickExpress.Domain.Entities.User.UserData
            {
                Username = "user",
                Email = "user@clickexpress.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"),
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ClickExpress API V1");
        c.RoutePrefix = "swagger";
    });
}

app.MapGet("/", () => Results.Redirect("/swagger"));
app.UseMiddleware<ExceptionMiddleware>();

app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    if (!app.Environment.IsDevelopment())
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    await next();
});

if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<OrderHub>("/hubs/orders");

app.Run();
