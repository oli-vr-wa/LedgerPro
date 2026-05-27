using Microsoft.AspNetCore.Mvc.Testing;


namespace LedgerPro.Tests.Api.Extensions.ReportsEndpointExtensionsTests;

public class ReportsEndpointExtensionsTestsBase : IClassFixture<LedgerTestFactory>
{
    protected readonly WebApplicationFactory<Program> _factory;

    public ReportsEndpointExtensionsTestsBase(LedgerTestFactory factory)
    {
        _factory = factory;
    }    
}


