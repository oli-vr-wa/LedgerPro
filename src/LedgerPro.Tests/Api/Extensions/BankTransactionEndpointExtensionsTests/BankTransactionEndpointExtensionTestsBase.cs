using NSubstitute;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace LedgerPro.Tests.Api.Extensions.BankTransactionEndpointExtensionsTests;

public class BankTransactionEndpointExtensionsTestsBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> _factory;
    protected readonly IBankTransactionRepository _bankTransactionRepository = Substitute.For<IBankTransactionRepository>();
    protected readonly IBankTransactionService _bankTransactionService = Substitute.For<IBankTransactionService>();
    protected readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public BankTransactionEndpointExtensionsTestsBase(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Replace the actual services with the mocked ones
                services.AddScoped(_ => _bankTransactionRepository);
                services.AddScoped(_ => _bankTransactionService);
                services.AddScoped(_ => _unitOfWork);
            });
        });
    }
}
