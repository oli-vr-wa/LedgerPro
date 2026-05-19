    using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using NSubstitute;

namespace LedgerPro.Tests.Api.Extensions.BankSourceEndpointExtensionsTests;

public class BankSourceEndpointExtensionsTestsBase
{
    protected readonly IBankImportService _bankImportService = Substitute.For<IBankImportService>();
    protected readonly IBankSourceRepository _bankSourceRepository = Substitute.For<IBankSourceRepository>();
    protected readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
}
