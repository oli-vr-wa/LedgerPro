using LedgerPro.Api.Extensions;
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

builder.Services.AddScoped<IBankSourceRepository, BankSourceRepository>();
builder.Services.AddScoped<IBankTransactionRepository, BankTransactionRepository>();
builder.Services.AddScoped<IGeneralLedgerRepository, GeneralLedgerRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBankStatementParser, BankStatementParser>();
builder.Services.AddScoped<ITransactionMatchService, TransactionMatchService>();
builder.Services.AddScoped<IBankImportService, BankImportService>();
builder.Services.AddScoped<IFileHasher, FileHasher>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

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

