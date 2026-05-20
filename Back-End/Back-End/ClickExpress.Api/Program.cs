using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
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
using ClickExpress.Api.Middleware;

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
    });

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .WithHeaders("Content-Type", "Authorization", "X-Requested-With")
              .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS");
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

app.Run();
