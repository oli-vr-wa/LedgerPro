using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace LedgerPro.Tests.Api.Extensions.GeneralLedgerEndpointExtensionsTests;

public class GeneralLedgerEndpointExtensionsTestsBase : IClassFixture<WebApplicationFactory<Program>>
{
    protected readonly WebApplicationFactory<Program> _factory;
    protected readonly IGeneralLedgerRepository _generalLedgerRepository = Substitute.For<IGeneralLedgerRepository>();
    protected readonly IGeneralLedgerService _generalLedgerService = Substitute.For<IGeneralLedgerService>();
    protected readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

    public GeneralLedgerEndpointExtensionsTestsBase(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                // Replace the IGeneralLedgerRepository, IGeneralLedgerService, and IUnitOfWork with the mocked instances
                services.AddScoped(_ => _generalLedgerRepository);
                services.AddScoped(_ => _generalLedgerService);
                services.AddScoped(_ => _unitOfWork);
            });
        });
    }
}
