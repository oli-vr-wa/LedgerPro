    using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;

namespace LedgerPro.Tests.Api.Extensions.BankSourceEndpointExtensionsTests;

public class BankSourceEndpointExtensionsTestsBase
{
    protected readonly IBankImportService _bankImportService = Substitute.For<IBankImportService>();
    protected readonly IBankSourceService _bankSourceService = Substitute.For<IBankSourceService>();
    protected readonly IBankSourceRepository _bankSourceRepository = Substitute.For<IBankSourceRepository>();
    protected readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

}
