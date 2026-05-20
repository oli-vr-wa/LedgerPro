using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LedgerPro.Application.Interfaces.Repositories;
using LedgerPro.Application.Interfaces.Services;
using LedgerPro.Application.Services;
using NSubstitute;

namespace LedgerPro.Tests.Application.Services.BankSourceServiceTests;

public class BankSourceServiceTestsBase
{
    protected readonly IGeneralLedgerRepository _generalLedgerRepository = Substitute.For<IGeneralLedgerRepository>();
    protected readonly IBankSourceRepository _bankSourceRepository = Substitute.For<IBankSourceRepository>();
    protected readonly IBankSourceService _bankSourceService;

    public BankSourceServiceTestsBase()
    {
        _bankSourceService = new BankSourceService(_bankSourceRepository, _generalLedgerRepository);
    }
}
