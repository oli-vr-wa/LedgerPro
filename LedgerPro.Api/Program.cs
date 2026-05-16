using LedgerPro.Api.Extensions;
using LedgerPro.Api.Middleware;
using LedgerPro.Application.Interfaces;
using LedgerPro.Application.Services;
using LedgerPro.Core.Interfaces;
using LedgerPro.Core.Services;
using LedgerPro.Infrastructure;
using LedgerPro.Infrastructure.Parsers;
using LedgerPro.Infrastructure.Repositories;
using LedgerPro.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add infrastructure / services to the container.
builder.Services.AddDbContext<LedgerDbContext>(options => 
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddScoped<IBankSourceRepository, BankSourceRepository>();
builder.Services.AddScoped<IBankTransactionRepository, BankTransactionRepository>();
builder.Services.AddScoped<IGeneralLedgerRepository, GeneralLedgerRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBankStatementParser, BankStatementParser>();
builder.Services.AddScoped<ITransactionMatchService, TransactionMatchService>();
builder.Services.AddScoped<IBankTransactionService, BankTransactionService>();
builder.Services.AddScoped<IBankImportService, BankImportService>();
builder.Services.AddScoped<IGeneralLedgerService, GeneralLedgerService>();
builder.Services.AddScoped<IFileHasher, FileHasher>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

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

app.Run();

