using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using TechnicalBackendTest.Api.Application.Addresses.Validators;
using TechnicalBackendTest.Api.Application.CurrencyConversion.Validators;
using TechnicalBackendTest.Api.Application.Currencies.Validators;
using TechnicalBackendTest.Api.Application.Users.Validators;
using TechnicalBackendTest.Api.Contracts.Request;
using TechnicalBackendTest.Api.Endpoints;
using TechnicalBackendTest.Api.Infrastructure.Persistence;
using TechnicalBackendTest.Api.Infrastructure.Security;

const string ApiKeySchemeName = "ApiKey";

var builder = WebApplication.CreateBuilder(args);

var apiKey = builder.Configuration["Security:ApiKey"];
if (string.IsNullOrWhiteSpace(apiKey))
{
    throw new InvalidOperationException("Security:ApiKey must be configured.");
}

// esto indica que se use el SQLite y que lo guarde en el archivo especificado
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Agregando los validadores
builder.Services.AddScoped<IValidator<CreateAddressRequest>, CreateAddressRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateAddressRequest>, UpdateAddressRequestValidator>();
builder.Services.AddScoped<IValidator<CreateCurrencyRequest>, CreateCurrencyRequestValidator>();
builder.Services.AddScoped<IValidator<ConvertCurrencyRequest>, ConvertCurrencyRequestValidator>();
builder.Services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Technical Backend Test API",
        Version = "v1",
        Description = "Minimal API for users, addresses, currencies, and currency conversion."
    });

    options.AddSecurityDefinition(ApiKeySchemeName, new OpenApiSecurityScheme
    {
        Description = "API Key required in the X-API-KEY header.",
        Name = "X-API-KEY",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(ApiKeySchemeName, document, null)] = new List<string>()
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Technical Backend Test API v1");
    });
}

app.UseMiddleware<ApiKeyMiddleware>();

// Agregando los endpoints
app.MapUsersEndpoints();
app.MapAddressesEndpoints();
app.MapCurrenciesEndpoints();

app.Run();
