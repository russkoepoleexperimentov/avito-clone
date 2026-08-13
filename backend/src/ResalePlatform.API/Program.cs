using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ResalePlatform.API.Hubs;
using ResalePlatform.API.Middleware;
using ResalePlatform.API.Services;
using ResalePlatform.Application;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Infrastructure;
using ResalePlatform.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicy = "frontend";
var frontendOrigin = builder.Configuration["Cors:FrontendOrigin"] ?? "http://localhost:5173";

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        // Enum'ы в JSON как строки ("New"/"Active") — удобнее для фронтенда.
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUserService>();
builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();
builder.Services.AddSignalR();
builder.Services.AddScoped<IChatNotifier, ChatNotifier>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Кнопка "Authorize" в Swagger для передачи Bearer-токена.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT access-токен.",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
            },
            Array.Empty<string>()
        },
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
    options.AddPolicy(CorsPolicy, policy =>
        policy.WithOrigins(frontendOrigin)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials())); // нужно для SignalR

// JWT-аутентификация.
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            ClockSkew = TimeSpan.FromSeconds(30),
        };

        // SignalR не может передать заголовок Authorization при WebSocket —
        // берём токен из query-строки для запросов к хабу.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken) &&
                    context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Раздача загруженных фото из wwwroot (директорию создаём заранее — иначе
// провайдер статики не привяжется, если папки ещё нет).
var webRootPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot");
Directory.CreateDirectory(Path.Combine(webRootPath, "uploads"));
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(webRootPath),
});

app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

// Применяем миграции и наполняем БД начальными данными.
await DbInitializer.InitializeAsync(app.Services);

app.Run();
