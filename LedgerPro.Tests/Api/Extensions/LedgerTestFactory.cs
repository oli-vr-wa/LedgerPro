using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Application.Services;
using LedgerPro.Infrastructure;
using LedgerPro.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LedgerPro.Tests.Api.Extensions;

public class LedgerTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptors = services.Where(d => d.ServiceType!.FullName!.Contains("DbContextOptions")).ToList();
            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<LedgerDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            services.AddScoped<IGeneralLedgerService, GeneralLedgerService>();
            services.AddScoped<IBankSourceService, BankSourceService>();
            services.AddScoped<IBankSourceRepository, BankSourceRepository>();
            services.AddScoped<IBankTransactionRepository, BankTransactionRepository>();
            services.AddScoped<IGeneralLedgerRepository, GeneralLedgerRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        });
    }
}
