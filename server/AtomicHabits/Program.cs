using AtomicHabits.Data;
using AtomicHabits.Repositories;
using AtomicHabits.Service;
using AtomicHabits.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConn")));


#region DI Registration

#region DI Repositories
builder.Services.AddScoped<IUserRepositories, UserRepositories>();
builder.Services.AddScoped<IHabitRepositories, HabitRepositories>();
builder.Services.AddScoped<IHabitTrackingRepositories, HabitTrackingRepositories>();
builder.Services.AddScoped<IStreakRepositories, StreakRepositories>();
builder.Services.AddScoped<IDashboardRepositories, DashboardRepositories>();

#endregion

#region DI Service
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailSender, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHabitTrackingService, HabitTrackingService>();
builder.Services.AddScoped<IHabitService, HabitService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

#endregion

builder.Services.AddScoped<IJwtKeyProvider, JwtKeyProvider>();

#endregion


// ===== JWT Authentication and Authorization =====
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("Missing JWT key");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "AtomicHabit";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "Piccolo";

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var sp = builder.Services.BuildServiceProvider();
    var provider = sp.GetRequiredService<IJwtKeyProvider>();
    var key = provider.GetKeyAsync().Result; // preload key

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero,
        NameClaimType = "username",
        RoleClaimType = "role"
    };
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AtomicHabits API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer' [space] and then your valid token. Example: Bearer eyJhbGciOi...",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            new string[] {}
        }
    });
});

builder.Services.AddScoped<JwtKeySeeder>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AtomicUI", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "https://localhost:5173",
            "http://localhost:5198",
            "https://localhost:7294"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<JwtKeySeeder>();
    await seeder.SeedInitialJwtKeyAsync();

    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedRolesAsync(dbContext);
}


app.UseCors("AtomicUI");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
