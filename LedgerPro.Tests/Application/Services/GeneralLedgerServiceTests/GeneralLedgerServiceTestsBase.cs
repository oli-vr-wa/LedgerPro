using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Services;
using NSubstitute;

namespace LedgerPro.Tests.Application.Services.GeneralLedgerServiceTests;

public class GeneralLedgerServiceTestsBase
{
    protected readonly IGeneralLedgerRepository _generalLedgerRepository = Substitute.For<IGeneralLedgerRepository>();
    protected readonly GeneralLedgerService _generalLedgerService;

    public GeneralLedgerServiceTestsBase()
    {
        _generalLedgerService = new GeneralLedgerService(_generalLedgerRepository);
    }
}
