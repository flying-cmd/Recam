using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Remp.API.Middleware;
using Remp.DataAccess.Data;
using Remp.Models.Entities;
using Remp.Service.Interfaces;
using Remp.Service.Services;
using Remp.Service.Mappers;
using Remp.Service.Validators;
using Serilog;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Remp.Repository.Interfaces;
using Remp.Repository.Repositories;
using Remp.Models.Constants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Serilog
var mongoDbConnectionString = builder.Configuration.GetConnectionString("MongoDb");
builder.Host.UseSerilog((ctx, services, lc) =>
{
    lc.MinimumLevel.Information()
      .Enrich.FromLogContext()
      .WriteTo.Console()

      // UserActivityLog
      .WriteTo.Logger(lc2 => lc2
          .Filter.ByIncludingOnly("LogType = 'UserActivity'")
          .WriteTo.MongoDB(
              mongoDbConnectionString!,
              collectionName: "UserActivityLog"));
});


// Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]!))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(RoleNames.PhotographyCompany, policy =>
        policy.RequireClaim("scope", RoleNames.PhotographyCompany));
    options.AddPolicy(RoleNames.Agent, policy =>
        policy.RequireClaim("scope", RoleNames.Agent));
});

// AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.LicenseKey = "<License Key Here>";
}, 
typeof(AgentProfile).Assembly);

// Validation
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestDtoValidator>();

// Repositories
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IPhotographyCompanyRepository, AgentRepository>();
builder.Services.AddScoped<IListingCaseRepository, ListingCaseRepository>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPhotographyCompanyService, AgentService>();
builder.Services.AddScoped<IListingCaseService, ListingCaseService>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Swagger
builder.Services.AddSwaggerGen(
    c =>
    {
        c.SwaggerDoc("v1", new() { Title = "Recam API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });
        c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed roles data
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    await AppDbContextSeed.SeedRolesAsyc(serviceProvider);
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
