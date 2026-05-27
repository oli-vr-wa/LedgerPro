using LedgerPro.Api.Extensions;
using LedgerPro.Api.Middleware;
using LedgerPro.Application.Interfaces;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Application.Services;
using LedgerPro.Core.Interfaces;
using LedgerPro.Core.Services;
using LedgerPro.Infrastructure;
using LedgerPro.Infrastructure.Parsers;
using LedgerPro.Infrastructure.Repositories;
using LedgerPro.Infrastructure.Services;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure JSON options to display enums as strings in API responses
builder.Services.Configure<JsonOptions>(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter())
);

// Add infrastructure / services to the container.
builder.Services.AddDbContext<LedgerDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.Scan(scan => scan
    .FromAssemblyOf<Program>()
    .AddClasses(classes => classes.Where(type => 
        type.Name.EndsWith("Service") || 
        type.Name.EndsWith("Repository") || 
        type.Name.EndsWith("Parser") || 
        type.Name.EndsWith("Hasher")))
    .AsImplementedInterfaces()
    .WithScopedLifetime()
);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// Map endpoints
app.MapBankSourcesEndpoints();
app.MapBankTransactionEndpoints();
app.MapGeneralLedgerEndpoints();
app.MapReportsEndpoints();

app.Run();

